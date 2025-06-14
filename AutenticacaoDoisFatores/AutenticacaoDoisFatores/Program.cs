using AutenticacaoDoisFatores;
using AutenticacaoDoisFatores.Compartilhados;
using AutenticacaoDoisFatores.Dominio.Compartilhados;
using AutenticacaoDoisFatores.Infra.Utilitarios.Migradores;
using AutenticacaoDoisFatores.Infra.Contexto;
using AutenticacaoDoisFatores.Servico.Compartilhados;
using Mensageiro.WebApi;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Npgsql;
using AutenticacaoDoisFatores.Infra.Repositorios;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Configuration.AddEnvironmentVariables();

var CORS_POLICY_ALLOW_ALL = "AllowAll";

builder.Services.AddCors(options =>
{
    options.AddPolicy(CORS_POLICY_ALLOW_ALL, policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var stringDeConexao = Environment.GetEnvironmentVariable("ADF_CONEXAO_BANCO");
if (stringDeConexao is null || stringDeConexao.EstaVazio())
    throw new ApplicationException("A string de conexão com o banco de dados não foi encontrada");

var dataSourceBuilder = new NpgsqlDataSourceBuilder(stringDeConexao);
dataSourceBuilder.EnableDynamicJson();
var dataSource = dataSourceBuilder.Build();

builder.Services.AddDbContext<BaseContext>(opt =>
    opt.UseNpgsql(dataSource)
);

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

builder.Services.AddAuthorization();

var chaveJwt = Seguranca.ChaveDeAutenticacao();

builder.Services.AddAuthentication(opt =>
{
    opt.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    opt.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(opt =>
{
    opt.RequireHttpsMetadata = false;
    opt.SaveToken = true;
    opt.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(chaveJwt),
        
        ValidateIssuer = true,
        ValidIssuer = Seguranca.RetornarEmissor(),
        ValidateAudience = true,
        ValidAudience = Seguranca.RetornarDestinatario(),
        ValidateLifetime = true
    };

    opt.Events = new JwtBearerEvents()
    {
        OnTokenValidated = async (TokenValidatedContext contexto) =>
        {
            var nomeDominio = contexto.Request.Headers["Dominio"].ToString();
            if (nomeDominio.EstaVazio())
            {
                contexto.Fail("O domínio do cliente não foi encontrado");
                return;
            }

            var token = contexto.Request.Headers.Authorization.ToString().Replace("Bearer ", "");
            if (!token.EstaVazio())
            {
                var stringDeConexao = Environment.GetEnvironmentVariable("ADF_CONEXAO_BANCO");
                if (stringDeConexao is null || stringDeConexao.EstaVazio())
                    throw new ApplicationException("A string de conexão com o banco de dados não foi encontrada");

                var contextoCliente = new ClientContext(stringDeConexao, nomeDominio);
                var repositorioUsuarios = new RepositorioDeUsuarios(contextoCliente);
                if (repositorioUsuarios is not null)
                {
                    var idUsuario = Seguranca.RetornarIdDoToken(token);
                    var usuario = await repositorioUsuarios.BuscarUsuarioPorDominioAsync(idUsuario, nomeDominio);
                    if (usuario is null || !usuario.Ativo)
                    {
                        contexto.Fail("Usuário não encontrado");
                        return;
                    }
                }
            }
        }
    };
});

builder.Services.AddAuthorizationBuilder()
    .AddPolicy("ConfirmacaoDeCliente", policy => policy.RequireRole(Seguranca.RegraConfirmacaoDeCliente))
    .AddPolicy("GeracaoNovaChaveCliente", policy => policy.RequireRole(Seguranca.RegraGeracaoNovaChaveCliente))
    .AddPolicy("CriacaoDeUsuario", policy => policy.RequireRole(Seguranca.RegraCriacaoDeUsuario))
    .AddPolicy("AtivacaoDeUsuario", policy => policy.RequireRole(Seguranca.RegraAtivacaoUsuario))
    .AddPolicy("DesativacaoDeUsuario", policy => policy.RequireRole(Seguranca.RegraDesativacaoUsuario))
    .AddPolicy("TrocarSenhaDeUsuario", policy => policy.RequireRole(Seguranca.RegraTrocarSenhaUsuario))
    .AddPolicy("DefinirPermissoes", policy => policy.RequireRole(Seguranca.RegraDefinirPermissoes))
    .AddPolicy("ExclusaoDeUsuario", policy => policy.RequireRole(Seguranca.RegraExclusaoDeUsuario))
    .AddPolicy("VisualizacaoDeUsuarios", policy => policy.RequireRole(Seguranca.RegraVisualizacaoDeUsuarios))
    .AddPolicy("TrocarEmailDeUsuario", policy => policy.RequireRole(Seguranca.RegraTrocarEmailDeUsuario))
    .AddPolicy("CodAutenticaoPorEmail", policy => policy.RequireRole(Seguranca.RegraCodAutenticaoPorEmail));

builder.Services.AddHttpContextAccessor();

builder.Services.AddMensageiro();

builder.Services.AddServicos();
builder.Services.AddRepositorios();
builder.Services.AddDominios();
builder.Services.AddCasosDeUso();
builder.Services.AddContextos();

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<BaseContext>();
    await db.Database.MigrateAsync();

    var migrador = scope.ServiceProvider.GetRequiredService<IMigration>();
    await migrador.ApplyMigrationsAsync();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseCors(CORS_POLICY_ALLOW_ALL);

app.UseMiddleware<Intermediador>();

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.UseStaticFiles();

app.Run();

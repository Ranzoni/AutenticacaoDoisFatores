using AutenticacaoDoisFatores;
using AutenticacaoDoisFatores.Dominio.Compartilhados;
using AutenticacaoDoisFatores.Infra.Compartilhados.Migradores;
using AutenticacaoDoisFatores.Infra.Contexto;
using AutenticacaoDoisFatores.Servico.Compartilhados;
using AutenticacaoDoisFatores.Servico.Mapeadores;
using Mensageiro.WebApi;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Npgsql;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Configuration.AddEnvironmentVariables();

var stringDeConexao = Environment.GetEnvironmentVariable("ADF_CONEXAO_BANCO");
if (stringDeConexao is null || stringDeConexao.EstaVazio())
    throw new ApplicationException("A string de conex�o com o banco de dados n�o foi encontrada");

var dataSourceBuilder = new NpgsqlDataSourceBuilder(stringDeConexao);
dataSourceBuilder.EnableDynamicJson();
var dataSource = dataSourceBuilder.Build();

builder.Services.AddDbContext<ContextoPadrao>(opt =>
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
        
        ValidateIssuer = false,
        ValidateAudience = false
    };
});

builder.Services.AddAuthorizationBuilder()
    .AddPolicy("ConfirmacaoDeCliente", policy => policy.RequireRole(Seguranca.RegraConfirmacaoDeCliente))
    .AddPolicy("GeracaoNovaChaveCliente", policy => policy.RequireRole(Seguranca.RegraGeracaoNovaChaveCliente));

builder.Services.AddAutoMapper(typeof(MapeadorDeCliente));
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

using(var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ContextoPadrao>();
    db.Database.Migrate();

    var migrador = scope.ServiceProvider.GetRequiredService<IMigrador>();
    migrador.AplicarMigracoes();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.UseStaticFiles();

app.Run();

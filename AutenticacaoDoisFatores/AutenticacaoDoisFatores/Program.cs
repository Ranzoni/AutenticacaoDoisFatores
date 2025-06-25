using AutenticacaoDoisFatores;
using AutenticacaoDoisFatores.Domain.Shared;
using AutenticacaoDoisFatores.Infra.Contexts;
using AutenticacaoDoisFatores.Infra.Repositories;
using AutenticacaoDoisFatores.Infra.Utils.Migrants;
using AutenticacaoDoisFatores.Service.Shared;
using AutenticacaoDoisFatores.Shared;
using Messenger.WebApi;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Npgsql;
using System.Security.Claims;

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

var connectionString = Environment.GetEnvironmentVariable("ADF_CONEXAO_BANCO");
if (connectionString is null || connectionString.IsNullOrEmptyOrWhiteSpaces())
    throw new ApplicationException("A string de conexão com o banco de dados não foi encontrada");

var dataSourceBuilder = new NpgsqlDataSourceBuilder(connectionString);
dataSourceBuilder.EnableDynamicJson();
var dataSource = dataSourceBuilder.Build();

builder.Services.AddDbContext<BaseContext>(opt =>
    opt.UseNpgsql(dataSource)
);

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

builder.Services.AddAuthorization();

var jwtAuthKey = Security.AuthKey();

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
        IssuerSigningKey = new SymmetricSecurityKey(jwtAuthKey),
        
        ValidateIssuer = true,
        ValidIssuer = Security.GetIssuer(),
        ValidateAudience = true,
        ValidAudience = Security.GetAudience(),
        ValidateLifetime = true
    };

    opt.Events = new JwtBearerEvents()
    {
        OnTokenValidated = async (TokenValidatedContext context) =>
        {
            var role = context.Principal?.FindFirstValue(ClaimTypes.Role);
            if (string.IsNullOrEmpty(role) || !role.Equals(Security.AuthenticatedUser))
                return;

            var domainName = context.Request.Headers["Dominio"].ToString();
            if (domainName.IsNullOrEmptyOrWhiteSpaces())
            {
                context.Fail("O domínio do cliente não foi encontrado");
                return;
            }

            var token = context.Request.Headers.Authorization.ToString().Replace("Bearer ", "");
            if (!token.IsNullOrEmptyOrWhiteSpaces())
            {
                var connectionString = Environment.GetEnvironmentVariable("ADF_CONEXAO_BANCO");
                if (connectionString is null || connectionString.IsNullOrEmptyOrWhiteSpaces())
                    throw new ApplicationException("A string de conexão com o banco de dados não foi encontrada");

                var clientContext = new ClientContext(connectionString, domainName);
                var userRepository = new UserRepository(clientContext);
                if (userRepository is not null)
                {
                    var userId = Security.GetIdFromToken(token);
                    var user = await userRepository.GetByIdAsync(userId, domainName);
                    if (user is null || !user.Active)
                    {
                        context.Fail("Usuário não encontrado");
                        return;
                    }

                    var tokenLastDataChange = context.Principal?.Claims.First(c => c.Type.Equals(Security.LastDataChange)).Value;
                    if (tokenLastDataChange is not null && !tokenLastDataChange.IsNullOrEmptyOrWhiteSpaces())
                    {
                        if (!user.LastDataChange.Equals(tokenLastDataChange))
                            context.Fail("Invalid token: User password was changed.");
                        return;
                    }
                }
            }
        }
    };
});

builder.Services.AddAuthorizationBuilder()
    .AddPolicy(nameof(Security.ClientConfirmationRole), policy => policy.RequireRole(Security.ClientConfirmationRole))
    .AddPolicy(nameof(Security.NewClientKeyGenerationRole), policy => policy.RequireRole(Security.NewClientKeyGenerationRole))
    .AddPolicy(nameof(Security.CreateUserRole), policy => policy.RequireRole(Security.CreateUserRole))
    .AddPolicy(nameof(Security.AcivateUserRole), policy => policy.RequireRole(Security.AcivateUserRole))
    .AddPolicy(nameof(Security.InactivateUserRole), policy => policy.RequireRole(Security.InactivateUserRole))
    .AddPolicy(nameof(Security.ChangeUserPasswordRole), policy => policy.RequireRole(Security.ChangeUserPasswordRole))
    .AddPolicy(nameof(Security.SetPermissionsRole), policy => policy.RequireRole(Security.SetPermissionsRole))
    .AddPolicy(nameof(Security.RemoveUserRole), policy => policy.RequireRole(Security.RemoveUserRole))
    .AddPolicy(nameof(Security.ViewUsersRole), policy => policy.RequireRole(Security.ViewUsersRole))
    .AddPolicy(nameof(Security.AuthCodeEmailSenderRole), policy => policy.RequireRole(Security.AuthCodeEmailSenderRole))
    .AddPolicy(nameof(Security.AuthenticatedUser), policy => policy.RequireRole(Security.AuthenticatedUser));

builder.Services.AddHttpContextAccessor();

builder.Services.AddMessenger();

builder.Services.AddServices();
builder.Services.AddRepositories();
builder.Services.AddDomains();
builder.Services.AddUseCases();
builder.Services.AddContexts();

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<BaseContext>();
    await db.Database.MigrateAsync();

    var migrator = scope.ServiceProvider.GetRequiredService<IMigration>();
    await migrator.ApplyMigrationsAsync();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseCors(CORS_POLICY_ALLOW_ALL);

app.UseMiddleware<Intermediator>();

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.UseStaticFiles();

app.Run();

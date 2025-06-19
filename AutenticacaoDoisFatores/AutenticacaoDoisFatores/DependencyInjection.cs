using AutenticacaoDoisFatores.Domain.Shared;
using AutenticacaoDoisFatores.Domain.Domains;
using AutenticacaoDoisFatores.Domain.Repositories;
using AutenticacaoDoisFatores.Domain.Services;
using AutenticacaoDoisFatores.Infra.Utils.Migrants;
using AutenticacaoDoisFatores.Infra.Utils.Migrants.Npgsql;
using AutenticacaoDoisFatores.Infra.Contexts;
using AutenticacaoDoisFatores.Infra.Repositories;
using AutenticacaoDoisFatores.Infra.Services;
using AutenticacaoDoisFatores.Service.UseCases.Clients;
using AutenticacaoDoisFatores.Service.UseCases.Permissions;
using AutenticacaoDoisFatores.Service.UseCases.Users;
using AutenticacaoDoisFatores.Service.UseCases.Users.Authenticators;
using AutenticacaoDoisFatores.Service.UseCases.Users.Authenticators.TwoFactorAuths;
using Messenger;

namespace AutenticacaoDoisFatores
{
    internal static class DependencyInjection
    {
        private static readonly string _pathToGenerateQrCode = "authentication/generate-qr-code.html";

        internal static void AddUseCases(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<CreateClient>();
            serviceCollection.AddTransient<ActivateClient>();
            serviceCollection.AddTransient<ResendClientKey>();
            serviceCollection.AddTransient<SendConfirmationOfNewClientKey>();
            serviceCollection.AddTransient<GenerateClientAccessKey>();
            serviceCollection.AddTransient<RegisterUser>();
            serviceCollection.AddTransient<ActivateUser>();
            serviceCollection.AddTransient<AuthenticateUser>();
            serviceCollection.AddTransient<UserAuthenticationByCode>();
            serviceCollection.AddTransient<BaseUserAuthenticator>();
            serviceCollection.AddTransient<UserTwoFactorAuthentication>();
            serviceCollection.AddTransient<UserTwoFactorAuthByEmail>();
            serviceCollection.AddTransient<UserTwoFactorAuthByApp>();

            serviceCollection.AddTransient(provider =>
            {
                return provider.GetGenerateAuthAppQrCoode();
            });

            serviceCollection.AddTransient<ChangeUserPassword>();
            serviceCollection.AddTransient<AddUserPermission>();
            serviceCollection.AddTransient<GetPermissions>();
            serviceCollection.AddTransient<RemoveUserPermission>();
            serviceCollection.AddTransient<RemoveUser>();
            serviceCollection.AddTransient<UpdateUser>();
            serviceCollection.AddTransient<SearchClients>();
            serviceCollection.AddTransient<SearchUsers>();
            serviceCollection.AddTransient<SendEmailToUser>();
            serviceCollection.AddTransient<ChangeUserEmail>();
        }

        internal static void AddDomains(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<ClientDomain>();
            serviceCollection.AddTransient<EmailSender>();
            serviceCollection.AddTransient<PermissionsDomain>();
            serviceCollection.AddTransient<UserDomain>();
            serviceCollection.AddTransient<AuthCodeManager>();
            serviceCollection.AddTransient<AuthApp>();
        }

        internal static void AddServices(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<IEmailService, EmailService>();
            serviceCollection.AddTransient<IAuthService, AuthService>();
        }

        internal static void AddRepositories(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<IClientRepository, ClientRepository>();
            serviceCollection.AddTransient<IUserRepository, UserRepository>();
            serviceCollection.AddTransient<IPermissionRepository, PermissionRepository>();
            serviceCollection.AddTransient<IAuthCodeRepository, AuthCodeRepository>();
        }

        internal static void AddContexts(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddScoped<IMigration>(provider =>
            {
                var connectionString = Environment.GetEnvironmentVariable("ADF_CONEXAO_BANCO");
                if (connectionString is null || connectionString.IsNullOrEmptyOrWhiteSpaces())
                    throw new ApplicationException("A string de conexão com o banco de dados não foi encontrada");

                return new NpsqlMigration(connectionString);
            });

            serviceCollection.AddScoped(provider =>
            {
                return provider.GetClientContext();
            });

            PermissionsContext.ApplyConfigurations();

            serviceCollection.AddScoped(provider =>
            {
                return provider.GetPermissionsContext();
            });

            serviceCollection.AddScoped(provider =>
            {
                return provider.GetAuthCodeContext();
            });
        }

        private static ClientContext GetClientContext(this IServiceProvider serviceProvider)
        {
            var connectionString = Environment.GetEnvironmentVariable("ADF_CONEXAO_BANCO");
            if (connectionString is null || connectionString.IsNullOrEmptyOrWhiteSpaces())
                throw new ApplicationException("A string de conexão com o banco de dados não foi encontrada");

            var httpContextAccessor = serviceProvider.GetRequiredService<IHttpContextAccessor>();
            var httpContext = httpContextAccessor.HttpContext;

            var domainName = httpContext?.Request.Headers["Dominio"].ToString() ?? "public";
            return new ClientContext(connectionString, domainName);
        }

        private static PermissionsContext GetPermissionsContext(this IServiceProvider serviceProvider)
        {
            var connectionString = Environment.GetEnvironmentVariable("ADF_PERMISSOES_CONEXAO_BANCO");
            if (connectionString is null || connectionString.IsNullOrEmptyOrWhiteSpaces())
                throw new ApplicationException("A string de conexão com o banco de dados de permissões não foi encontrada");

            var httpContextAccessor = serviceProvider.GetRequiredService<IHttpContextAccessor>();
            var httpContext = httpContextAccessor.HttpContext;

            var domainName = httpContext?.Request.Headers["Dominio"].ToString() ?? "public";
            return new PermissionsContext(connectionString, domainName);
        }

        private static AuthCodeContext GetAuthCodeContext(this IServiceProvider serviceProvider)
        {
            var connectionString = Environment.GetEnvironmentVariable("ADF_COD_AUTENTICACAO_CONEXAO_BANCO");
            if (connectionString is null || connectionString.IsNullOrEmptyOrWhiteSpaces())
                throw new ApplicationException("A string de conexão com o banco de dados de permissões não foi encontrada");

            var stringEmArray = connectionString.Split(",");
            var host = stringEmArray[0];
            var port = int.Parse(stringEmArray[1]);
            var user = stringEmArray[2];
            var password = stringEmArray[3];

            var httpContextAccessor = serviceProvider.GetRequiredService<IHttpContextAccessor>();
            var httpContext = httpContextAccessor.HttpContext;

            var domainName = httpContext?.Request.Headers["Dominio"].ToString() ?? "public";
            return new AuthCodeContext(host, port, user, password, domainName);
        }

        private static GenerateAuthAppQrCode GetGenerateAuthAppQrCoode(this IServiceProvider serviceProvider)
        {
            var authApp = serviceProvider.GetRequiredService<AuthApp>();
            var userDomain = serviceProvider.GetRequiredService<UserDomain>();
            var emailSender = serviceProvider.GetRequiredService<EmailSender>();
            var notifier = serviceProvider.GetRequiredService<INotifier>();

            var httpContextAccessor = serviceProvider.GetRequiredService<IHttpContextAccessor>();
            var httpContext = httpContextAccessor.HttpContext;
            var baseLinkToGenerateQrCode = $"{httpContext?.Request.Scheme}://{httpContext?.Request.Host}/{_pathToGenerateQrCode}";

            return new GenerateAuthAppQrCode(authApp, userDomain, emailSender, notifier, baseLinkToGenerateQrCode);
        }
    }
}

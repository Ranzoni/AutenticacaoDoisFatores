using AutenticacaoDoisFatores.Dominio.Compartilhados;
using AutenticacaoDoisFatores.Dominio.Dominios;
using AutenticacaoDoisFatores.Dominio.Repositorios;
using AutenticacaoDoisFatores.Dominio.Servicos;
using AutenticacaoDoisFatores.Infra.Utilitarios.Migradores;
using AutenticacaoDoisFatores.Infra.Utilitarios.Migradores.Npgsql;
using AutenticacaoDoisFatores.Infra.Contexto;
using AutenticacaoDoisFatores.Infra.Repositorios;
using AutenticacaoDoisFatores.Infra.Servicos;
using AutenticacaoDoisFatores.Servico.CasosDeUso.Clientes;
using AutenticacaoDoisFatores.Servico.CasosDeUso.Permissoes;
using AutenticacaoDoisFatores.Servico.CasosDeUso.Usuarios;
using AutenticacaoDoisFatores.Servico.CasosDeUso.Usuarios.Autenticadores;
using AutenticacaoDoisFatores.Servico.CasosDeUso.Usuarios.Autenticadores.AutenticacoesDoisFatores;
using Mensageiro;

namespace AutenticacaoDoisFatores
{
    internal static class InjecaoDependencia
    {
        private static readonly string _caminhoParaQrCode = "autenticacao/gerar-qr-code.html";

        internal static void AddCasosDeUso(this IServiceCollection servicos)
        {
            servicos.AddTransient<CreateClient>();
            servicos.AddTransient<ActivateClient>();
            servicos.AddTransient<ResendClientKey>();
            servicos.AddTransient<SendConfirmationOfNewClientKey>();
            servicos.AddTransient<GenerateClientAccessKey>();
            servicos.AddTransient<RegisterUser>();
            servicos.AddTransient<ActivateUser>();
            servicos.AddTransient<AuthenticateUser>();
            servicos.AddTransient<UserAuthenticationByCode>();
            servicos.AddTransient<BaseUserAuthenticator>();
            servicos.AddTransient<UserTwoFactorAuthentication>();
            servicos.AddTransient<UserTwoFactorAuthByEmail>();
            servicos.AddTransient<UserTwoFactorAuthByApp>();

            servicos.AddTransient(provider =>
            {
                return provider.RetornarGerarQrCodeAppAutenticacao();
            });

            servicos.AddTransient<ChangeUserPassword>();
            servicos.AddTransient<AddUserPermission>();
            servicos.AddTransient<GetPermissions>();
            servicos.AddTransient<RemoveUserPermission>();
            servicos.AddTransient<RemoveUser>();
            servicos.AddTransient<UpdateUser>();
            servicos.AddTransient<SearchClients>();
            servicos.AddTransient<SearchUsers>();
            servicos.AddTransient<SendEmailToUser>();
            servicos.AddTransient<ChangeUserEmail>();
        }

        internal static void AddDominios(this IServiceCollection servicos)
        {
            servicos.AddTransient<DominioDeClientes>();
            servicos.AddTransient<EnvioDeEmail>();
            servicos.AddTransient<DominioDePermissoes>();
            servicos.AddTransient<DominioDeUsuarios>();
            servicos.AddTransient<GerenciadorDeCodAutenticacao>();
            servicos.AddTransient<AppAutenticador>();
        }

        internal static void AddServicos(this IServiceCollection servicos)
        {
            servicos.AddTransient<IServicoDeEmail, EmailService>();
            servicos.AddTransient<IServicoDeAutenticador, AuthService>();
        }

        internal static void AddRepositorios(this IServiceCollection servicos)
        {
            servicos.AddTransient<IRepositorioDeClientes, ClientRepository>();
            servicos.AddTransient<IRepositorioDeUsuarios, UserRepository>();
            servicos.AddTransient<IRepositorioDePermissoes, PermissionRepository>();
            servicos.AddTransient<IRepositorioDeCodigoDeAutenticacao, AuthCodeRepository>();
        }

        internal static void AddContextos(this IServiceCollection servicos)
        {
            servicos.AddScoped<IMigration>(provider =>
            {
                var stringDeConexao = Environment.GetEnvironmentVariable("ADF_CONEXAO_BANCO");
                if (stringDeConexao is null || stringDeConexao.EstaVazio())
                    throw new ApplicationException("A string de conexão com o banco de dados não foi encontrada");

                return new MigradorNpsql(stringDeConexao);
            });

            servicos.AddScoped(provider =>
            {
                return provider.RetornarContextoCliente();
            });

            PermissionsContext.ApplyConfigurations();

            servicos.AddScoped(provider =>
            {
                return provider.RetornarContextoPermissoes();
            });

            servicos.AddScoped(provider =>
            {
                return provider.RetornarContextoDeCodigoDeAutenticacao();
            });
        }

        private static ClientContext RetornarContextoCliente(this IServiceProvider serviceProvider)
        {
            var stringDeConexao = Environment.GetEnvironmentVariable("ADF_CONEXAO_BANCO");
            if (stringDeConexao is null || stringDeConexao.EstaVazio())
                throw new ApplicationException("A string de conexão com o banco de dados não foi encontrada");

            var httpContextAccessor = serviceProvider.GetRequiredService<IHttpContextAccessor>();
            var httpContext = httpContextAccessor.HttpContext;

            var nomeDominio = httpContext?.Request.Headers["Dominio"].ToString() ?? "public";
            return new ClientContext(stringDeConexao, nomeDominio);
        }

        private static PermissionsContext RetornarContextoPermissoes(this IServiceProvider serviceProvider)
        {
            var stringDeConexao = Environment.GetEnvironmentVariable("ADF_PERMISSOES_CONEXAO_BANCO");
            if (stringDeConexao is null || stringDeConexao.EstaVazio())
                throw new ApplicationException("A string de conexão com o banco de dados de permissões não foi encontrada");

            var httpContextAccessor = serviceProvider.GetRequiredService<IHttpContextAccessor>();
            var httpContext = httpContextAccessor.HttpContext;

            var nomeDominio = httpContext?.Request.Headers["Dominio"].ToString() ?? "public";
            return new PermissionsContext(stringDeConexao, nomeDominio);
        }

        private static AuthCodeContext RetornarContextoDeCodigoDeAutenticacao(this IServiceProvider serviceProvider)
        {
            var stringDeConexao = Environment.GetEnvironmentVariable("ADF_COD_AUTENTICACAO_CONEXAO_BANCO");
            if (stringDeConexao is null || stringDeConexao.EstaVazio())
                throw new ApplicationException("A string de conexão com o banco de dados de permissões não foi encontrada");

            var stringEmArray = stringDeConexao.Split(",");
            var host = stringEmArray[0];
            var porta = int.Parse(stringEmArray[1]);
            var usuario = stringEmArray[2];
            var senha = stringEmArray[3];

            var httpContextAccessor = serviceProvider.GetRequiredService<IHttpContextAccessor>();
            var httpContext = httpContextAccessor.HttpContext;

            var nomeDominio = httpContext?.Request.Headers["Dominio"].ToString() ?? "public";
            return new AuthCodeContext(host, porta, usuario, senha, nomeDominio);
        }

        private static GenerateAuthAppQrCode RetornarGerarQrCodeAppAutenticacao(this IServiceProvider serviceProvider)
        {
            var dominioAppAutenticador = serviceProvider.GetRequiredService<AppAutenticador>();
            var dominioDeUsuarios = serviceProvider.GetRequiredService<DominioDeUsuarios>();
            var envioDeEmail = serviceProvider.GetRequiredService<EnvioDeEmail>();
            var notificador = serviceProvider.GetRequiredService<INotificador>();

            var httpContextAccessor = serviceProvider.GetRequiredService<IHttpContextAccessor>();
            var httpContext = httpContextAccessor.HttpContext;
            var linkBaseParaQrCode = $"{httpContext?.Request.Scheme}://{httpContext?.Request.Host}/{_caminhoParaQrCode}";

            return new GerarQrCodeAppAutenticacao(dominioAppAutenticador, dominioDeUsuarios, envioDeEmail, notificador, linkBaseParaQrCode);
        }
    }
}

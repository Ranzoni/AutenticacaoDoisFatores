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
            servicos.AddTransient<CriarCliente>();
            servicos.AddTransient<AtivarCliente>();
            servicos.AddTransient<ReenviarChaveCliente>();
            servicos.AddTransient<EnviarConfirmacaoNovaChaveCliente>();
            servicos.AddTransient<GerarNovaChaveAcessoCliente>();
            servicos.AddTransient<CriarUsuario>();
            servicos.AddTransient<AtivarUsuario>();
            servicos.AddTransient<AutenticarUsuario>();
            servicos.AddTransient<AutenticarUsuarioPorCodigo>();
            servicos.AddTransient<AutenticadorUsuarioPadrao>();
            servicos.AddTransient<AutenticadorUsuarioEmDoisFatores>();
            servicos.AddTransient<AutenticadorUsuarioEmDoisFatoresPorEmail>();

            //servicos.AddTransient<AutenticadorUsuarioEmDoisFatoresPorApp>();
            servicos.AddTransient(provider =>
            {
                return provider.RetornarAutenticadorUsuarioEmDoisFatoresPorApp();
            });

            servicos.AddTransient<GerarNovaSenhaUsuario>();
            servicos.AddTransient<IncluirPermissoesParaUsuario>();
            servicos.AddTransient<RetornarPermissoes>();
            servicos.AddTransient<RemoverPermissoesParaUsuario>();
            servicos.AddTransient<ExcluirUsuario>();
            servicos.AddTransient<AlterarUsuario>();
            servicos.AddTransient<BuscarClientes>();
            servicos.AddTransient<BuscarUsuarios>();
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
            servicos.AddTransient<IServicoDeEmail, ServicoDeEmail>();
            servicos.AddTransient<IServicoDeAutenticador, ServicoDeAutenticador>();
        }

        internal static void AddRepositorios(this IServiceCollection servicos)
        {
            servicos.AddTransient<IRepositorioDeClientes, RepositorioDeClientes>();
            servicos.AddTransient<IRepositorioDeUsuarios, RepositorioDeUsuarios>();
            servicos.AddTransient<IRepositorioDePermissoes, RepositorioDePermissoes>();
            servicos.AddTransient<IRepositorioDeCodigoDeAutenticacao, RepositorioDeCodigoDeAutenticacao>();
        }

        internal static void AddContextos(this IServiceCollection servicos)
        {
            servicos.AddScoped<IMigrador>(provider =>
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

            ContextoPermissoes.AplicarConfiguracoes();

            servicos.AddScoped(provider =>
            {
                return provider.RetornarContextoPermissoes();
            });

            servicos.AddScoped(provider =>
            {
                return provider.RetornarContextoDeCodigoDeAutenticacao();
            });
        }

        private static ContextoCliente RetornarContextoCliente(this IServiceProvider serviceProvider)
        {
            var stringDeConexao = Environment.GetEnvironmentVariable("ADF_CONEXAO_BANCO");
            if (stringDeConexao is null || stringDeConexao.EstaVazio())
                throw new ApplicationException("A string de conexão com o banco de dados não foi encontrada");

            var httpContextAccessor = serviceProvider.GetRequiredService<IHttpContextAccessor>();
            var httpContext = httpContextAccessor.HttpContext;

            var nomeDominio = httpContext?.Request.Headers["Dominio"].ToString() ?? "public";
            return new ContextoCliente(stringDeConexao, nomeDominio);
        }

        private static ContextoPermissoes RetornarContextoPermissoes(this IServiceProvider serviceProvider)
        {
            var stringDeConexao = Environment.GetEnvironmentVariable("ADF_PERMISSOES_CONEXAO_BANCO");
            if (stringDeConexao is null || stringDeConexao.EstaVazio())
                throw new ApplicationException("A string de conexão com o banco de dados de permissões não foi encontrada");

            var httpContextAccessor = serviceProvider.GetRequiredService<IHttpContextAccessor>();
            var httpContext = httpContextAccessor.HttpContext;

            var nomeDominio = httpContext?.Request.Headers["Dominio"].ToString() ?? "public";
            return new ContextoPermissoes(stringDeConexao, nomeDominio);
        }

        private static ContextoDeCodigoDeAutenticacao RetornarContextoDeCodigoDeAutenticacao(this IServiceProvider serviceProvider)
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
            return new ContextoDeCodigoDeAutenticacao(host, porta, usuario, senha, nomeDominio);
        }

        private static AutenticadorUsuarioEmDoisFatoresPorApp RetornarAutenticadorUsuarioEmDoisFatoresPorApp(this IServiceProvider serviceProvider)
        {
            var dominioAppAutenticador = serviceProvider.GetRequiredService<AppAutenticador>();
            var envioDeEmail = serviceProvider.GetRequiredService<EnvioDeEmail>();
            var notificador = serviceProvider.GetRequiredService<INotificador>();

            var httpContextAccessor = serviceProvider.GetRequiredService<IHttpContextAccessor>();
            var httpContext = httpContextAccessor.HttpContext;
            var linkBaseParaQrCode = $"{httpContext?.Request.Scheme}://{httpContext?.Request.Host}/{_caminhoParaQrCode}";

            return new AutenticadorUsuarioEmDoisFatoresPorApp(dominioAppAutenticador, envioDeEmail, notificador, linkBaseParaQrCode);
        }
    }
}

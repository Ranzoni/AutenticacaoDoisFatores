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

namespace AutenticacaoDoisFatores
{
    internal static class InjecaoDependencia
    {
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
            servicos.AddTransient<GerarNovaSenhaUsuario>();
            servicos.AddTransient<IncluirPermissoesParaUsuario>();
            servicos.AddTransient<RetornarPermissoes>();
            servicos.AddTransient<RemoverPermissoesParaUsuario>();
            servicos.AddTransient<ExcluirUsuario>();
            servicos.AddTransient<AlterarUsuario>();
        }

        internal static void AddDominios(this IServiceCollection servicos)
        {
            servicos.AddTransient<DominioDeClientes>();
            servicos.AddTransient<EnvioDeEmail>();
            servicos.AddTransient<DominioDePermissoes>();
            servicos.AddTransient<DominioDeUsuarios>();
        }

        internal static void AddServicos(this IServiceCollection servicos)
        {
            servicos.AddTransient<IServicoDeEmail, ServicoDeEmail>();
        }

        internal static void AddRepositorios(this IServiceCollection servicos)
        {
            servicos.AddTransient<IRepositorioDeClientes, RepositorioDeClientes>();
            servicos.AddTransient<IRepositorioDeUsuarios, RepositorioDeUsuarios>();
            servicos.AddTransient<IRepositorioDePermissoes, RepositorioDePermissoes>();
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
                var stringDeConexao = Environment.GetEnvironmentVariable("ADF_PERMISSOES_CONEXAO_BANCO");
                if (stringDeConexao is null || stringDeConexao.EstaVazio())
                    throw new ApplicationException("A string de conexão com o banco de dados de permissões não foi encontrada");

                var httpContextAccessor = provider.GetRequiredService<IHttpContextAccessor>();
                var httpContext = httpContextAccessor.HttpContext;

                var nomeDominio = httpContext?.Request.Headers["Dominio"].ToString() ?? "public";
                return new ContextoPermissoes(stringDeConexao, nomeDominio);
            });
        }

        internal static ContextoCliente RetornarContextoCliente(this IServiceProvider serviceProvider)
        {
            var stringDeConexao = Environment.GetEnvironmentVariable("ADF_CONEXAO_BANCO");
            if (stringDeConexao is null || stringDeConexao.EstaVazio())
                throw new ApplicationException("A string de conexão com o banco de dados não foi encontrada");

            var httpContextAccessor = serviceProvider.GetRequiredService<IHttpContextAccessor>();
            var httpContext = httpContextAccessor.HttpContext;

            var nomeDominio = httpContext?.Request.Headers["Dominio"].ToString() ?? "public";
            return new ContextoCliente(stringDeConexao, nomeDominio);
        }

        internal static void RetornarRepositorioUsuario(this IServiceProvider serviceProvider)
        {
            var contextoCliente = serviceProvider.RetornarContextoCliente();

        }
    }
}

using AutenticacaoDoisFatores.Dominio.Compartilhados;
using AutenticacaoDoisFatores.Dominio.Dominios;
using AutenticacaoDoisFatores.Dominio.Repositorios;
using AutenticacaoDoisFatores.Dominio.Servicos;
using AutenticacaoDoisFatores.Infra.Compartilhados.Migradores;
using AutenticacaoDoisFatores.Infra.Compartilhados.Migradores.Npgsql;
using AutenticacaoDoisFatores.Infra.Contexto;
using AutenticacaoDoisFatores.Infra.Repositorios;
using AutenticacaoDoisFatores.Infra.Servicos;
using AutenticacaoDoisFatores.Servico.CasosDeUso.Clientes;
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
        }

        internal static void AddDominios(this IServiceCollection servicos)
        {
            servicos.AddTransient<DominioDeClientes>();
            servicos.AddTransient<EnvioDeEmail>();
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
                var stringDeConexao = Environment.GetEnvironmentVariable("ADF_CONEXAO_BANCO");
                if (stringDeConexao is null || stringDeConexao.EstaVazio())
                    throw new ApplicationException("A string de conexão com o banco de dados não foi encontrada");

                var httpContextAccessor = provider.GetRequiredService<IHttpContextAccessor>();
                var httpContext = httpContextAccessor.HttpContext;

                var nomeDominio = httpContext?.Request.Headers["Dominio"].ToString() ?? "";
                if (nomeDominio.EstaVazio())
                    throw new ApplicationException("O Domínio do cliente não foi encontrado na requisição");

                return new ContextoCliente(stringDeConexao, nomeDominio);
            });
        }
    }
}

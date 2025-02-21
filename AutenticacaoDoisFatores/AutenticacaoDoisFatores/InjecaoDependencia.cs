using AutenticacaoDoisFatores.Dominio.Compartilhados;
using AutenticacaoDoisFatores.Dominio.Dominios;
using AutenticacaoDoisFatores.Dominio.Repositorios;
using AutenticacaoDoisFatores.Dominio.Servicos;
using AutenticacaoDoisFatores.Infra.Compartilhados.Migradores;
using AutenticacaoDoisFatores.Infra.Compartilhados.Migradores.Npgsql;
using AutenticacaoDoisFatores.Infra.Repositorios;
using AutenticacaoDoisFatores.Infra.Servicos;
using AutenticacaoDoisFatores.Servico.CasosDeUso.Clientes;

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
        }

        internal static void AddDominios(this IServiceCollection servicos)
        {
            servicos.AddTransient<DominioDeClientes>();
            servicos.AddTransient<EnvioDeEmail>();
        }

        internal static void AddServicos(this IServiceCollection servicos)
        {
            servicos.AddTransient<IServicoDeEmail, ServicoDeEmail>();
        }

        internal static void AddRepositorios(this IServiceCollection servicos)
        {
            servicos.AddTransient<IRepositorioDeClientes, RepositorioDeClientes>();
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
        }
    }
}

using AutenticacaoDoisFatores.Dominio.Dominios;
using AutenticacaoDoisFatores.Dominio.Repositorios;
using AutenticacaoDoisFatores.Infra.Repositorios;
using AutenticacaoDoisFatores.Servico.CasosDeUso;

namespace AutenticacaoDoisFatores
{
    internal static class InjecaoDependencia
    {
        internal static void AddCasosDeUso(this IServiceCollection services)
        {
            services.AddTransient<CriarCliente>();
        }

        internal static void AddDominios(this IServiceCollection services)
        {
            services.AddTransient<DominioDeClientes>();
        }

        internal static void AddRepositorios(this IServiceCollection services)
        {
            services.AddTransient<IRepositorioDeClientes, RepositorioDeClientes>();
        }
    }
}

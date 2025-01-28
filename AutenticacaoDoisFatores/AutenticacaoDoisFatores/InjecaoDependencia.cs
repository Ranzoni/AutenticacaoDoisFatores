using AutenticacaoDoisFatores.Dominio.Dominios;
using AutenticacaoDoisFatores.Dominio.Repositorios;
using AutenticacaoDoisFatores.Dominio.Servicos;
using AutenticacaoDoisFatores.Infra.Repositorios;
using AutenticacaoDoisFatores.Infra.Servicos;
using AutenticacaoDoisFatores.Servico.CasosDeUso.Clientes;

namespace AutenticacaoDoisFatores
{
    internal static class InjecaoDependencia
    {
        internal static void AddCasosDeUso(this IServiceCollection services)
        {
            services.AddTransient<CriarCliente>();
            services.AddTransient<AtivarCliente>();
            services.AddTransient<ReenviarChaveCliente>();
            services.AddTransient<EnviarConfirmacaoNovaChaveCliente>();
        }

        internal static void AddDominios(this IServiceCollection services)
        {
            services.AddTransient<DominioDeClientes>();
            services.AddTransient<EnvioDeEmail>();
        }

        internal static void AddServicos(this IServiceCollection services)
        {
            services.AddTransient<IServicoDeEmail, ServicoDeEmail>();
        }

        internal static void AddRepositorios(this IServiceCollection services)
        {
            services.AddTransient<IRepositorioDeClientes, RepositorioDeClientes>();
        }
    }
}

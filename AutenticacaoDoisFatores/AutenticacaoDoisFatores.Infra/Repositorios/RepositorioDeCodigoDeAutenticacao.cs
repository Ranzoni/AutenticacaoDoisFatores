using AutenticacaoDoisFatores.Dominio.Repositorios;
using AutenticacaoDoisFatores.Infra.Contexto;

namespace AutenticacaoDoisFatores.Infra.Repositorios
{
    public class RepositorioDeCodigoDeAutenticacao(ContextoDeCodigoDeAutenticacao contexto) : IRepositorioDeCodigoDeAutenticacao
    {
        private readonly ContextoDeCodigoDeAutenticacao _contexto = contexto;

        public async Task SalvarAsync(Guid idUsuario, string codigo)
        {
            await _contexto.SalvarAsync(idUsuario.ToString(), codigo);
        }
    }
}

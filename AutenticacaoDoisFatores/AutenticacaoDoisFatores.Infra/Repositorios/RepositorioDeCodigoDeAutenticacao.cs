using AutenticacaoDoisFatores.Dominio.Repositorios;
using AutenticacaoDoisFatores.Infra.Contexto;

namespace AutenticacaoDoisFatores.Infra.Repositorios
{
    public class RepositorioDeCodigoDeAutenticacao(ContextoDeCodigoDeAutenticacao contexto) : IRepositorioDeCodigoDeAutenticacao
    {
        private readonly ContextoDeCodigoDeAutenticacao _contexto = contexto;

        public async Task<string?> BuscarCodigoAsync(Guid idUsuario)
        {
            return await _contexto.BuscarAsync(ChaveIdUsuario(idUsuario));
        }

        public async Task SalvarAsync(Guid idUsuario, string codigo)
        {
            await _contexto.SalvarAsync(ChaveIdUsuario(idUsuario), codigo);
        }

        private static string ChaveIdUsuario(Guid idUsuario)
        {
            return $"idusuario_{idUsuario}";
        }
    }
}

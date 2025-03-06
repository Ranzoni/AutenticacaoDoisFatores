using AutenticacaoDoisFatores.Dominio.Compartilhados.Permissoes;
using AutenticacaoDoisFatores.Dominio.Repositorios;

namespace AutenticacaoDoisFatores.Dominio.Dominios
{
    public class DominioDePermissoes(IRepositorioDePermissoes repositorio)
    {
        private readonly IRepositorioDePermissoes _repositorio = repositorio;

        public async Task AdicionarAsync(Guid idUsuario, IEnumerable<TipoDePermissao> permissoes)
        {
            await _repositorio.AdicionarAsync(idUsuario, permissoes);
        }

        public async Task EditarAsync(Guid idUsuario, IEnumerable<TipoDePermissao> permissoes)
        {
            await _repositorio.EditarAsync(idUsuario, permissoes);
        }

        public async Task<IEnumerable<TipoDePermissao>> RetornarPermissoesAsync(Guid idUsuario)
        {
            return await _repositorio.RetornarPermissoesAsync(idUsuario);
        }

        public static IEnumerable<TipoDePermissao> RetornarTodasPermissoes()
        {
            return Enum.GetValues<TipoDePermissao>();
        }
    }
}

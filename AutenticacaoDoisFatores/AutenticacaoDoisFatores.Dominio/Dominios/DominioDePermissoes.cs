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

        public async Task<string?> BuscarAsync(Guid idUsuario, TipoDePermissao tipoDePermissao)
        {
            return await _repositorio.BuscarAsync(idUsuario, tipoDePermissao);
        }
    }
}

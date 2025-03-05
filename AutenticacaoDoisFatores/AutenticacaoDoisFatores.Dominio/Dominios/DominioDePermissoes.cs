using AutenticacaoDoisFatores.Dominio.Compartilhados.Permissoes;
using AutenticacaoDoisFatores.Dominio.Repositorios;

namespace AutenticacaoDoisFatores.Dominio.Dominios
{
    public class DominioDePermissoes(IRepositorioDePermissoes repositorio, IRepositorioDeUsuarios repositorioDeUsuarios)
    {
        private readonly IRepositorioDePermissoes _repositorio = repositorio;
        private readonly IRepositorioDeUsuarios _repositorioDeUsuarios = repositorioDeUsuarios;

        public async Task AdicionarAsync(Guid idUsuario, IEnumerable<TipoDePermissao> permissoes)
        {
            await _repositorio.AdicionarAsync(idUsuario, permissoes);
        }

        public async Task<IEnumerable<TipoDePermissao>> RetornarPermissoes(Guid idUsuario)
        {
            if (await _repositorioDeUsuarios.EhAdm(idUsuario))
                return RetornarTodasPermissoes();

            return await _repositorio.RetornarPermissoes(idUsuario);
        }

        public static IEnumerable<TipoDePermissao> RetornarTodasPermissoes()
        {
            return Enum.GetValues<TipoDePermissao>();
        }
    }
}

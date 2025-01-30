using AutenticacaoDoisFatores.Dominio.Entidades;
using AutenticacaoDoisFatores.Dominio.Repositorios;

namespace AutenticacaoDoisFatores.Dominio.Dominios
{
    public class DominioDeUsuario(IRepositorioDeUsuarios repositorio)
    {
        private readonly IRepositorioDeUsuarios _repositorio = repositorio;

        #region Escrita

        public async Task CriarUsuarioAsync(Usuario usuario)
        {
            _repositorio.Adicionar(usuario);
            await _repositorio.SalvarAlteracoesAsync();
        }

        #endregion
    }
}

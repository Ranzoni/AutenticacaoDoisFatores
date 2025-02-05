using AutenticacaoDoisFatores.Dominio.Entidades;
using AutenticacaoDoisFatores.Dominio.Excecoes;
using AutenticacaoDoisFatores.Dominio.Repositorios;

namespace AutenticacaoDoisFatores.Dominio.Dominios
{
    public partial class DominioDeUsuarios(IRepositorioDeUsuarios repositorio)
    {
        private readonly IRepositorioDeUsuarios _repositorio = repositorio;

        #region Escrita

        public async Task CriarUsuarioAsync(Usuario usuario)
        {
            await ValidarCriacaoUsuarioAsync(usuario);

            _repositorio.Adicionar(usuario);
            await _repositorio.SalvarAlteracoesAsync();
        }

        #endregion
    }

    #region Validador

    public partial class DominioDeUsuarios
    {
        public async Task ValidarCriacaoUsuarioAsync(Usuario usuario)
        {
            var existeNomeUsuario = await _repositorio.ExisteNomeUsuarioAsync(usuario.NomeUsuario);
            if (existeNomeUsuario)
                ExcecoesUsuario.NomeUsuarioJaCadastrado();

            var existeEmail = await _repositorio.ExisteEmailAsync(usuario.Email);
            if (existeEmail)
                ExcecoesUsuario.EmailJaCadastrado();
        }
    }

    #endregion
}

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
            await ValidarUsuarioAsync(usuario);

            _repositorio.Adicionar(usuario);
            await _repositorio.SalvarAlteracoesAsync();
        }

        public async Task<Usuario> AlterarUsuarioAsync(Usuario usuario)
        {
            await ValidarUsuarioAsync(usuario);

            _repositorio.Editar(usuario);
            await _repositorio.SalvarAlteracoesAsync();

            return usuario;
        }

        #endregion

        #region Leitura

        public async Task<Usuario?> BuscarUnicoAsync(Guid id)
        {
            return await _repositorio.BuscarUnicoAsync(id);
        }

        public async Task<Usuario?> BuscarPorNomeUsuarioAsync(string nomeUsuario)
        {
            return await _repositorio.BuscarPorNomeUsuarioAsync(nomeUsuario);
        }

        public async Task<Usuario?> BuscarPorEmailAsync(string email)
        {
            return await _repositorio.BuscarPorEmailAsync(email);
        }

        public async Task<bool> ExisteNomeUsuarioAsync(string nomeUsuario)
        {
            return await _repositorio.ExisteNomeUsuarioAsync(nomeUsuario);
        }

        public async Task<bool> ExisteEmailAsync(string email)
        {
            return await _repositorio.ExisteEmailAsync(email);
        }

        #endregion
    }

    #region Validador

    public partial class DominioDeUsuarios
    {
        public async Task ValidarUsuarioAsync(Usuario usuario)
        {
            var existeNomeUsuario = await _repositorio.ExisteNomeUsuarioAsync(usuario.NomeUsuario, usuario.Id);
            if (existeNomeUsuario)
                ExcecoesUsuario.NomeUsuarioJaCadastrado();

            var existeEmail = await _repositorio.ExisteEmailAsync(usuario.Email, usuario.Id);
            if (existeEmail)
                ExcecoesUsuario.EmailJaCadastrado();
        }
    }

    #endregion
}

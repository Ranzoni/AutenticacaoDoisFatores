using AutenticacaoDoisFatores.Dominio.Entidades;
using AutenticacaoDoisFatores.Dominio.Excecoes;
using AutenticacaoDoisFatores.Dominio.Repositorios;

namespace AutenticacaoDoisFatores.Dominio.Dominios
{
    public partial class DominioDeUsuarios(IRepositorioDeUsuarios repositorio)
    {
        private readonly IRepositorioDeUsuarios _repositorio = repositorio;

        #region Escrita

        public async Task CriarAsync(Usuario usuario)
        {
            await ValidarUsuarioAsync(usuario);

            _repositorio.Adicionar(usuario);
            await _repositorio.SalvarAlteracoesAsync();
        }

        public async Task CriarUsuarioComDominioAsync(Usuario usuario, string dominio)
        {
            await ValidarUsuarioComDominioAsync(usuario, dominio);

            _repositorio.Adicionar(usuario, dominio);
            await _repositorio.SalvarAlteracoesAsync();
        }

        public async Task<Usuario> AlterarAsync(Usuario usuario)
        {
            await ValidarAlteracaoUsuarioAsync(usuario);

            _repositorio.Editar(usuario);
            await _repositorio.SalvarAlteracoesAsync();

            return usuario;
        }

        public async Task ExcluirUsuarioAsync(Guid id)
        {
            await ValidarExclusaoUsuarioAsync(id);

            _repositorio.Excluir(id);
            await _repositorio.SalvarAlteracoesAsync();
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

        public async Task<bool> EhAdmAsync(Guid idUsuario)
        {
            return await _repositorio.EhAdmAsync(idUsuario);
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

        public async Task ValidarAlteracaoUsuarioAsync(Usuario usuario)
        {
            await ValidarUsuarioAsync(usuario);
            await ValidarSeUsuarioExisteAsync(usuario.Id);
        }

        public async Task ValidarUsuarioComDominioAsync(Usuario usuario, string dominio)
        {
            var existeNomeUsuario = await _repositorio.ExisteNomeUsuarioAsync(usuario.NomeUsuario, dominio);
            if (existeNomeUsuario)
                ExcecoesUsuario.NomeUsuarioJaCadastrado();

            var existeEmail = await _repositorio.ExisteEmailAsync(usuario.Email, dominio);
            if (existeEmail)
                ExcecoesUsuario.EmailJaCadastrado();
        }

        public async Task ValidarExclusaoUsuarioAsync(Guid id)
        {
            await ValidarSeUsuarioExisteAsync(id);
        }

        private async Task ValidarSeUsuarioExisteAsync(Guid id)
        {
            var usuarioExiste = await _repositorio.BuscarUnicoAsync(id) is not null;
            if (!usuarioExiste)
                ExcecoesUsuario.UsuarioNaoEncontrado();
        }
    }

    #endregion
}

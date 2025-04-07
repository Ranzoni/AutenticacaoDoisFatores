using AutenticacaoDoisFatores.Dominio.Entidades;
using AutenticacaoDoisFatores.Dominio.Excecoes;
using AutenticacaoDoisFatores.Dominio.Filtros;
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
            var usuario = await _repositorio.BuscarUnicoAsync(id);

            ValidarExclusaoUsuario(usuario);

            _repositorio.Excluir(usuario);
            await _repositorio.SalvarAlteracoesAsync();
        }

        #endregion

        #region Leitura

        public async Task<Usuario?> BuscarUnicoAsync(Guid id)
        {
            return await _repositorio.BuscarUnicoAsync(id);
        }

        public async Task<IEnumerable<Usuario>> BuscarVariosAsync(FiltroPadrao filtro)
        {
            return await _repositorio.BuscarVariosAsync(filtro);
        }

        public async Task<Usuario?> BuscarPorNomeUsuarioAsync(string nomeUsuario)
        {
            return await _repositorio.BuscarPorNomeUsuarioAsync(nomeUsuario);
        }

        public async Task<Usuario?> BuscarPorEmailAsync(string email)
        {
            return await _repositorio.BuscarPorEmailAsync(email);
        }

        public async Task<bool> ExisteNomeUsuarioAsync(string nomeUsuario, Guid? id = null)
        {
            return await _repositorio.ExisteNomeUsuarioAsync(nomeUsuario, id);
        }

        public async Task<bool> ExisteEmailAsync(string email, Guid? id = null)
        {
            return await _repositorio.ExisteEmailAsync(email, id);
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
            ValidarSeUsuarioExiste(usuario);
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

        public static void ValidarExclusaoUsuario(Usuario? usuario)
        {
            ValidarSeUsuarioExiste(usuario);
        }

        private static void ValidarSeUsuarioExiste(Usuario? usuario)
        {
            if (usuario is null)
                ExcecoesUsuario.UsuarioNaoEncontrado();
        }
    }

    #endregion
}

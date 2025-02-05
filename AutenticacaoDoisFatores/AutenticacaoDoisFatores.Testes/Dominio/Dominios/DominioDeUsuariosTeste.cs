using AutenticacaoDoisFatores.Dominio.Compartilhados;
using AutenticacaoDoisFatores.Dominio.Compartilhados.Mensagens;
using AutenticacaoDoisFatores.Dominio.Dominios;
using AutenticacaoDoisFatores.Dominio.Entidades;
using AutenticacaoDoisFatores.Dominio.Excecoes;
using AutenticacaoDoisFatores.Dominio.Repositorios;
using AutenticacaoDoisFatores.Testes.Compartilhados;
using Moq;
using Moq.AutoMock;

namespace AutenticacaoDoisFatores.Testes.Dominio.Dominios
{
    public class DominioDeUsuariosTeste
    {
        private readonly AutoMocker _mocker = new();

        [Fact]
        internal async Task DeveCriarUsuario()
        {
            #region Preparação do teste

            var dominio = _mocker.CreateInstance<DominioDeUsuarios>();

            var usuarioParaCriar = ConstrutorDeUsuariosTeste
                .RetornarConstrutorDeNovo()
                .ConstruirNovo();

            #endregion

            await dominio.CriarUsuarioAsync(usuarioParaCriar);

            #region Verificação do teste

            _mocker.Verify<IRepositorioDeUsuarios>(r => r.Adicionar(usuarioParaCriar), Times.Once);
            _mocker.Verify<IRepositorioDeUsuarios>(r => r.SalvarAlteracoesAsync(), Times.Once);

            #endregion
        }

        [Fact]
        internal async Task DeveRetornarExcecaoAoCriarUsuarioComNomeUsuarioJaExistente()
        {
            #region Preparação do teste

            var dominio = _mocker.CreateInstance<DominioDeUsuarios>();

            var usuarioParaCriar = ConstrutorDeUsuariosTeste
                .RetornarConstrutorDeNovo()
                .ConstruirNovo();

            _mocker.GetMock<IRepositorioDeUsuarios>().Setup(r => r.ExisteNomeUsuarioAsync(usuarioParaCriar.NomeUsuario)).ReturnsAsync(true);

            #endregion

            var excecao = await Assert.ThrowsAsync<ExcecoesUsuario>(() => dominio.CriarUsuarioAsync(usuarioParaCriar));

            #region Verificação do teste

            Assert.Equal(MensagensValidacaoUsuario.NomeUsuarioJaCadastrado.Descricao(), excecao.Message);
            _mocker.Verify<IRepositorioDeUsuarios>(r => r.Adicionar(It.IsAny<Usuario>()), Times.Never);
            _mocker.Verify<IRepositorioDeUsuarios>(r => r.SalvarAlteracoesAsync(), Times.Never);

            #endregion
        }

        [Fact]
        internal async Task DeveRetornarExcecaoAoCriarUsuarioComEmailJaExistente()
        {
            #region Preparação do teste

            var dominio = _mocker.CreateInstance<DominioDeUsuarios>();

            var usuarioParaCriar = ConstrutorDeUsuariosTeste
                .RetornarConstrutorDeNovo()
                .ConstruirNovo();

            _mocker.GetMock<IRepositorioDeUsuarios>().Setup(r => r.ExisteEmailAsync(usuarioParaCriar.Email)).ReturnsAsync(true);

            #endregion

            var excecao = await Assert.ThrowsAsync<ExcecoesUsuario>(() => dominio.CriarUsuarioAsync(usuarioParaCriar));

            #region Verificação do teste

            Assert.Equal(MensagensValidacaoUsuario.EmailJaCadastrado.Descricao(), excecao.Message);
            _mocker.Verify<IRepositorioDeUsuarios>(r => r.Adicionar(It.IsAny<Usuario>()), Times.Never);
            _mocker.Verify<IRepositorioDeUsuarios>(r => r.SalvarAlteracoesAsync(), Times.Never);

            #endregion
        }
    }
}

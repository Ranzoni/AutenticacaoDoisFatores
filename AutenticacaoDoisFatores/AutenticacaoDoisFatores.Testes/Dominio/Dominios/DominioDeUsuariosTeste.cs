using AutenticacaoDoisFatores.Dominio.Compartilhados;
using AutenticacaoDoisFatores.Dominio.Compartilhados.Mensagens;
using AutenticacaoDoisFatores.Dominio.Dominios;
using AutenticacaoDoisFatores.Dominio.Entidades;
using AutenticacaoDoisFatores.Dominio.Excecoes;
using AutenticacaoDoisFatores.Dominio.Repositorios;
using AutenticacaoDoisFatores.Testes.Compartilhados;
using Bogus;
using Moq;
using Moq.AutoMock;

namespace AutenticacaoDoisFatores.Testes.Dominio.Dominios
{
    public class DominioDeUsuariosTeste
    {
        private readonly AutoMocker _mocker = new();
        private readonly Faker _faker = new();

        [Fact]
        internal async Task DeveCriarUsuario()
        {
            #region Preparação do teste

            var dominio = _mocker.CreateInstance<DominioDeUsuarios>();

            var usuarioParaCriar = ConstrutorDeUsuariosTeste
                .RetornarConstrutor()
                .ConstruirNovo();

            #endregion

            await dominio.CriarAsync(usuarioParaCriar);

            #region Verificação do teste

            _mocker.Verify<IRepositorioDeUsuarios>(r => r.Adicionar(usuarioParaCriar), Times.Once);
            _mocker.Verify<IRepositorioDeUsuarios>(r => r.SalvarAlteracoesAsync(), Times.Once);

            #endregion
        }

        [Fact]
        internal async Task DeveCriarUsuarioComDominio()
        {
            #region Preparação do teste

            var dominio = _mocker.CreateInstance<DominioDeUsuarios>();

            var usuarioParaCriar = ConstrutorDeUsuariosTeste
                .RetornarConstrutor()
                .ConstruirNovo();

            var nomeDominio = _faker.Internet.DomainName();

            #endregion

            await dominio.CriarUsuarioComDominioAsync(usuarioParaCriar, nomeDominio);

            #region Verificação do teste

            _mocker.Verify<IRepositorioDeUsuarios>(r => r.Adicionar(usuarioParaCriar, nomeDominio), Times.Once);
            _mocker.Verify<IRepositorioDeUsuarios>(r => r.SalvarAlteracoesAsync(), Times.Once);

            #endregion
        }

        [Fact]
        internal async Task DeveRetornarExcecaoAoCriarUsuarioComNomeUsuarioJaExistente()
        {
            #region Preparação do teste

            var dominio = _mocker.CreateInstance<DominioDeUsuarios>();

            var usuarioParaCriar = ConstrutorDeUsuariosTeste
                .RetornarConstrutor()
                .ConstruirNovo();

            _mocker.GetMock<IRepositorioDeUsuarios>().Setup(r => r.ExisteNomeUsuarioAsync(usuarioParaCriar.NomeUsuario, It.IsAny<Guid>())).ReturnsAsync(true);

            #endregion

            var excecao = await Assert.ThrowsAsync<ExcecoesUsuario>(() => dominio.CriarAsync(usuarioParaCriar));

            #region Verificação do teste

            Assert.Equal(MensagensValidacaoUsuario.NomeUsuarioJaCadastrado.Descricao(), excecao.Message);
            _mocker.Verify<IRepositorioDeUsuarios>(r => r.Adicionar(It.IsAny<Usuario>()), Times.Never);
            _mocker.Verify<IRepositorioDeUsuarios>(r => r.SalvarAlteracoesAsync(), Times.Never);

            #endregion
        }

        [Fact]
        internal async Task DeveRetornarExcecaoAoCriarUsuarioComDominioComNomeUsuarioJaExistente()
        {
            #region Preparação do teste

            var dominio = _mocker.CreateInstance<DominioDeUsuarios>();

            var usuarioParaCriar = ConstrutorDeUsuariosTeste
                .RetornarConstrutor()
                .ConstruirNovo();

            var nomeDominio = _faker.Internet.DomainName();

            _mocker.GetMock<IRepositorioDeUsuarios>().Setup(r => r.ExisteNomeUsuarioAsync(usuarioParaCriar.NomeUsuario, nomeDominio)).ReturnsAsync(true);

            #endregion

            var excecao = await Assert.ThrowsAsync<ExcecoesUsuario>(() => dominio.CriarUsuarioComDominioAsync(usuarioParaCriar, nomeDominio));

            #region Verificação do teste

            Assert.Equal(MensagensValidacaoUsuario.NomeUsuarioJaCadastrado.Descricao(), excecao.Message);
            _mocker.Verify<IRepositorioDeUsuarios>(r => r.Adicionar(It.IsAny<Usuario>(), It.IsAny<string>()), Times.Never);
            _mocker.Verify<IRepositorioDeUsuarios>(r => r.SalvarAlteracoesAsync(), Times.Never);

            #endregion
        }

        [Fact]
        internal async Task DeveRetornarExcecaoAoCriarUsuarioComEmailJaExistente()
        {
            #region Preparação do teste

            var dominio = _mocker.CreateInstance<DominioDeUsuarios>();

            var usuarioParaCriar = ConstrutorDeUsuariosTeste
                .RetornarConstrutor()
                .ConstruirNovo();

            _mocker.GetMock<IRepositorioDeUsuarios>().Setup(r => r.ExisteEmailAsync(usuarioParaCriar.Email, It.IsAny<Guid>())).ReturnsAsync(true);

            #endregion

            var excecao = await Assert.ThrowsAsync<ExcecoesUsuario>(() => dominio.CriarAsync(usuarioParaCriar));

            #region Verificação do teste

            Assert.Equal(MensagensValidacaoUsuario.EmailJaCadastrado.Descricao(), excecao.Message);
            _mocker.Verify<IRepositorioDeUsuarios>(r => r.Adicionar(It.IsAny<Usuario>()), Times.Never);
            _mocker.Verify<IRepositorioDeUsuarios>(r => r.SalvarAlteracoesAsync(), Times.Never);

            #endregion
        }

        [Fact]
        internal async Task DeveRetornarExcecaoAoCriarUsuarioComDominioComEmailJaExistente()
        {
            #region Preparação do teste

            var dominio = _mocker.CreateInstance<DominioDeUsuarios>();

            var usuarioParaCriar = ConstrutorDeUsuariosTeste
                .RetornarConstrutor()
                .ConstruirNovo();

            var nomeDominio = _faker.Internet.DomainName();

            _mocker.GetMock<IRepositorioDeUsuarios>().Setup(r => r.ExisteEmailAsync(usuarioParaCriar.Email, nomeDominio)).ReturnsAsync(true);

            #endregion

            var excecao = await Assert.ThrowsAsync<ExcecoesUsuario>(() => dominio.CriarUsuarioComDominioAsync(usuarioParaCriar, nomeDominio));

            #region Verificação do teste

            Assert.Equal(MensagensValidacaoUsuario.EmailJaCadastrado.Descricao(), excecao.Message);
            _mocker.Verify<IRepositorioDeUsuarios>(r => r.Adicionar(It.IsAny<Usuario>(), It.IsAny<string>()), Times.Never);
            _mocker.Verify<IRepositorioDeUsuarios>(r => r.SalvarAlteracoesAsync(), Times.Never);

            #endregion
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        internal async Task DeveRetornarSeExisteNomeUsuarioJaCadastrado(bool resultadoEsperado)
        {
            #region Preparação do teste

            var dominio = _mocker.CreateInstance<DominioDeUsuarios>();

            var nomeUsuarioParaTeste = _faker.Person.UserName;

            _mocker.GetMock<IRepositorioDeUsuarios>().Setup(r => r.ExisteNomeUsuarioAsync(nomeUsuarioParaTeste, It.IsAny<Guid?>())).ReturnsAsync(resultadoEsperado);

            #endregion

            var existeNomeUsuario = await dominio.ExisteNomeUsuarioAsync(nomeUsuarioParaTeste);

            #region Verificação do teste

            Assert.Equal(resultadoEsperado, existeNomeUsuario);
            _mocker.Verify<IRepositorioDeUsuarios>(r => r.ExisteNomeUsuarioAsync(nomeUsuarioParaTeste, It.IsAny<Guid?>()), Times.Once);

            #endregion
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        internal async Task DeveRetornarSeExisteEmailJaCadastrado(bool resultadoEsperado)
        {
            #region Preparação do teste

            var dominio = _mocker.CreateInstance<DominioDeUsuarios>();

            var nomeUsuarioParaTeste = _faker.Person.Email;

            _mocker.GetMock<IRepositorioDeUsuarios>().Setup(r => r.ExisteEmailAsync(nomeUsuarioParaTeste, It.IsAny<Guid?>())).ReturnsAsync(resultadoEsperado);

            #endregion

            var existeEmail = await dominio.ExisteEmailAsync(nomeUsuarioParaTeste);

            #region Verificação do teste

            Assert.Equal(resultadoEsperado, existeEmail);
            _mocker.Verify<IRepositorioDeUsuarios>(r => r.ExisteEmailAsync(nomeUsuarioParaTeste, It.IsAny<Guid?>()), Times.Once);

            #endregion
        }

        [Fact]
        internal async Task DeveAlterarUsuario()
        {
            #region Preparação do teste

            var dominio = _mocker.CreateInstance<DominioDeUsuarios>();

            var usuarioParaEditar = ConstrutorDeUsuariosTeste
                .RetornarConstrutor()
                .ConstruirNovo();

            _mocker.GetMock<IRepositorioDeUsuarios>().Setup(r => r.BuscarUnicoAsync(usuarioParaEditar.Id)).ReturnsAsync(usuarioParaEditar);

            #endregion

            await dominio.AlterarAsync(usuarioParaEditar);

            #region Verificação do teste

            _mocker.Verify<IRepositorioDeUsuarios>(r => r.Editar(usuarioParaEditar), Times.Once);
            _mocker.Verify<IRepositorioDeUsuarios>(r => r.SalvarAlteracoesAsync(), Times.Once);

            #endregion
        }

        [Fact]
        internal async Task DeveRetornarExcecaoAoAlterarUsuarioComNomeUsuarioJaExistente()
        {
            #region Preparação do teste

            var dominio = _mocker.CreateInstance<DominioDeUsuarios>();

            var usuarioParaAlterar = ConstrutorDeUsuariosTeste
                .RetornarConstrutor()
                .ConstruirCadastrado();

            _mocker.GetMock<IRepositorioDeUsuarios>().Setup(r => r.ExisteNomeUsuarioAsync(usuarioParaAlterar.NomeUsuario, usuarioParaAlterar.Id)).ReturnsAsync(true);

            #endregion

            var excecao = await Assert.ThrowsAsync<ExcecoesUsuario>(() => dominio.AlterarAsync(usuarioParaAlterar));

            #region Verificação do teste

            Assert.Equal(MensagensValidacaoUsuario.NomeUsuarioJaCadastrado.Descricao(), excecao.Message);
            _mocker.Verify<IRepositorioDeUsuarios>(r => r.Editar(It.IsAny<Usuario>()), Times.Never);
            _mocker.Verify<IRepositorioDeUsuarios>(r => r.SalvarAlteracoesAsync(), Times.Never);

            #endregion
        }

        [Fact]
        internal async Task DeveRetornarExcecaoAoAlterarUsuarioComEmailJaExistente()
        {
            #region Preparação do teste

            var dominio = _mocker.CreateInstance<DominioDeUsuarios>();

            var usuarioParaAlterar = ConstrutorDeUsuariosTeste
                .RetornarConstrutor()
                .ConstruirCadastrado();

            _mocker.GetMock<IRepositorioDeUsuarios>().Setup(r => r.ExisteEmailAsync(usuarioParaAlterar.Email, usuarioParaAlterar.Id)).ReturnsAsync(true);

            #endregion

            var excecao = await Assert.ThrowsAsync<ExcecoesUsuario>(() => dominio.AlterarAsync(usuarioParaAlterar));

            #region Verificação do teste

            Assert.Equal(MensagensValidacaoUsuario.EmailJaCadastrado.Descricao(), excecao.Message);
            _mocker.Verify<IRepositorioDeUsuarios>(r => r.Editar(It.IsAny<Usuario>()), Times.Never);
            _mocker.Verify<IRepositorioDeUsuarios>(r => r.SalvarAlteracoesAsync(), Times.Never);

            #endregion
        }

        [Fact]
        internal async Task DeveBuscarUnicoUsuario()
        {
            #region Preparação do teste

            var idUsuario = Guid.NewGuid();
            var dominio = _mocker.CreateInstance<DominioDeUsuarios>();

            var usuarioCadastrado = ConstrutorDeUsuariosTeste
                .RetornarConstrutor()
                .ConstruirCadastrado();

            _mocker.GetMock<IRepositorioDeUsuarios>().Setup(r => r.BuscarUnicoAsync(idUsuario)).ReturnsAsync(usuarioCadastrado);

            #endregion

            var usuario = await dominio.BuscarUnicoAsync(idUsuario);

            #region Verificação do teste

            Assert.NotNull(usuario);
            Assert.Equal(usuarioCadastrado, usuario);

            #endregion
        }

        [Fact]
        internal async Task DeveRetornarNuloAoBuscarUnicoUsuarioNaoExistente()
        {
            #region Preparação do teste

            var idUsuario = Guid.NewGuid();
            var dominio = _mocker.CreateInstance<DominioDeUsuarios>();

            #endregion

            var usuario = await dominio.BuscarUnicoAsync(idUsuario);

            #region Verificação do teste

            Assert.Null(usuario);

            #endregion
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        internal async Task DeveRetornarSeUsuarioEhAdmin(bool resultadoEsperado)
        {
            #region Preparação do teste

            var idUsuario = Guid.NewGuid();

            var dominio = _mocker.CreateInstance<DominioDeUsuarios>();

            _mocker.GetMock<IRepositorioDeUsuarios>().Setup(r => r.EhAdmAsync(idUsuario)).ReturnsAsync(resultadoEsperado);

            #endregion

            var retorno = await dominio.EhAdmAsync(idUsuario);

            #region Verificação do teste

            Assert.Equal(resultadoEsperado, retorno);
            _mocker.Verify<IRepositorioDeUsuarios>(r => r.EhAdmAsync(idUsuario), Times.Once);

            #endregion
        }

        [Fact]
        internal async Task DeveExcluirUsuario()
        {
            #region Preparação do teste

            var dominio = _mocker.CreateInstance<DominioDeUsuarios>();

            var usuarioParaExcluir = ConstrutorDeUsuariosTeste
                .RetornarConstrutor()
                .ConstruirNovo();

            _mocker.GetMock<IRepositorioDeUsuarios>().Setup(r => r.BuscarUnicoAsync(usuarioParaExcluir.Id)).ReturnsAsync(usuarioParaExcluir);

            #endregion

            await dominio.ExcluirUsuarioAsync(usuarioParaExcluir.Id);

            #region Verificação do teste

            _mocker.Verify<IRepositorioDeUsuarios>(r => r.Excluir(usuarioParaExcluir), Times.Once);
            _mocker.Verify<IRepositorioDeUsuarios>(r => r.SalvarAlteracoesAsync(), Times.Once);

            #endregion
        }

        [Fact]
        internal async Task DeveRetornarExcecaoAoTentarExcluirUsuarioInexistente()
        {
            #region Preparação do teste

            var dominio = _mocker.CreateInstance<DominioDeUsuarios>();

            #endregion

            var excecao = await Assert.ThrowsAsync<ExcecoesUsuario>(() => dominio.ExcluirUsuarioAsync(Guid.NewGuid()));

            #region Verificação do teste

            Assert.Equal(MensagensValidacaoUsuario.UsuarioNaoEncontrado.Descricao(), excecao.Message);
            _mocker.Verify<IRepositorioDeUsuarios>(r => r.Excluir(It.IsAny<Usuario>()), Times.Never);
            _mocker.Verify<IRepositorioDeUsuarios>(r => r.SalvarAlteracoesAsync(), Times.Never);

            #endregion
        }
    }
}

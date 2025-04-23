using AutenticacaoDoisFatores.Dominio.Compartilhados;
using AutenticacaoDoisFatores.Dominio.Compartilhados.Mensagens;
using AutenticacaoDoisFatores.Dominio.Dominios;
using AutenticacaoDoisFatores.Dominio.Excecoes;
using AutenticacaoDoisFatores.Dominio.Servicos;
using AutenticacaoDoisFatores.Testes.Compartilhados;
using Bogus;
using Moq;
using Moq.AutoMock;

namespace AutenticacaoDoisFatores.Testes.Dominio.Dominios
{
    public class DominioAppAutenticadorTeste
    {
        [Fact]
        internal void DeveGerarQrCode()
        {
            #region Preparação do teste

            var mocker = new AutoMocker();

            var dominio = mocker.CreateInstance<DominioAppAutenticador>();

            var usuario = ConstrutorDeUsuariosTeste
                .RetornarConstrutor()
                .ConstruirCadastrado();

            var faker = new Faker();

            var qrCode = faker.Commerce.Ean13();

            mocker.GetMock<IServicoDeAutenticador>().Setup(s => s.GerarQrCode(usuario.Email, usuario.ChaveSecreta)).Returns(qrCode);

            #endregion

            var retorno = dominio.GerarQrCode(usuario);

            #region Verificação do teste

            Assert.NotNull(retorno);
            Assert.Equal(qrCode, retorno);
            mocker.Verify<IServicoDeAutenticador>(s => s.GerarQrCode(usuario.Email, usuario.ChaveSecreta), Times.Once);

            #endregion
        }

        [Fact]
        internal void NaoDeveGerarQrCodeParaUsuarioNulo()
        {
            #region Preparação do teste

            var mocker = new AutoMocker();

            var dominio = mocker.CreateInstance<DominioAppAutenticador>();

            #endregion

            var excecao = Assert.Throws<ExcecoesUsuario>(() => dominio.GerarQrCode(null));

            #region Verificação do teste

            Assert.Equal(MensagensValidacaoUsuario.UsuarioNaoEncontrado.Descricao(), excecao.Message);
            mocker.Verify<IServicoDeAutenticador>(s => s.GerarQrCode(It.IsAny<string>(), It.IsAny<string>()), Times.Never);

            #endregion
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        internal void DeveRetornarSeCodigoEhValido(bool valorEsperado)
        {
            #region Preparação do teste

            var mocker = new AutoMocker();

            var dominio = mocker.CreateInstance<DominioAppAutenticador>();

            var usuario = ConstrutorDeUsuariosTeste
                .RetornarConstrutor()
                .ConstruirCadastrado();

            var codigo = "123456";
            mocker.GetMock<IServicoDeAutenticador>().Setup(s => s.CodigoEhValido(codigo, usuario.ChaveSecreta)).Returns(valorEsperado);

            #endregion

            var retorno = dominio.CodigoEhValido(codigo, usuario);

            #region Verificação do teste

            Assert.Equal(valorEsperado, retorno);
            mocker.Verify<IServicoDeAutenticador, bool>(s => s.CodigoEhValido(codigo, usuario.ChaveSecreta), Times.Once);

            #endregion
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        internal void DeveRetornarExcecaoSeVerificarSeCodigoEhValidoParaCodigoVazio(string? codigo)
        {
            #region Preparação do teste

            var mocker = new AutoMocker();

            var dominio = mocker.CreateInstance<DominioAppAutenticador>();

            var usuario = ConstrutorDeUsuariosTeste
                .RetornarConstrutor()
                .ConstruirCadastrado();

            #endregion

            var excecao = Assert.Throws<ExcecoesAppAutenticador>(() => dominio.CodigoEhValido(codigo, usuario));

            #region Verificação do teste

            Assert.Equal(MensagensValidacaoAppAutenticacao.CodigoNaoInformado.Descricao(), excecao.Message);
            mocker.Verify<IServicoDeAutenticador, bool>(s => s.CodigoEhValido(It.IsAny<string>(), It.IsAny<string>()), Times.Never);

            #endregion
        }
    }
}

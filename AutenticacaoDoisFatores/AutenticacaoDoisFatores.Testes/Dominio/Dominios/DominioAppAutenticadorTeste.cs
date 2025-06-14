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

            var dominio = mocker.CreateInstance<AuthApp>();

            var usuario = ConstrutorDeUsuariosTeste
                .RetornarConstrutor()
                .Build();

            var faker = new Faker();

            var qrCode = faker.Commerce.Ean13();

            mocker.GetMock<IAuthService>().Setup(s => s.GerarQrCode(usuario.Email, usuario.ChaveSecreta)).Returns(qrCode);

            #endregion

            var retorno = dominio.GerarQrCode(usuario);

            #region Verificação do teste

            Assert.NotNull(retorno);
            Assert.Equal(qrCode, retorno);
            mocker.Verify<IAuthService>(s => s.GerarQrCode(usuario.Email, usuario.ChaveSecreta), Times.Once);

            #endregion
        }

        [Fact]
        internal void NaoDeveGerarQrCodeParaUsuarioNulo()
        {
            #region Preparação do teste

            var mocker = new AutoMocker();

            var dominio = mocker.CreateInstance<AuthApp>();

            #endregion

            var excecao = Assert.Throws<UserException>(() => dominio.GerarQrCode(null));

            #region Verificação do teste

            Assert.Equal(UserValidationMessages.UserNotFound.Description(), excecao.Message);
            mocker.Verify<IAuthService>(s => s.GenerateQrCode(It.IsAny<string>(), It.IsAny<string>()), Times.Never);

            #endregion
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        internal void DeveRetornarSeCodigoEhValido(bool valorEsperado)
        {
            #region Preparação do teste

            var mocker = new AutoMocker();

            var dominio = mocker.CreateInstance<AuthApp>();

            var usuario = ConstrutorDeUsuariosTeste
                .RetornarConstrutor()
                .Build();

            var codigo = "123456";
            mocker.GetMock<IAuthService>().Setup(s => s.CodigoEhValido(codigo, usuario.ChaveSecreta)).Returns(valorEsperado);

            #endregion

            var retorno = dominio.CodigoEhValido(codigo, usuario);

            #region Verificação do teste

            Assert.Equal(valorEsperado, retorno);
            mocker.Verify<IAuthService, bool>(s => s.CodigoEhValido(codigo, usuario.ChaveSecreta), Times.Once);

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

            var dominio = mocker.CreateInstance<AuthApp>();

            var usuario = ConstrutorDeUsuariosTeste
                .RetornarConstrutor()
                .Build();

            #endregion

            var excecao = Assert.Throws<AuthAppException>(() => dominio.CodigoEhValido(codigo, usuario));

            #region Verificação do teste

            Assert.Equal(AuthAppValidationMessages.CodeNotInformed.Description(), excecao.Message);
            mocker.Verify<IAuthService, bool>(s => s.IsValidCode(It.IsAny<string>(), It.IsAny<string>()), Times.Never);

            #endregion
        }
    }
}

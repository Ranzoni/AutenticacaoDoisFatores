using AutenticacaoDoisFatores.Dominio.Dominios;
using AutenticacaoDoisFatores.Dominio.Servicos;
using AutenticacaoDoisFatores.Testes.Compartilhados;
using Bogus;
using Mensageiro;
using Moq.AutoMock;
using Moq;
using AutenticacaoDoisFatores.Servico.CasosDeUso.Usuarios.Autenticadores;
using AutenticacaoDoisFatores.Dominio.Repositorios;
using AutenticacaoDoisFatores.Dominio.Compartilhados.Mensagens;

namespace AutenticacaoDoisFatores.Testes.Servico.Usuarios
{
    public class GerarQrCodeAppAutenticacaoTeste
    {
        [Fact]
        internal async Task DeveGerarQrCode()
        {
            #region Preparação do teste

            var mocker = new AutoMocker();

            var autenticadorPorApp = mocker.GetMock<AuthApp>().Object;
            var dominioDeUsuarios = mocker.GetMock<UserDomain>().Object;
            var email = mocker.GetMock<EmailSender>().Object;
            var notificador = mocker.GetMock<INotificador>().Object;
            var linkBaseParaQrCode = "https://example.com/qrcode";

            var servico = new GerarQrCodeAppAutenticacao(autenticadorPorApp, dominioDeUsuarios, email, notificador, linkBaseParaQrCode);

            var usuario = ConstrutorDeUsuariosTeste
                .RetornarConstrutor(ativo: true)
                .Build();

            var faker = new Faker();

            var qrCode = faker.Commerce.Ean13();

            mocker.GetMock<IUserRepository>().Setup(r => r.BuscarUnicoAsync(usuario.Id)).ReturnsAsync(usuario);
            mocker.GetMock<IAuthService>().Setup(s => s.GerarQrCode(usuario.Email, usuario.ChaveSecreta)).Returns(qrCode);

            #endregion

            await servico.ExecutarAsync(usuario.Id);

            #region Verificação do teste

            mocker.Verify<IEmailService>(s => s.Enviar(usuario.Email, It.IsAny<string>(), It.IsAny<string>()), Times.Once);
            mocker.Verify<IAuthService>(s => s.GerarQrCode(usuario.Email, usuario.ChaveSecreta), Times.Once);

            #endregion
        }

        [Fact]
        internal async Task NaoDeveAutenticarPorAppParaUsuarioInexistente()
        {
            #region Preparação do teste

            var mocker = new AutoMocker();

            var autenticadorPorApp = mocker.GetMock<AuthApp>().Object;
            var dominioDeUsuarios = mocker.GetMock<UserDomain>().Object;
            var email = mocker.GetMock<EmailSender>().Object;
            var notificador = mocker.GetMock<INotificador>().Object;
            var linkBaseParaQrCode = "https://example.com/qrcode";

            var servico = new GerarQrCodeAppAutenticacao(autenticadorPorApp, dominioDeUsuarios, email, notificador, linkBaseParaQrCode);

            mocker.GetMock<INotificador>().Setup(n => n.ExisteMensagem()).Returns(true);

            #endregion

            await servico.ExecutarAsync(Guid.NewGuid());

            #region Verificação do teste

            mocker.Verify<IAuthService>(s => s.GenerateQrCode(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
            mocker.Verify<IEmailService>(s => s.Send(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
            mocker.Verify<INotificador>(n => n.AddMensagemNaoEncontrado(UserValidationMessages.UserNotFound), Times.Once);

            #endregion
        }

        [Fact]
        internal async Task NaoDeveAutenticarPorAppParaUsuarioInativo()
        {
            #region Preparação do teste

            var mocker = new AutoMocker();

            var autenticadorPorApp = mocker.GetMock<AuthApp>().Object;
            var dominioDeUsuarios = mocker.GetMock<UserDomain>().Object;
            var email = mocker.GetMock<EmailSender>().Object;
            var notificador = mocker.GetMock<INotificador>().Object;
            var linkBaseParaQrCode = "https://example.com/qrcode";

            var servico = new GerarQrCodeAppAutenticacao(autenticadorPorApp, dominioDeUsuarios, email, notificador, linkBaseParaQrCode);

            var usuario = ConstrutorDeUsuariosTeste
                .RetornarConstrutor(ativo: false)
                .Build();

            mocker.GetMock<IUserRepository>().Setup(r => r.BuscarUnicoAsync(usuario.Id)).ReturnsAsync(usuario);
            mocker.GetMock<INotificador>().Setup(n => n.ExisteMensagem()).Returns(true);

            #endregion

            await servico.ExecutarAsync(usuario.Id);

            #region Verificação do teste

            mocker.Verify<IAuthService>(s => s.GenerateQrCode(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
            mocker.Verify<IEmailService>(s => s.Send(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
            mocker.Verify<INotificador>(n => n.AddMensagemNaoEncontrado(UserValidationMessages.UserNotFound), Times.Once);

            #endregion
        }
    }
}

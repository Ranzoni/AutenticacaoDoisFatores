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

            var autenticadorPorApp = mocker.GetMock<AppAutenticador>().Object;
            var dominioDeUsuarios = mocker.GetMock<DominioDeUsuarios>().Object;
            var email = mocker.GetMock<EnvioDeEmail>().Object;
            var notificador = mocker.GetMock<INotificador>().Object;
            var linkBaseParaQrCode = "https://example.com/qrcode";

            var servico = new GerarQrCodeAppAutenticacao(autenticadorPorApp, dominioDeUsuarios, email, notificador, linkBaseParaQrCode);

            var usuario = ConstrutorDeUsuariosTeste
                .RetornarConstrutor(ativo: true)
                .ConstruirCadastrado();

            var faker = new Faker();

            var qrCode = faker.Commerce.Ean13();

            mocker.GetMock<IRepositorioDeUsuarios>().Setup(r => r.BuscarUnicoAsync(usuario.Id)).ReturnsAsync(usuario);
            mocker.GetMock<IServicoDeAutenticador>().Setup(s => s.GerarQrCode(usuario.Email, usuario.ChaveSecreta)).Returns(qrCode);

            #endregion

            await servico.ExecutarAsync(usuario.Id);

            #region Verificação do teste

            mocker.Verify<IServicoDeEmail>(s => s.Enviar(usuario.Email, It.IsAny<string>(), It.IsAny<string>()), Times.Once);
            mocker.Verify<IServicoDeAutenticador>(s => s.GerarQrCode(usuario.Email, usuario.ChaveSecreta), Times.Once);

            #endregion
        }

        [Fact]
        internal async Task NaoDeveAutenticarPorAppParaUsuarioInexistente()
        {
            #region Preparação do teste

            var mocker = new AutoMocker();

            var autenticadorPorApp = mocker.GetMock<AppAutenticador>().Object;
            var dominioDeUsuarios = mocker.GetMock<DominioDeUsuarios>().Object;
            var email = mocker.GetMock<EnvioDeEmail>().Object;
            var notificador = mocker.GetMock<INotificador>().Object;
            var linkBaseParaQrCode = "https://example.com/qrcode";

            var servico = new GerarQrCodeAppAutenticacao(autenticadorPorApp, dominioDeUsuarios, email, notificador, linkBaseParaQrCode);

            mocker.GetMock<INotificador>().Setup(n => n.ExisteMensagem()).Returns(true);

            #endregion

            await servico.ExecutarAsync(Guid.NewGuid());

            #region Verificação do teste

            mocker.Verify<IServicoDeAutenticador>(s => s.GerarQrCode(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
            mocker.Verify<IServicoDeEmail>(s => s.Enviar(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
            mocker.Verify<INotificador>(n => n.AddMensagemNaoEncontrado(MensagensValidacaoUsuario.UsuarioNaoEncontrado), Times.Once);

            #endregion
        }

        [Fact]
        internal async Task NaoDeveAutenticarPorAppParaUsuarioInativo()
        {
            #region Preparação do teste

            var mocker = new AutoMocker();

            var autenticadorPorApp = mocker.GetMock<AppAutenticador>().Object;
            var dominioDeUsuarios = mocker.GetMock<DominioDeUsuarios>().Object;
            var email = mocker.GetMock<EnvioDeEmail>().Object;
            var notificador = mocker.GetMock<INotificador>().Object;
            var linkBaseParaQrCode = "https://example.com/qrcode";

            var servico = new GerarQrCodeAppAutenticacao(autenticadorPorApp, dominioDeUsuarios, email, notificador, linkBaseParaQrCode);

            var usuario = ConstrutorDeUsuariosTeste
                .RetornarConstrutor(ativo: false)
                .ConstruirCadastrado();

            mocker.GetMock<IRepositorioDeUsuarios>().Setup(r => r.BuscarUnicoAsync(usuario.Id)).ReturnsAsync(usuario);
            mocker.GetMock<INotificador>().Setup(n => n.ExisteMensagem()).Returns(true);

            #endregion

            await servico.ExecutarAsync(usuario.Id);

            #region Verificação do teste

            mocker.Verify<IServicoDeAutenticador>(s => s.GerarQrCode(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
            mocker.Verify<IServicoDeEmail>(s => s.Enviar(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
            mocker.Verify<INotificador>(n => n.AddMensagemNaoEncontrado(MensagensValidacaoUsuario.UsuarioNaoEncontrado), Times.Once);

            #endregion
        }
    }
}

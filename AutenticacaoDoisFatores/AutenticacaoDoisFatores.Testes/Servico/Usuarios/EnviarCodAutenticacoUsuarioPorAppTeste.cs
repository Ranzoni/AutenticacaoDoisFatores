using AutenticacaoDoisFatores.Dominio.Compartilhados.Mensagens;
using AutenticacaoDoisFatores.Dominio.Servicos;
using AutenticacaoDoisFatores.Servico.CasosDeUso.Usuarios.Autenticadores.AutenticacoesDoisFatores;
using AutenticacaoDoisFatores.Testes.Compartilhados;
using Bogus;
using Mensageiro;
using Moq;
using Moq.AutoMock;

namespace AutenticacaoDoisFatores.Testes.Servico.Usuarios
{
    public class EnviarCodAutenticacoUsuarioPorAppTeste
    {
        [Fact]
        internal async Task DeveAutenticarPorApp()
        {
            #region Preparação do teste

            var mocker = new AutoMocker();

            var servico = mocker.CreateInstance<AutenticadorUsuarioEmDoisFatoresPorApp>();

            var usuario = ConstrutorDeUsuariosTeste
                .RetornarConstrutor(ativo: true)
                .ConstruirCadastrado();

            var faker = new Faker();

            var qrCode = faker.Commerce.Ean13();

            mocker.GetMock<IServicoDeAutenticador>().Setup(s => s.GerarQrCode(usuario.Email, usuario.ChaveSecreta)).Returns(qrCode);

            #endregion

            var retorno = await servico.EnviarAsync(usuario);

            #region Verificação do teste

            Assert.NotNull(retorno);
            Assert.NotEmpty(retorno.Token);
            Assert.NotNull(retorno.QrCode);
            Assert.NotEmpty(retorno.QrCode);

            mocker.Verify<IServicoDeAutenticador>(s => s.GerarQrCode(usuario.Email, usuario.ChaveSecreta), Times.Once);

            #endregion
        }

        [Fact]
        internal async Task NaoDeveAutenticarPorAppParaUsuarioInativo()
        {
            #region Preparação do teste

            var mocker = new AutoMocker();

            var servico = mocker.CreateInstance<AutenticadorUsuarioEmDoisFatoresPorApp>();

            var usuario = ConstrutorDeUsuariosTeste
                .RetornarConstrutor(ativo: false)
                .ConstruirCadastrado();

            mocker.GetMock<INotificador>().Setup(n => n.ExisteMensagem()).Returns(true);

            #endregion

            var retorno = await servico.EnviarAsync(usuario);

            #region Verificação do teste

            Assert.Null(retorno);
            mocker.Verify<IServicoDeAutenticador>(s => s.GerarQrCode(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
            mocker.Verify<INotificador>(n => n.AddMensagemNaoEncontrado(MensagensValidacaoUsuario.UsuarioNaoEncontrado), Times.Once);

            #endregion
        }
    }
}

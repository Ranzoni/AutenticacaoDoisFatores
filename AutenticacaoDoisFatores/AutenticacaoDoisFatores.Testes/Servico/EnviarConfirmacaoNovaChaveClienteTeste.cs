using AutenticacaoDoisFatores.Dominio.Compartilhados.Mensagens;
using AutenticacaoDoisFatores.Dominio.Repositorios;
using AutenticacaoDoisFatores.Dominio.Servicos;
using AutenticacaoDoisFatores.Servico.CasosDeUso.Clientes;
using AutenticacaoDoisFatores.Testes.Compartilhados;
using Bogus;
using Mensageiro;
using Moq;
using Moq.AutoMock;

namespace AutenticacaoDoisFatores.Testes.Servico
{
    public class EnviarConfirmacaoNovaChaveClienteTeste
    {
        private readonly AutoMocker _mocker = new();
        private readonly Faker _faker = new();

        [Fact]
        internal async Task DeveEnviarEmailDeConfirmacaoDeNovaChave()
        {
            #region Preparação do teste

            var servico = _mocker.CreateInstance<EnviarConfirmacaoNovaChaveCliente>();
            var emailParaTeste = _faker.Person.Email;
            var urlParaTeste = _faker.Internet.UrlWithPath();
            var cliente = ConstrutorDeClientesTeste
                .RetornarConstrutorDeCliente(email: emailParaTeste, ativo: true)
                .ConstruirClienteCadastrado();

            _mocker.GetMock<IRepositorioDeClientes>().Setup(r => r.BuscarPorEmailAsync(emailParaTeste)).ReturnsAsync(cliente);

            #endregion

            await servico.EnviarAsync(emailParaTeste, urlParaTeste);

            #region Verificação do teste

            _mocker.Verify<IServicoDeEmail>(e => e.Enviar(emailParaTeste, It.IsAny<string>(), It.IsAny<string>()), Times.Once);

            #endregion
        }

        [Fact]
        internal async Task NaoDeveEnviarEmailDeConfirmacaoDeNovaChaveQuandoClienteNaoExiste()
        {
            #region Preparação do teste

            var servico = _mocker.CreateInstance<EnviarConfirmacaoNovaChaveCliente>();
            var emailParaTeste = _faker.Person.Email;
            var urlParaTeste = _faker.Internet.UrlWithPath();

            _mocker.GetMock<INotificador>().Setup(n => n.ExisteMensagem()).Returns(true);

            #endregion

            await servico.EnviarAsync(emailParaTeste, urlParaTeste);

            #region Verificação do teste

            _mocker.Verify<IServicoDeEmail>(e => e.Enviar(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
            _mocker.Verify<INotificador>(n => n.AddMensagemNaoEncontrado(MensagensValidacaoCliente.ClienteNaoEncontrado), Times.Once);

            #endregion
        }

        [Fact]
        internal async Task NaoDeveEnviarEmailDeConfirmacaoDeNovaChaveQuandoClienteEstaInativo()
        {
            #region Preparação do teste

            var servico = _mocker.CreateInstance<EnviarConfirmacaoNovaChaveCliente>();
            var emailParaTeste = _faker.Person.Email;
            var urlParaTeste = _faker.Internet.UrlWithPath();
            var cliente = ConstrutorDeClientesTeste
                .RetornarConstrutorDeCliente(email: emailParaTeste, ativo: false)
                .ConstruirClienteCadastrado();

            _mocker.GetMock<IRepositorioDeClientes>().Setup(r => r.BuscarPorEmailAsync(emailParaTeste)).ReturnsAsync(cliente);
            _mocker.GetMock<INotificador>().Setup(n => n.ExisteMensagem()).Returns(true);

            #endregion

            await servico.EnviarAsync(emailParaTeste, urlParaTeste);

            #region Verificação do teste

            _mocker.Verify<IServicoDeEmail>(e => e.Enviar(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
            _mocker.Verify<INotificador>(n => n.AddMensagem(MensagensValidacaoCliente.ClienteNaoAtivo), Times.Once);

            #endregion
        }
    }
}

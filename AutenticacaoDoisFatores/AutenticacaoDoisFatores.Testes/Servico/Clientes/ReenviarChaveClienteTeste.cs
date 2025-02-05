using AutenticacaoDoisFatores.Dominio.Compartilhados.Mensagens;
using AutenticacaoDoisFatores.Dominio.Entidades;
using AutenticacaoDoisFatores.Dominio.Repositorios;
using AutenticacaoDoisFatores.Dominio.Servicos;
using AutenticacaoDoisFatores.Servico.CasosDeUso.Clientes;
using AutenticacaoDoisFatores.Testes.Compartilhados;
using Bogus;
using Mensageiro;
using Moq;
using Moq.AutoMock;

namespace AutenticacaoDoisFatores.Testes.Servico.Clientes
{
    public class ReenviarChaveClienteTeste
    {
        private readonly Faker _faker = new();
        private readonly AutoMocker _mock = new();

        [Fact]
        internal async Task DeveReenviarChaveAoCliente()
        {
            #region Preparação do teste

            var emailParaTeste = _faker.Person.Email;
            var urlParaTeste = _faker.Internet.UrlRootedPath();
            var cliente = ConstrutorDeClientesTeste
                .RetornarConstrutor(email: emailParaTeste, ativo: false)
                .ConstruirCadastrado();

            _mock.GetMock<IRepositorioDeClientes>().Setup(r => r.BuscarPorEmailAsync(emailParaTeste)).ReturnsAsync(cliente);

            var servico = _mock.CreateInstance<ReenviarChaveCliente>();

            #endregion

            await servico.ReenviarAsync(emailParaTeste, urlParaTeste);

            #region Verificação do teste

            _mock.Verify<IRepositorioDeClientes>(r => r.Editar(cliente), Times.Once);
            _mock.Verify<IRepositorioDeClientes>(r => r.SalvarAlteracoesAsync(), Times.Once);
            _mock.Verify<IServicoDeEmail>(s => s.Enviar(cliente.Email, It.IsAny<string>(), It.IsAny<string>()), Times.Once);

            #endregion
        }

        [Fact]
        internal async Task NaoDeveReenviarChaveAoClienteQuandoIdClienteNaoExiste()
        {
            #region Preparação do teste

            var emailParaTeste = _faker.Person.Email;
            var urlParaTeste = _faker.Internet.UrlRootedPath();
            var servico = _mock.CreateInstance<ReenviarChaveCliente>();

            #endregion

            await servico.ReenviarAsync(emailParaTeste, urlParaTeste);

            #region Verificação do teste

            _mock.Verify<IRepositorioDeClientes>(r => r.Editar(It.IsAny<Cliente>()), Times.Never);
            _mock.Verify<IRepositorioDeClientes>(r => r.SalvarAlteracoesAsync(), Times.Never);
            _mock.Verify<IServicoDeEmail>(s => s.Enviar(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
            _mock.Verify<INotificador>(n => n.AddMensagemNaoEncontrado(MensagensValidacaoCliente.ClienteNaoEncontrado), Times.Once);

            #endregion
        }

        [Fact]
        internal async Task NaoDeveReenviarChaveAoClienteQuandoJaAtivado()
        {
            #region Preparação do teste

            var emailParaTeste = _faker.Person.Email;
            var urlParaTeste = _faker.Internet.UrlRootedPath();
            var cliente = ConstrutorDeClientesTeste
                .RetornarConstrutor(email: emailParaTeste, ativo: true)
                .ConstruirCadastrado();

            _mock.GetMock<IRepositorioDeClientes>().Setup(r => r.BuscarPorEmailAsync(emailParaTeste)).ReturnsAsync(cliente);
            _mock.GetMock<INotificador>().Setup(n => n.ExisteMensagem()).Returns(true);

            var servico = _mock.CreateInstance<ReenviarChaveCliente>();

            #endregion

            await servico.ReenviarAsync(emailParaTeste, urlParaTeste);

            #region Verificação do teste

            _mock.Verify<IRepositorioDeClientes>(r => r.Editar(It.IsAny<Cliente>()), Times.Never);
            _mock.Verify<IRepositorioDeClientes>(r => r.SalvarAlteracoesAsync(), Times.Never);
            _mock.Verify<IServicoDeEmail>(s => s.Enviar(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
            _mock.Verify<INotificador>(n => n.AddMensagem(MensagensValidacaoCliente.ClienteJaAtivado), Times.Once);

            #endregion
        }
    }
}

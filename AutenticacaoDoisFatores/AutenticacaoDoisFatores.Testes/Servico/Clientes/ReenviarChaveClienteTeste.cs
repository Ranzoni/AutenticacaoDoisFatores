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
                .Build();

            _mock.GetMock<IClientRepository>().Setup(r => r.GetByEmailAsync(emailParaTeste)).ReturnsAsync(cliente);

            var servico = _mock.CreateInstance<ResendClientKey>();

            #endregion

            await servico.ResendAsync(emailParaTeste, urlParaTeste);

            #region Verificação do teste

            _mock.Verify<IClientRepository>(r => r.Editar(cliente), Times.Once);
            _mock.Verify<IClientRepository>(r => r.SaveChangesAsync(), Times.Once);
            _mock.Verify<IEmailService>(s => s.Enviar(cliente.Email, It.IsAny<string>(), It.IsAny<string>()), Times.Once);

            #endregion
        }

        [Fact]
        internal async Task NaoDeveReenviarChaveAoClienteQuandoIdClienteNaoExiste()
        {
            #region Preparação do teste

            var emailParaTeste = _faker.Person.Email;
            var urlParaTeste = _faker.Internet.UrlRootedPath();
            var servico = _mock.CreateInstance<ResendClientKey>();

            #endregion

            await servico.ResendAsync(emailParaTeste, urlParaTeste);

            #region Verificação do teste

            _mock.Verify<IClientRepository>(r => r.Update(It.IsAny<Client>()), Times.Never);
            _mock.Verify<IClientRepository>(r => r.SaveChangesAsync(), Times.Never);
            _mock.Verify<IEmailService>(s => s.Send(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
            _mock.Verify<INotificador>(n => n.AddMensagemNaoEncontrado(ClientValidationMessages.ClientNotFound), Times.Once);

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
                .Build();

            _mock.GetMock<IClientRepository>().Setup(r => r.GetByEmailAsync(emailParaTeste)).ReturnsAsync(cliente);
            _mock.GetMock<INotificador>().Setup(n => n.ExisteMensagem()).Returns(true);

            var servico = _mock.CreateInstance<ResendClientKey>();

            #endregion

            await servico.ResendAsync(emailParaTeste, urlParaTeste);

            #region Verificação do teste

            _mock.Verify<IClientRepository>(r => r.Update(It.IsAny<Client>()), Times.Never);
            _mock.Verify<IClientRepository>(r => r.SaveChangesAsync(), Times.Never);
            _mock.Verify<IEmailService>(s => s.Send(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
            _mock.Verify<INotificador>(n => n.AddMensagem(ClientValidationMessages.ClientAlreadyActivated), Times.Once);

            #endregion
        }
    }
}

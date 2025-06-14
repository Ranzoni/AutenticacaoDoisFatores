using AutenticacaoDoisFatores.Dominio.Compartilhados.Mensagens;
using AutenticacaoDoisFatores.Dominio.Entidades;
using AutenticacaoDoisFatores.Dominio.Repositorios;
using AutenticacaoDoisFatores.Dominio.Servicos;
using AutenticacaoDoisFatores.Servico.CasosDeUso.Clientes;
using AutenticacaoDoisFatores.Testes.Compartilhados;
using Mensageiro;
using Moq;
using Moq.AutoMock;

namespace AutenticacaoDoisFatores.Testes.Servico.Clientes
{
    public class GerarNovaChaveAcessoClienteTeste
    {
        private readonly AutoMocker _mocker = new();

        [Fact]
        internal async Task DeveGerarNovaChaveAcesso()
        {
            #region Preparação do teste

            var servico = _mocker.CreateInstance<GerarNovaChaveAcessoCliente>();
            var idClienteParaTeste = Guid.NewGuid();
            var cliente = ConstrutorDeClientesTeste
                .RetornarConstrutor(id: idClienteParaTeste, ativo: true)
                .Build();

            _mocker.GetMock<IClientRepository>().Setup(r => r.GetByIdAsync(idClienteParaTeste)).ReturnsAsync(cliente);

            #endregion

            await servico.GerarNovaChaveAsync(idClienteParaTeste);

            #region Verificação do teste

            _mocker.Verify<IClientRepository>(r => r.Editar(cliente), Times.Once);
            _mocker.Verify<IClientRepository>(r => r.SaveChangesAsync(), Times.Once);
            _mocker.Verify<IEmailService>(e => e.Enviar(cliente.Email, It.IsAny<string>(), It.IsAny<string>()), Times.Once);

            #endregion
        }

        [Fact]
        internal async Task NaoDeveGerarNovaChaveAcessoQuandoClienteNaoExiste()
        {
            #region Preparação do teste

            var servico = _mocker.CreateInstance<GerarNovaChaveAcessoCliente>();
            var idClienteParaTeste = Guid.NewGuid();

            #endregion

            await servico.GerarNovaChaveAsync(idClienteParaTeste);

            #region Verificação do teste

            _mocker.Verify<IClientRepository>(r => r.Update(It.IsAny<Client>()), Times.Never);
            _mocker.Verify<IClientRepository>(r => r.SaveChangesAsync(), Times.Never);
            _mocker.Verify<IEmailService>(e => e.Send(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
            _mocker.Verify<INotificador>(n => n.AddMensagemNaoEncontrado(ClientValidationMessages.ClientNotFound), Times.Once);

            #endregion
        }

        [Fact]
        internal async Task NaoDeveGerarNovaChaveAcessoQuandoClienteEstaInativo()
        {
            #region Preparação do teste

            var servico = _mocker.CreateInstance<GerarNovaChaveAcessoCliente>();
            var idClienteParaTeste = Guid.NewGuid();
            var cliente = ConstrutorDeClientesTeste
                .RetornarConstrutor(id: idClienteParaTeste, ativo: false)
                .Build();

            _mocker.GetMock<IClientRepository>().Setup(r => r.GetByIdAsync(idClienteParaTeste)).ReturnsAsync(cliente);
            _mocker.GetMock<INotificador>().Setup(n => n.ExisteMensagem()).Returns(true);

            #endregion

            await servico.GerarNovaChaveAsync(idClienteParaTeste);

            #region Verificação do teste

            _mocker.Verify<IClientRepository>(r => r.Update(It.IsAny<Client>()), Times.Never);
            _mocker.Verify<IClientRepository>(r => r.SaveChangesAsync(), Times.Never);
            _mocker.Verify<IEmailService>(e => e.Send(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
            _mocker.Verify<INotificador>(n => n.AddMensagem(ClientValidationMessages.ClientNotActive), Times.Once);

            #endregion
        }
    }
}

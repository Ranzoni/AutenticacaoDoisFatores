using AutenticacaoDoisFatores.Dominio.Entidades;
using AutenticacaoDoisFatores.Dominio.Repositorios;
using AutenticacaoDoisFatores.Servico.CasosDeUso.Clientes;
using AutenticacaoDoisFatores.Testes.Compartilhados;
using AutenticacaoDoisFatores.Dominio.Compartilhados.Mensagens;
using Mensageiro;
using Moq;
using Moq.AutoMock;

namespace AutenticacaoDoisFatores.Testes.Servico.Clientes
{
    public class AtivarClienteTeste
    {
        private readonly AutoMocker _mocker = new();

        [Fact]
        internal async Task DeveAtivarCliente()
        {
            #region Preparação do teste

            var cliente = ConstrutorDeClientesTeste.RetornarConstrutor(ativo: false).Build();

            _mocker.GetMock<IClientRepository>().Setup(r => r.BuscarUnicoAsync(cliente.Id)).ReturnsAsync(cliente);

            var servico = _mocker.CreateInstance<ActivateClient>();

            #endregion

            await servico.AtivarAsync(cliente.Id);

            #region Verificação do teste

            Assert.True(cliente.Ativo);
            _mocker.Verify<IClientRepository>(r => r.Editar(cliente), Times.Once);
            _mocker.Verify<IClientRepository>(r => r.SaveChangesAsync(), Times.Once);

            #endregion
        }

        [Fact]
        internal async Task NaoDeveAtivarClienteQuandoIdClienteNaoExiste()
        {
            #region Preparação do teste

            var idInexistente = Guid.NewGuid();

            _mocker.GetMock<INotificador>().Setup(n => n.ExisteMensagem()).Returns(true);

            var servico = _mocker.CreateInstance<ActivateClient>();

            #endregion

            await servico.ActivateAsync(idInexistente);

            #region Verificação do teste

            _mocker.Verify<IClientRepository>(r => r.Update(It.IsAny<Client>()), Times.Never);
            _mocker.Verify<IClientRepository>(r => r.SaveChangesAsync(), Times.Never);
            _mocker.Verify<INotificador>(n => n.AddMensagemNaoEncontrado(ClientValidationMessages.ClientNotFound), Times.Once);

            #endregion
        }

        [Fact]
        internal async Task NaoDeveAtivarClienteQuandoClienteEstaAtivado()
        {
            #region Preparação do teste

            var cliente = ConstrutorDeClientesTeste.RetornarConstrutor(ativo: true).Build();

            _mocker.GetMock<IClientRepository>().Setup(r => r.BuscarUnicoAsync(cliente.Id)).ReturnsAsync(cliente);
            _mocker.GetMock<INotificador>().Setup(n => n.ExisteMensagem()).Returns(true);

            var servico = _mocker.CreateInstance<ActivateClient>();

            #endregion

            await servico.AtivarAsync(cliente.Id);

            #region Verificação do teste

            _mocker.Verify<IClientRepository>(r => r.Update(It.IsAny<Client>()), Times.Never);
            _mocker.Verify<IClientRepository>(r => r.SaveChangesAsync(), Times.Never);
            _mocker.Verify<INotificador>(n => n.AddMensagem(ClientValidationMessages.ClientAlreadyActivated), Times.Once);

            #endregion
        }
    }
}

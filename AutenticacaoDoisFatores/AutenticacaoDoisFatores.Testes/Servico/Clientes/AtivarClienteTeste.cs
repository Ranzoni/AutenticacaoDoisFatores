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

            var cliente = ConstrutorDeClientesTeste.RetornarConstrutor(ativo: false).ConstruirCadastrado();

            _mocker.GetMock<IRepositorioDeClientes>().Setup(r => r.BuscarUnicoAsync(cliente.Id)).ReturnsAsync(cliente);

            var servico = _mocker.CreateInstance<AtivarCliente>();

            #endregion

            await servico.AtivarAsync(cliente.Id);

            #region Verificação do teste

            Assert.True(cliente.Ativo);
            _mocker.Verify<IRepositorioDeClientes>(r => r.Editar(cliente), Times.Once);
            _mocker.Verify<IRepositorioDeClientes>(r => r.SalvarAlteracoesAsync(), Times.Once);

            #endregion
        }

        [Fact]
        internal async Task NaoDeveAtivarClienteQuandoIdClienteNaoExiste()
        {
            #region Preparação do teste

            var idInexistente = Guid.NewGuid();

            _mocker.GetMock<INotificador>().Setup(n => n.ExisteMensagem()).Returns(true);

            var servico = _mocker.CreateInstance<AtivarCliente>();

            #endregion

            await servico.AtivarAsync(idInexistente);

            #region Verificação do teste

            _mocker.Verify<IRepositorioDeClientes>(r => r.Editar(It.IsAny<Cliente>()), Times.Never);
            _mocker.Verify<IRepositorioDeClientes>(r => r.SalvarAlteracoesAsync(), Times.Never);
            _mocker.Verify<INotificador>(n => n.AddMensagemNaoEncontrado(MensagensValidacaoCliente.ClienteNaoEncontrado), Times.Once);

            #endregion
        }

        [Fact]
        internal async Task NaoDeveAtivarClienteQuandoClienteEstaAtivado()
        {
            #region Preparação do teste

            var cliente = ConstrutorDeClientesTeste.RetornarConstrutor(ativo: true).ConstruirCadastrado();

            _mocker.GetMock<IRepositorioDeClientes>().Setup(r => r.BuscarUnicoAsync(cliente.Id)).ReturnsAsync(cliente);
            _mocker.GetMock<INotificador>().Setup(n => n.ExisteMensagem()).Returns(true);

            var servico = _mocker.CreateInstance<AtivarCliente>();

            #endregion

            await servico.AtivarAsync(cliente.Id);

            #region Verificação do teste

            _mocker.Verify<IRepositorioDeClientes>(r => r.Editar(It.IsAny<Cliente>()), Times.Never);
            _mocker.Verify<IRepositorioDeClientes>(r => r.SalvarAlteracoesAsync(), Times.Never);
            _mocker.Verify<INotificador>(n => n.AddMensagem(MensagensValidacaoCliente.ClienteJaAtivado), Times.Once);

            #endregion
        }
    }
}

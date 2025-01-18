using AutenticacaoDoisFatores.Dominio.Entidades;
using AutenticacaoDoisFatores.Dominio.Repositorios;
using AutenticacaoDoisFatores.Servico.CasosDeUso.Clientes;
using AutenticacaoDoisFatores.Servico.Compartilhados;
using AutenticacaoDoisFatores.Testes.Compartilhados;
using AutenticacaoDoisFatores.Dominio.Compartilhados.Mensagens;
using Mensageiro;
using Moq;
using Moq.AutoMock;
using AutenticacaoDoisFatores.Dominio.Compartilhados;

namespace AutenticacaoDoisFatores.Testes.Servico
{
    public class AtivarClienteTeste
    {
        private readonly AutoMocker _mocker = new();

        [Fact]
        internal async Task DeveAtivarCliente()
        {
            #region Preparação do teste

            var cliente = ConstrutorDeClientesTeste.RetornarConstrutorDeCliente(ativo: false).ConstruirClienteCadastrado();
            var token = Seguranca.GerarTokenDeConfirmacaoDeCliente(cliente.Id);

            _mocker.GetMock<IRepositorioDeClientes>().Setup(r => r.BuscarUnicoAsync(cliente.Id)).ReturnsAsync(cliente);

            var servico = _mocker.CreateInstance<AtivarCliente>();

            #endregion

            await servico.AtivarAsync(token);

            #region Verificação do teste

            Assert.True(cliente.Ativo);
            _mocker.Verify<IRepositorioDeClientes>(r => r.Editar(cliente), Times.Once);
            _mocker.Verify<IRepositorioDeClientes>(r => r.SalvarAlteracoesAsync(), Times.Once);

            #endregion
        }

        [Fact]
        internal async Task NaoDeveAtivarClienteQuandoIdClienteNaoEstaNoToken()
        {
            #region Preparação do teste

            var cliente = ConstrutorDeClientesTeste.RetornarConstrutorDeCliente(ativo: false).ConstruirClienteCadastrado();
            var token = Seguranca.GerarTokenDeConfirmacaoDeCliente(Guid.Empty);

            _mocker.GetMock<INotificador>().Setup(n => n.ExisteMensagem()).Returns(true);

            var servico = _mocker.CreateInstance<AtivarCliente>();

            #endregion

            await servico.AtivarAsync(token);

            #region Verificação do teste

            Assert.False(cliente.Ativo);
            _mocker.Verify<IRepositorioDeClientes>(r => r.Editar(It.IsAny<Cliente>()), Times.Never);
            _mocker.Verify<IRepositorioDeClientes>(r => r.SalvarAlteracoesAsync(), Times.Never);
            _mocker.Verify<INotificador>(n => n.AddMensagem(MensagensValidacaoCliente.TokenInvalido), Times.Once);

            #endregion
        }

        [Fact]
        internal async Task NaoDeveAtivarClienteQuandoIdClienteNaoExiste()
        {
            #region Preparação do teste

            var token = Seguranca.GerarTokenDeConfirmacaoDeCliente(Guid.NewGuid());

            var notificador = new Notificador();
            _mocker.Use<INotificador>(notificador);

            var servico = _mocker.CreateInstance<AtivarCliente>();

            #endregion

            await servico.AtivarAsync(token);

            #region Verificação do teste

            _mocker.Verify<IRepositorioDeClientes>(r => r.Editar(It.IsAny<Cliente>()), Times.Never);
            _mocker.Verify<IRepositorioDeClientes>(r => r.SalvarAlteracoesAsync(), Times.Never);
            Assert.True(notificador.ExisteMsgNaoEncontrado());
            Assert.Equal(MensagensValidacaoCliente.ClienteNaoEncontrado.Descricao(), notificador.Mensagens().First());

            #endregion
        }

        [Fact]
        internal async Task NaoDeveAtivarClienteQuandoClienteEstaAtivado()
        {
            #region Preparação do teste

            var cliente = ConstrutorDeClientesTeste.RetornarConstrutorDeCliente(ativo: true).ConstruirClienteCadastrado();
            var token = Seguranca.GerarTokenDeConfirmacaoDeCliente(cliente.Id);

            var notificador = new Notificador();
            _mocker.Use<INotificador>(notificador);
            _mocker.GetMock<IRepositorioDeClientes>().Setup(r => r.BuscarUnicoAsync(cliente.Id)).ReturnsAsync(cliente);

            var servico = _mocker.CreateInstance<AtivarCliente>();

            #endregion

            await servico.AtivarAsync(token);

            #region Verificação do teste

            _mocker.Verify<IRepositorioDeClientes>(r => r.Editar(It.IsAny<Cliente>()), Times.Never);
            _mocker.Verify<IRepositorioDeClientes>(r => r.SalvarAlteracoesAsync(), Times.Never);
            Assert.True(notificador.ExisteMensagem());
            Assert.Equal(MensagensValidacaoCliente.ClienteJaAtivado.Descricao(), notificador.Mensagens().First());

            #endregion
        }
    }
}

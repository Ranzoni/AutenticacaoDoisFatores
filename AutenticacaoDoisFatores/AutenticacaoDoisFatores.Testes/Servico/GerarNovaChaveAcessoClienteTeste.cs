using AutenticacaoDoisFatores.Dominio.Compartilhados.Mensagens;
using AutenticacaoDoisFatores.Dominio.Entidades;
using AutenticacaoDoisFatores.Dominio.Repositorios;
using AutenticacaoDoisFatores.Dominio.Servicos;
using AutenticacaoDoisFatores.Servico.CasosDeUso.Clientes;
using AutenticacaoDoisFatores.Testes.Compartilhados;
using Mensageiro;
using Moq;
using Moq.AutoMock;

namespace AutenticacaoDoisFatores.Testes.Servico
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
                .ConstruirCadastrado();

            _mocker.GetMock<IRepositorioDeClientes>().Setup(r => r.BuscarUnicoAsync(idClienteParaTeste)).ReturnsAsync(cliente);

            #endregion

            await servico.GerarNovaChaveAsync(idClienteParaTeste);

            #region Verificação do teste

            _mocker.Verify<IRepositorioDeClientes>(r => r.Editar(cliente), Times.Once);
            _mocker.Verify<IRepositorioDeClientes>(r => r.SalvarAlteracoesAsync(), Times.Once);
            _mocker.Verify<IServicoDeEmail>(e => e.Enviar(cliente.Email, It.IsAny<string>(), It.IsAny<string>()), Times.Once);

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

            _mocker.Verify<IRepositorioDeClientes>(r => r.Editar(It.IsAny<Cliente>()), Times.Never);
            _mocker.Verify<IRepositorioDeClientes>(r => r.SalvarAlteracoesAsync(), Times.Never);
            _mocker.Verify<IServicoDeEmail>(e => e.Enviar(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
            _mocker.Verify<INotificador>(n => n.AddMensagemNaoEncontrado(MensagensValidacaoCliente.ClienteNaoEncontrado), Times.Once);

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
                .ConstruirCadastrado();

            _mocker.GetMock<IRepositorioDeClientes>().Setup(r => r.BuscarUnicoAsync(idClienteParaTeste)).ReturnsAsync(cliente);
            _mocker.GetMock<INotificador>().Setup(n => n.ExisteMensagem()).Returns(true);

            #endregion

            await servico.GerarNovaChaveAsync(idClienteParaTeste);

            #region Verificação do teste

            _mocker.Verify<IRepositorioDeClientes>(r => r.Editar(It.IsAny<Cliente>()), Times.Never);
            _mocker.Verify<IRepositorioDeClientes>(r => r.SalvarAlteracoesAsync(), Times.Never);
            _mocker.Verify<IServicoDeEmail>(e => e.Enviar(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
            _mocker.Verify<INotificador>(n => n.AddMensagem(MensagensValidacaoCliente.ClienteNaoAtivo), Times.Once);

            #endregion
        }
    }
}

using AutenticacaoDoisFatores.Dominio.Compartilhados.Mensagens;
using AutenticacaoDoisFatores.Dominio.Entidades;
using AutenticacaoDoisFatores.Dominio.Repositorios;
using AutenticacaoDoisFatores.Servico.CasosDeUso.Usuarios;
using AutenticacaoDoisFatores.Testes.Compartilhados;
using Mensageiro;
using Moq;
using Moq.AutoMock;

namespace AutenticacaoDoisFatores.Testes.Servico.Usuarios
{
    public class AtivarUsuarioTeste
    {
        private readonly AutoMocker _mocker = new();

        [Fact]
        internal async Task DeveAtivarUsuario()
        {
            #region Preparação do teste

            var idUsuario = Guid.NewGuid();

            var usuarioCadastradoTeste = ConstrutorDeUsuariosTeste
                .RetornarConstrutor(id: idUsuario, ativo: false)
                .ConstruirCadastrado();

            var servico = _mocker.CreateInstance<AtivarUsuario>();

            _mocker.GetMock<IRepositorioDeUsuarios>().Setup(r => r.BuscarUnicoAsync(idUsuario)).ReturnsAsync(usuarioCadastradoTeste);

            #endregion

            await servico.AtivarAsync(idUsuario);

            #region Verificação do teste

            Assert.True(usuarioCadastradoTeste.Ativo);
            _mocker.Verify<IRepositorioDeUsuarios>(r => r.Editar(usuarioCadastradoTeste), Times.Once);
            _mocker.Verify<IRepositorioDeUsuarios>(r => r.SalvarAlteracoesAsync(), Times.Once);

            #endregion
        }

        [Fact]
        internal async Task NaoDeveAtivarUsuarioQuandoUsuarioNaoExiste()
        {
            #region Preparação do teste

            var idUsuario = Guid.NewGuid();

            var servico = _mocker.CreateInstance<AtivarUsuario>();

            #endregion

            await servico.AtivarAsync(idUsuario);

            #region Verificação do teste

            _mocker.Verify<IRepositorioDeUsuarios>(r => r.Editar(It.IsAny<Usuario>()), Times.Never);
            _mocker.Verify<IRepositorioDeUsuarios>(r => r.SalvarAlteracoesAsync(), Times.Never);
            _mocker.Verify<INotificador>(n => n.AddMensagemNaoEncontrado(MensagensValidacaoUsuario.UsuarioNaoEncontrado), Times.Once);

            #endregion
        }

        [Fact]
        internal async Task NaoDeveAtivarUsuarioQuandoJaAtivo()
        {
            #region Preparação do teste

            var idUsuario = Guid.NewGuid();

            var usuarioCadastradoTeste = ConstrutorDeUsuariosTeste
                .RetornarConstrutor(id: idUsuario, ativo: true)
                .ConstruirCadastrado();

            var servico = _mocker.CreateInstance<AtivarUsuario>();

            _mocker.GetMock<IRepositorioDeUsuarios>().Setup(r => r.BuscarUnicoAsync(idUsuario)).ReturnsAsync(usuarioCadastradoTeste);
            _mocker.GetMock<INotificador>().Setup(n => n.ExisteMensagem()).Returns(true);

            #endregion

            await servico.AtivarAsync(idUsuario);

            #region Verificação do teste

            _mocker.Verify<IRepositorioDeUsuarios>(r => r.Editar(It.IsAny<Usuario>()), Times.Never);
            _mocker.Verify<IRepositorioDeUsuarios>(r => r.SalvarAlteracoesAsync(), Times.Never);
            _mocker.Verify<INotificador>(n => n.AddMensagem(MensagensValidacaoUsuario.UsuarioJaAtivado), Times.Once);

            #endregion
        }
    }
}

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

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        internal async Task DeveAtivarUsuario(bool ativar)
        {
            #region Preparação do teste

            var idUsuario = Guid.NewGuid();

            var usuarioCadastradoTeste = ConstrutorDeUsuariosTeste
                .RetornarConstrutor(id: idUsuario, ativo: false)
                .ConstruirCadastrado();

            var servico = _mocker.CreateInstance<AtivarUsuario>();

            _mocker.GetMock<IRepositorioDeUsuarios>().Setup(r => r.BuscarUnicoAsync(idUsuario)).ReturnsAsync(usuarioCadastradoTeste);

            #endregion

            await servico.AtivarAsync(idUsuario, ativar);

            #region Verificação do teste

            Assert.Equal(ativar, usuarioCadastradoTeste.Ativo);
            _mocker.Verify<IRepositorioDeUsuarios>(r => r.Editar(usuarioCadastradoTeste), Times.Once);
            _mocker.Verify<IRepositorioDeUsuarios>(r => r.SalvarAlteracoesAsync(), Times.Once);

            #endregion
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        internal async Task NaoDeveAtivarUsuarioQuandoUsuarioNaoExiste(bool ativar)
        {
            #region Preparação do teste

            var idUsuario = Guid.NewGuid();

            var servico = _mocker.CreateInstance<AtivarUsuario>();

            #endregion

            await servico.AtivarAsync(idUsuario, ativar);

            #region Verificação do teste

            _mocker.Verify<IRepositorioDeUsuarios>(r => r.Editar(It.IsAny<Usuario>()), Times.Never);
            _mocker.Verify<IRepositorioDeUsuarios>(r => r.SalvarAlteracoesAsync(), Times.Never);
            _mocker.Verify<INotificador>(n => n.AddMensagemNaoEncontrado(MensagensValidacaoUsuario.UsuarioNaoEncontrado), Times.Once);

            #endregion
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        internal async Task NaoDeveAtivarUsuarioQuandoUsuarioEhAdm(bool ativar)
        {
            #region Preparação do teste

            var idUsuario = Guid.NewGuid();

            var usuarioCadastradoTeste = ConstrutorDeUsuariosTeste
                .RetornarConstrutor(id: idUsuario, ativo: true, ehAdm: true)
                .ConstruirCadastrado();

            var servico = _mocker.CreateInstance<AtivarUsuario>();

            _mocker.GetMock<IRepositorioDeUsuarios>().Setup(r => r.BuscarUnicoAsync(idUsuario)).ReturnsAsync(usuarioCadastradoTeste);

            #endregion

            await servico.AtivarAsync(idUsuario, ativar);

            #region Verificação do teste

            _mocker.Verify<IRepositorioDeUsuarios>(r => r.Editar(It.IsAny<Usuario>()), Times.Never);
            _mocker.Verify<IRepositorioDeUsuarios>(r => r.SalvarAlteracoesAsync(), Times.Never);
            _mocker.Verify<INotificador>(n => n.AddMensagemNaoEncontrado(MensagensValidacaoUsuario.UsuarioNaoEncontrado), Times.Once);

            #endregion
        }
    }
}

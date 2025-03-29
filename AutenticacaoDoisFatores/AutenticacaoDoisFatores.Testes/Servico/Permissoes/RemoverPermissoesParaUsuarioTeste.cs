using AutenticacaoDoisFatores.Dominio.Compartilhados.Mensagens;
using AutenticacaoDoisFatores.Dominio.Compartilhados.Permissoes;
using AutenticacaoDoisFatores.Dominio.Repositorios;
using AutenticacaoDoisFatores.Servico.CasosDeUso.Permissoes;
using AutenticacaoDoisFatores.Testes.Compartilhados;
using Mensageiro;
using Moq;
using Moq.AutoMock;

namespace AutenticacaoDoisFatores.Testes.Servico.Permissoes
{
    public class RemoverPermissoesParaUsuarioTeste
    {
        private readonly AutoMocker _mocker = new();

        [Fact]
        internal async Task DeveRemoverPermissoesParaUsuarioAtivo()
        {
            #region Preparação do teste

            var servico = _mocker.CreateInstance<RemoverPermissoesParaUsuario>();

            var idUsuario = Guid.NewGuid();
            var usuario = ConstrutorDeUsuariosTeste
                .RetornarConstrutor(ativo: true, id: idUsuario)
                .ConstruirCadastrado();
            var permissoesParaExcluir = new List<TipoDePermissao>()
            {
                TipoDePermissao.AtivarUsuario,
                TipoDePermissao.DesativarUsuario
            };
            var permissoesInclusas = new List<TipoDePermissao>
            {
                TipoDePermissao.CriarUsuario,
                TipoDePermissao.AtivarUsuario,
                TipoDePermissao.DesativarUsuario
            };
            var permissoesEsperadas = permissoesInclusas.Except(permissoesParaExcluir);

            _mocker.GetMock<IRepositorioDeUsuarios>().Setup(r => r.BuscarUnicoAsync(idUsuario)).ReturnsAsync(usuario);
            _mocker.GetMock<IRepositorioDePermissoes>().Setup(r => r.RetornarPorUsuarioAsync(idUsuario)).ReturnsAsync(permissoesInclusas);

            #endregion

            await servico.ExecutarAsync(idUsuario, permissoesParaExcluir);

            #region Verificação do teste

            _mocker.Verify<IRepositorioDePermissoes>(r => r.EditarAsync(idUsuario, permissoesEsperadas), Times.Once);

            #endregion
        }

        [Fact]
        internal async Task NaoDeveRemoverPermissoesParaUsuarioAdmin()
        {
            #region Preparação do teste

            var servico = _mocker.CreateInstance<RemoverPermissoesParaUsuario>();

            var idUsuario = Guid.NewGuid();
            var usuario = ConstrutorDeUsuariosTeste
                .RetornarConstrutor(ativo: true, ehAdm: true, id: idUsuario)
                .ConstruirCadastrado();
            var permissoesParaExcluir = new List<TipoDePermissao>()
            {
                TipoDePermissao.AtivarUsuario,
                TipoDePermissao.DesativarUsuario
            };

            _mocker.GetMock<IRepositorioDeUsuarios>().Setup(r => r.BuscarUnicoAsync(idUsuario)).ReturnsAsync(usuario);

            #endregion

            await servico.ExecutarAsync(idUsuario, permissoesParaExcluir);

            #region Verificação do teste

            _mocker.Verify<IRepositorioDePermissoes>(r => r.EditarAsync(It.IsAny<Guid>(), It.IsAny<IEnumerable<TipoDePermissao>>()), Times.Never);
            _mocker.Verify<INotificador>(n => n.AddMensagemNaoEncontrado(MensagensValidacaoUsuario.UsuarioNaoEncontrado), Times.Once);

            #endregion
        }

        [Fact]
        internal async Task NaoDeveRemoverPermissoesParaUsuarioInativo()
        {
            #region Preparação do teste

            var servico = _mocker.CreateInstance<RemoverPermissoesParaUsuario>();

            var idUsuario = Guid.NewGuid();
            var usuario = ConstrutorDeUsuariosTeste
                .RetornarConstrutor(ativo: false, id: idUsuario)
                .ConstruirCadastrado();
            var permissoesParaExcluir = new List<TipoDePermissao>()
            {
                TipoDePermissao.AtivarUsuario,
                TipoDePermissao.DesativarUsuario
            };

            _mocker.GetMock<IRepositorioDeUsuarios>().Setup(r => r.BuscarUnicoAsync(idUsuario)).ReturnsAsync(usuario);

            #endregion

            await servico.ExecutarAsync(idUsuario, permissoesParaExcluir);

            #region Verificação do teste

            _mocker.Verify<IRepositorioDePermissoes>(r => r.EditarAsync(It.IsAny<Guid>(), It.IsAny<IEnumerable<TipoDePermissao>>()), Times.Never);
            _mocker.Verify<INotificador>(n => n.AddMensagemNaoEncontrado(MensagensValidacaoUsuario.UsuarioNaoEncontrado), Times.Once);

            #endregion
        }

        [Fact]
        internal async Task NaoDeveRemoverPermissoesParaUsuarioNaoExistente()
        {
            #region Preparação do teste

            var servico = _mocker.CreateInstance<RemoverPermissoesParaUsuario>();

            var idUsuario = Guid.NewGuid();
            var permissoesParaExcluir = new List<TipoDePermissao>()
            {
                TipoDePermissao.AtivarUsuario,
                TipoDePermissao.DesativarUsuario
            };

            #endregion

            await servico.ExecutarAsync(idUsuario, permissoesParaExcluir);

            #region Verificação do teste

            _mocker.Verify<IRepositorioDePermissoes>(r => r.EditarAsync(It.IsAny<Guid>(), It.IsAny<IEnumerable<TipoDePermissao>>()), Times.Never);
            _mocker.Verify<INotificador>(n => n.AddMensagemNaoEncontrado(MensagensValidacaoUsuario.UsuarioNaoEncontrado), Times.Once);

            #endregion
        }
    }
}

using AutenticacaoDoisFatores.Dominio.Compartilhados.Mensagens;
using AutenticacaoDoisFatores.Dominio.Compartilhados.Permissoes;
using AutenticacaoDoisFatores.Dominio.Repositorios;
using AutenticacaoDoisFatores.Servico.CasosDeUso.Permissoes;
using AutenticacaoDoisFatores.Testes.Compartilhados;
using Bogus;
using Mensageiro;
using Moq;
using Moq.AutoMock;

namespace AutenticacaoDoisFatores.Testes.Servico.Permissoes
{
    public class IncluirPermissoesParaUsuarioTeste
    {
        private readonly AutoMocker _mocker = new();
        private readonly Faker _faker = new();

        [Fact]
        internal async Task DeveIncluirPermissoes()
        {
            #region Preparação do teste

            var idUsuario = Guid.NewGuid();
            var permissoesParaIncluir = _faker.Random.EnumValues<TipoDePermissao>();

            var usuario = ConstrutorDeUsuariosTeste
                .RetornarConstrutor(id: idUsuario, ativo: true)
                .ConstruirCadastrado();

            var servico = _mocker.CreateInstance<IncluirPermissoesParaUsuario>();

            _mocker.GetMock<IRepositorioDeUsuarios>().Setup(r => r.BuscarUnicoAsync(idUsuario)).ReturnsAsync(usuario);

            #endregion

            await servico.ExecutarAsync(idUsuario, permissoesParaIncluir);

            _mocker.Verify<IRepositorioDePermissoes>(r => r.AdicionarAsync(idUsuario, permissoesParaIncluir), Times.Once);
        }

        [Fact]
        internal async Task NaoDeveIncluirPermissoesQuandoUsuarioNaoExiste()
        {
            #region Preparação do teste

            var idUsuario = Guid.NewGuid();
            var permissoesParaIncluir = _faker.Random.EnumValues<TipoDePermissao>();

            var servico = _mocker.CreateInstance<IncluirPermissoesParaUsuario>();

            #endregion

            await servico.ExecutarAsync(idUsuario, permissoesParaIncluir);

            #region Verificação do teste

            _mocker.Verify<IRepositorioDePermissoes>(r => r.AdicionarAsync(It.IsAny<Guid>(), It.IsAny<IEnumerable<TipoDePermissao>>()), Times.Never);
            _mocker.Verify<IRepositorioDePermissoes>(r => r.EditarAsync(It.IsAny<Guid>(), It.IsAny<IEnumerable<TipoDePermissao>>()), Times.Never);
            _mocker.Verify<INotificador>(n => n.AddMensagemNaoEncontrado(MensagensValidacaoUsuario.UsuarioNaoEncontrado), Times.Once);

            #endregion
        }

        [Fact]
        internal async Task NaoDeveIncluirPermissoesQuandoUsuarioInativo()
        {
            #region Preparação do teste

            var idUsuario = Guid.NewGuid();
            var permissoesParaIncluir = _faker.Random.EnumValues<TipoDePermissao>();

            var usuario = ConstrutorDeUsuariosTeste
                .RetornarConstrutor(id: idUsuario, ativo: false)
                .ConstruirCadastrado();

            var servico = _mocker.CreateInstance<IncluirPermissoesParaUsuario>();

            _mocker.GetMock<IRepositorioDeUsuarios>().Setup(r => r.BuscarUnicoAsync(idUsuario)).ReturnsAsync(usuario);

            #endregion

            await servico.ExecutarAsync(idUsuario, permissoesParaIncluir);

            #region Verificação do teste

            _mocker.Verify<IRepositorioDePermissoes>(r => r.AdicionarAsync(It.IsAny<Guid>(), It.IsAny<IEnumerable<TipoDePermissao>>()), Times.Never);
            _mocker.Verify<IRepositorioDePermissoes>(r => r.EditarAsync(It.IsAny<Guid>(), It.IsAny<IEnumerable<TipoDePermissao>>()), Times.Never);
            _mocker.Verify<INotificador>(n => n.AddMensagemNaoEncontrado(MensagensValidacaoUsuario.UsuarioNaoEncontrado), Times.Once);

            #endregion
        }

        [Fact]
        internal async Task NaoDeveIncluirPermissoesQuandoUsuarioEhAdm()
        {
            #region Preparação do teste

            var idUsuario = Guid.NewGuid();
            var permissoesParaIncluir = _faker.Random.EnumValues<TipoDePermissao>();

            var usuario = ConstrutorDeUsuariosTeste
                .RetornarConstrutor(id: idUsuario, ativo: true, ehAdm: true)
                .ConstruirCadastrado();

            var servico = _mocker.CreateInstance<IncluirPermissoesParaUsuario>();

            _mocker.GetMock<IRepositorioDeUsuarios>().Setup(r => r.BuscarUnicoAsync(idUsuario)).ReturnsAsync(usuario);

            #endregion

            await servico.ExecutarAsync(idUsuario, permissoesParaIncluir);

            #region Verificação do teste

            _mocker.Verify<IRepositorioDePermissoes>(r => r.AdicionarAsync(It.IsAny<Guid>(), It.IsAny<IEnumerable<TipoDePermissao>>()), Times.Never);
            _mocker.Verify<INotificador>(n => n.AddMensagemNaoEncontrado(MensagensValidacaoUsuario.UsuarioNaoEncontrado), Times.Once);

            #endregion
        }
    }
}

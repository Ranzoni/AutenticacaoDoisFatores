using AutenticacaoDoisFatores.Dominio.Compartilhados.Permissoes;
using AutenticacaoDoisFatores.Dominio.Dominios;
using AutenticacaoDoisFatores.Dominio.Repositorios;
using Bogus;
using Moq;
using Moq.AutoMock;

namespace AutenticacaoDoisFatores.Testes.Dominio.Dominios
{
    public class DominioDePermissoesTeste
    {
        private static readonly AutoMocker _mocker = new();
        private static readonly Faker _faker = new();

        [Fact]
        internal async Task DeveAdicionarPermissao()
        {
            #region Preparação do teste

            var dominio = _mocker.CreateInstance<DominioDePermissoes>();

            var idUsuario = Guid.NewGuid();
            var permissoes = _faker.Random.EnumValues<TipoDePermissao>();

            #endregion

            await dominio.AdicionarAsync(idUsuario, permissoes);

            #region Verificação do teste

            _mocker.Verify<IRepositorioDePermissoes>(r => r.AdicionarAsync(idUsuario, permissoes), Times.Once);

            #endregion
        }

        [Fact]
        internal async Task DeveRetornarPermissoesDoUsuario()
        {
            #region Preparação do teste

            var idUsuario = Guid.NewGuid();
            var permissoes = _faker.Random.EnumValues<TipoDePermissao>();

            var dominio = _mocker.CreateInstance<DominioDePermissoes>();
            _mocker.GetMock<IRepositorioDePermissoes>().Setup(r => r.RetornarPorUsuarioAsync(idUsuario)).ReturnsAsync(permissoes);

            #endregion

            var retorno = await dominio.RetornarPermissoesAsync(idUsuario);

            #region Verificação do teste

            Assert.Equal(permissoes, retorno);
            _mocker.Verify<IRepositorioDePermissoes>(r => r.RetornarPorUsuarioAsync(idUsuario), Times.Once);

            #endregion
        }

        [Fact]
        internal void DeveRetornarTodasPermissoesDoUsuario()
        {
            var permissoesEsperadas = Enum.GetValues<TipoDePermissao>();

            var retorno = DominioDePermissoes.RetornarTodasPermissoes();

            Assert.Equal(permissoesEsperadas, retorno);
        }

        [Fact]
        internal async Task DeveEditarPermissoes()
        {
            #region Preparação do teste

            var dominio = _mocker.CreateInstance<DominioDePermissoes>();

            var idUsuario = Guid.NewGuid();
            var permissoesParaIncluir = new List<TipoDePermissao>()
            {
                TipoDePermissao.TrocarSenhaUsuario
            };
            var permissoesInclusas = new List<TipoDePermissao>
            {
                TipoDePermissao.AtivarUsuario,
                TipoDePermissao.DesativarUsuario
            };

            var permissoesEsperadas = permissoesInclusas.Concat(permissoesParaIncluir);

            _mocker.GetMock<IRepositorioDePermissoes>().Setup(r => r.RetornarPorUsuarioAsync(idUsuario)).ReturnsAsync(permissoesInclusas);

            #endregion

            await dominio.AdicionarAsync(idUsuario, permissoesParaIncluir);

            #region Verificação do teste

            _mocker.Verify<IRepositorioDePermissoes>(r => r.EditarAsync(idUsuario, permissoesEsperadas), Times.Once);

            #endregion
        }

        [Fact]
        internal async Task DeveRemoverPermissoes()
        {
            #region Preparação do teste

            var dominio = _mocker.CreateInstance<DominioDePermissoes>();

            var idUsuario = Guid.NewGuid();
            var permissoesParaExcluir = new List<TipoDePermissao>()
            {
                TipoDePermissao.AtivarUsuario,
                TipoDePermissao.DesativarUsuario
            };
            var permissoesInclusas = new List<TipoDePermissao>
            {
                TipoDePermissao.TrocarSenhaUsuario,
                TipoDePermissao.AtivarUsuario,
                TipoDePermissao.DesativarUsuario
            };
            var permissoesEsperadas = permissoesInclusas.Except(permissoesParaExcluir);

            _mocker.GetMock<IRepositorioDePermissoes>().Setup(r => r.RetornarPorUsuarioAsync(idUsuario)).ReturnsAsync(permissoesInclusas);

            #endregion

            await dominio.RemoverAsync(idUsuario, permissoesParaExcluir);

            #region Verificação do teste

            _mocker.Verify<IRepositorioDePermissoes>(r => r.EditarAsync(idUsuario, permissoesEsperadas), Times.Once);

            #endregion
        }

        [Fact]
        internal async Task NaoDeveRemoverQuandoNaoHouverPermissoes()
        {
            #region Preparação do teste

            var dominio = _mocker.CreateInstance<DominioDePermissoes>();

            var idUsuario = Guid.NewGuid();
            var permissoesParaExcluir = new List<TipoDePermissao>()
            {
                TipoDePermissao.AtivarUsuario,
                TipoDePermissao.DesativarUsuario
            };

            #endregion

            await dominio.RemoverAsync(idUsuario, permissoesParaExcluir);

            #region Verificação do teste

            _mocker.Verify<IRepositorioDePermissoes>(r => r.EditarAsync(It.IsAny<Guid>(), It.IsAny<IEnumerable<TipoDePermissao>>()), Times.Never);

            #endregion
        }
    }
}

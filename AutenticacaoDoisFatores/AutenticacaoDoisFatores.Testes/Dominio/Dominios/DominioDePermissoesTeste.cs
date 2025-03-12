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
            _mocker.GetMock<IRepositorioDeUsuarios>().Setup(r => r.EhAdmAsync(idUsuario)).ReturnsAsync(false);

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
        internal async Task DeveEditarPermissao()
        {
            #region Preparação do teste

            var dominio = _mocker.CreateInstance<DominioDePermissoes>();

            var idUsuario = Guid.NewGuid();
            var permissoes = _faker.Random.EnumValues<TipoDePermissao>();

            #endregion

            await dominio.EditarAsync(idUsuario, permissoes);

            #region Verificação do teste

            _mocker.Verify<IRepositorioDePermissoes>(r => r.EditarAsync(idUsuario, permissoes), Times.Once);

            #endregion
        }
    }
}

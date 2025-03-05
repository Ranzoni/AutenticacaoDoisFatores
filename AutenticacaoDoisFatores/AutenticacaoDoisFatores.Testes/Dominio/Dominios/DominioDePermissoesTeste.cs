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
            var permissoes = _faker.Random.EnumValues<TipoDePermissao>(1);

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
            _mocker.GetMock<IRepositorioDePermissoes>().Setup(r => r.RetornarPermissoes(idUsuario)).ReturnsAsync(permissoes);
            _mocker.GetMock<IRepositorioDeUsuarios>().Setup(r => r.EhAdm(idUsuario)).ReturnsAsync(false);

            #endregion

            var retorno = await dominio.RetornarPermissoes(idUsuario);

            #region Verificação do teste

            Assert.Equal(permissoes, retorno);
            _mocker.Verify<IRepositorioDePermissoes>(r => r.RetornarPermissoes(idUsuario), Times.Once);

            #endregion
        }

        [Fact]
        internal async Task DeveRetornarPermissoesDoUsuarioAdm()
        {
            #region Preparação do teste

            var idUsuario = Guid.NewGuid();
            var permissoesEsperadas = Enum.GetValues<TipoDePermissao>();

            var dominio = _mocker.CreateInstance<DominioDePermissoes>();
            _mocker.GetMock<IRepositorioDeUsuarios>().Setup(r => r.EhAdm(idUsuario)).ReturnsAsync(true);

            #endregion

            var retorno = await dominio.RetornarPermissoes(idUsuario);

            #region Verificação do teste

            Assert.Equal(permissoesEsperadas, retorno);
            _mocker.Verify<IRepositorioDePermissoes>(r => r.RetornarPermissoes(It.IsAny<Guid>()), Times.Never);

            #endregion
        }

        [Fact]
        internal void DeveRetornarTodasPermissoesDoUsuario()
        {
            var permissoesEsperadas = Enum.GetValues<TipoDePermissao>();

            var retorno = DominioDePermissoes.RetornarTodasPermissoes();

            Assert.Equal(permissoesEsperadas, retorno);
        }
    }
}

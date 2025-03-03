using AutenticacaoDoisFatores.Dominio.Compartilhados;
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
        internal async Task DeveRetornarPermissao()
        {
            #region Preparação do teste

            var idUsuario = Guid.NewGuid();
            var permissao = _faker.Random.Enum<TipoDePermissao>();
            var idPermissao = _faker.Random.AlphaNumeric(12);

            var dominio = _mocker.CreateInstance<DominioDePermissoes>();
            _mocker.GetMock<IRepositorioDePermissoes>().Setup(r => r.BuscarAsync(idUsuario, permissao)).ReturnsAsync(idPermissao);

            #endregion

            var retorno = await dominio.BuscarAsync(idUsuario, permissao);

            #region Verificação do teste

            Assert.NotNull(retorno);
            Assert.False(retorno.EstaVazio());
            Assert.Equal(idPermissao, retorno);
            _mocker.Verify<IRepositorioDePermissoes>(r => r.BuscarAsync(idUsuario, permissao), Times.Once);

            #endregion
        }
    }
}

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

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        internal async Task DeveRetornarSePossuiPermissao(bool valorEsperado)
        {
            #region Preparação do teste

            var idUsuario = Guid.NewGuid();
            var permissao = _faker.Random.Enum<TipoDePermissao>();

            var dominio = _mocker.CreateInstance<DominioDePermissoes>();
            _mocker.GetMock<IRepositorioDePermissoes>().Setup(r => r.ExistePermissaoAsync(idUsuario, permissao)).ReturnsAsync(valorEsperado);

            #endregion

            var retorno = await dominio.TemPermissaoAsync(idUsuario, permissao);

            #region Verificação do teste

            Assert.Equal(valorEsperado, retorno);
            _mocker.Verify<IRepositorioDePermissoes>(r => r.ExistePermissaoAsync(idUsuario, permissao), Times.Once);

            #endregion
        }
    }
}

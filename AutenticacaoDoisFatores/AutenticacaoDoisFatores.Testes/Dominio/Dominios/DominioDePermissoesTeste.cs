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

            var dominio = _mocker.CreateInstance<PermissionsDomain>();

            var idUsuario = Guid.NewGuid();
            var permissoes = _faker.Random.EnumValues<PermissionType>();

            #endregion

            await dominio.AdicionarAsync(idUsuario, permissoes);

            #region Verificação do teste

            _mocker.Verify<IPermissionsRepository>(r => r.AddAsync(idUsuario, permissoes), Times.Once);

            #endregion
        }

        [Fact]
        internal async Task DeveRetornarPermissoesDoUsuario()
        {
            #region Preparação do teste

            var idUsuario = Guid.NewGuid();
            var permissoes = _faker.Random.EnumValues<PermissionType>();

            var dominio = _mocker.CreateInstance<PermissionsDomain>();
            _mocker.GetMock<IPermissionsRepository>().Setup(r => r.GetByUserIdAsync(idUsuario)).ReturnsAsync(permissoes);

            #endregion

            var retorno = await dominio.GetByIdAsync(idUsuario);

            #region Verificação do teste

            Assert.Equal(permissoes, retorno);
            _mocker.Verify<IPermissionsRepository>(r => r.GetByUserIdAsync(idUsuario), Times.Once);

            #endregion
        }

        [Fact]
        internal void DeveRetornarTodasPermissoesDoUsuario()
        {
            var permissoesEsperadas = Enum.GetValues<PermissionType>();

            var retorno = PermissionsDomain.GetAll();

            Assert.Equal(permissoesEsperadas, retorno);
        }

        [Fact]
        internal async Task DeveEditarPermissoes()
        {
            #region Preparação do teste

            var dominio = _mocker.CreateInstance<PermissionsDomain>();

            var idUsuario = Guid.NewGuid();
            var permissoesParaIncluir = new List<PermissionType>()
            {
                PermissionType.CreateUser
            };
            var permissoesInclusas = new List<PermissionType>
            {
                PermissionType.ActivateUser,
                PermissionType.InactivateUser
            };

            var permissoesEsperadas = permissoesInclusas.Concat(permissoesParaIncluir);

            _mocker.GetMock<IPermissionsRepository>().Setup(r => r.GetByUserIdAsync(idUsuario)).ReturnsAsync(permissoesInclusas);

            #endregion

            await dominio.AdicionarAsync(idUsuario, permissoesParaIncluir);

            #region Verificação do teste

            _mocker.Verify<IPermissionsRepository>(r => r.UpdateAsync(idUsuario, permissoesEsperadas), Times.Once);

            #endregion
        }

        [Fact]
        internal async Task DeveRemoverPermissoes()
        {
            #region Preparação do teste

            var mocker = new AutoMocker();
            var dominio = mocker.CreateInstance<PermissionsDomain>();

            var idUsuario = Guid.NewGuid();
            var permissoesParaExcluir = new List<PermissionType>()
            {
                PermissionType.ActivateUser,
                PermissionType.InactivateUser
            };
            var permissoesInclusas = new List<PermissionType>
            {
                PermissionType.CreateUser,
                PermissionType.ActivateUser,
                PermissionType.InactivateUser
            };
            var permissoesEsperadas = permissoesInclusas.Except(permissoesParaExcluir);

            mocker.GetMock<IPermissionsRepository>().Setup(r => r.GetByUserIdAsync(idUsuario)).ReturnsAsync(permissoesInclusas);

            #endregion

            await dominio.RemoverAsync(idUsuario, permissoesParaExcluir);

            #region Verificação do teste

            mocker.Verify<IPermissionsRepository>(r => r.UpdateAsync(idUsuario, permissoesEsperadas), Times.Once);

            #endregion
        }

        [Fact]
        internal async Task NaoDeveRemoverQuandoNaoHouverPermissoes()
        {
            #region Preparação do teste

            var dominio = _mocker.CreateInstance<PermissionsDomain>();

            var idUsuario = Guid.NewGuid();
            var permissoesParaExcluir = new List<PermissionType>()
            {
                PermissionType.ActivateUser,
                PermissionType.InactivateUser
            };

            #endregion

            await dominio.RemoverAsync(idUsuario, permissoesParaExcluir);

            #region Verificação do teste

            _mocker.Verify<IPermissionsRepository>(r => r.UpdateAsync(It.IsAny<Guid>(), It.IsAny<IEnumerable<PermissionType>>()), Times.Never);

            #endregion
        }
    }
}

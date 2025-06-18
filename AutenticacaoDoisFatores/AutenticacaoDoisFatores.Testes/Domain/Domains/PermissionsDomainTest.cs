using AutenticacaoDoisFatores.Domain.Shared.Permissions;
using AutenticacaoDoisFatores.Domain.Domains;
using AutenticacaoDoisFatores.Domain.Repositories;
using Bogus;
using Moq;
using Moq.AutoMock;

namespace AutenticacaoDoisFatores.Tests.Domain.Domains
{
    public class PermissionsDomainTest
    {
        [Fact]
        internal async Task ShouldAdicionarPermissao()
        {
            #region Arrange

            var mocker = new AutoMocker();
            var faker = new Faker();

            var domain = mocker.CreateInstance<PermissionsDomain>();

            var userId = Guid.NewGuid();
            var permissions = faker.Random.EnumValues<PermissionType>();

            #endregion

            await domain.AddAsync(userId, permissions);

            #region Assert

            mocker.Verify<IPermissionRepository>(r => r.AddAsync(userId, permissions), Times.Once);

            #endregion
        }

        [Fact]
        internal async Task ShouldReturnUserPermissions()
        {
            #region Arrange

            var mocker = new AutoMocker();
            var faker = new Faker();

            var userId = Guid.NewGuid();
            var permissions = faker.Random.EnumValues<PermissionType>();

            var domain = mocker.CreateInstance<PermissionsDomain>();
            mocker.GetMock<IPermissionRepository>().Setup(r => r.GetByUserIdAsync(userId)).ReturnsAsync(permissions);

            #endregion

            var result = await domain.GetByUserIdAsync(userId);

            #region Assert

            Assert.Equal(permissions, result);
            mocker.Verify<IPermissionRepository>(r => r.GetByUserIdAsync(userId), Times.Once);

            #endregion
        }

        [Fact]
        internal void ShouldReturnAllPermissions()
        {
            var expectedPermissions = Enum.GetValues<PermissionType>();

            var result = PermissionsDomain.GetAll();

            Assert.Equal(expectedPermissions, result);
        }

        [Fact]
        internal async Task ShouldUpdatePermissions()
        {
            #region Arrange

            var mocker = new AutoMocker();

            var domain = mocker.CreateInstance<PermissionsDomain>();

            var userId = Guid.NewGuid();
            var permissionsToInclude = new List<PermissionType>()
            {
                PermissionType.CreateUser
            };
            var existingPermissions = new List<PermissionType>
            {
                PermissionType.ActivateUser,
                PermissionType.InactivateUser
            };

            var expectedPermissions = existingPermissions.Concat(permissionsToInclude);

            mocker.GetMock<IPermissionRepository>().Setup(r => r.GetByUserIdAsync(userId)).ReturnsAsync(existingPermissions);

            #endregion

            await domain.AddAsync(userId, permissionsToInclude);

            #region Assert

            mocker.Verify<IPermissionRepository>(r => r.UpdateAsync(userId, expectedPermissions), Times.Once);

            #endregion
        }

        [Fact]
        internal async Task ShouldRemovePermissions()
        {
            #region Arrange

            var mocker = new AutoMocker();
            var domain = mocker.CreateInstance<PermissionsDomain>();

            var userId = Guid.NewGuid();
            var permissionsToRemove = new List<PermissionType>()
            {
                PermissionType.ActivateUser,
                PermissionType.InactivateUser
            };
            var existingPermissions = new List<PermissionType>
            {
                PermissionType.CreateUser,
                PermissionType.ActivateUser,
                PermissionType.InactivateUser
            };
            var expectedPermissions = existingPermissions.Except(permissionsToRemove);

            mocker.GetMock<IPermissionRepository>().Setup(r => r.GetByUserIdAsync(userId)).ReturnsAsync(existingPermissions);

            #endregion

            await domain.RemoveAsync(userId, permissionsToRemove);

            #region Assert

            mocker.Verify<IPermissionRepository>(r => r.UpdateAsync(userId, expectedPermissions), Times.Once);

            #endregion
        }

        [Fact]
        internal async Task ShouldNotRemoveWhenNotExistsPermissions()
        {
            #region Arrange

            var mocker = new AutoMocker();

            var domain = mocker.CreateInstance<PermissionsDomain>();

            var userId = Guid.NewGuid();
            var permissionsToRemove = new List<PermissionType>()
            {
                PermissionType.ActivateUser,
                PermissionType.InactivateUser
            };

            #endregion

            await domain.RemoveAsync(userId, permissionsToRemove);

            #region Assert

            mocker.Verify<IPermissionRepository>(r => r.UpdateAsync(It.IsAny<Guid>(), It.IsAny<IEnumerable<PermissionType>>()), Times.Never);

            #endregion
        }
    }
}

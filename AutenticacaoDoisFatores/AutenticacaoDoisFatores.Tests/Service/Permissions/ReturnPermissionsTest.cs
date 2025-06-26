using AutenticacaoDoisFatores.Domain.Shared;
using AutenticacaoDoisFatores.Domain.Shared.Permissions;
using AutenticacaoDoisFatores.Domain.Repositories;
using AutenticacaoDoisFatores.Service.UseCases.Permissions;
using AutenticacaoDoisFatores.Service.Dtos.Permissions;
using Bogus;
using Moq;
using Moq.AutoMock;

namespace AutenticacaoDoisFatores.Tests.Service.Permissions
{
    public class ReturnPermissionsTest
    {
        [Fact]
        internal void ShouldReturnAllPermissions()
        {
            #region Arrange

            var expectedPermissions = Enum.GetValues<PermissionType>()
                .Select(tipo => new AvaiblePermission(tipo.Description() ?? "", tipo));

            #endregion

            var response = GetPermissions.GetAll();

            #region Assert

            Assert.Equal(expectedPermissions.Count(), response.Count());
            for (var i = 0; i <= expectedPermissions.Count() - 1; i++)
            {
                var expectedPermission = expectedPermissions.ToArray()[i];
                var returnedItem = response.ToArray()[i];

                Assert.Equal(expectedPermission.Name, returnedItem.Name);
                Assert.Equal(expectedPermission.Value, returnedItem.Value);
            }

            #endregion
        }

        [Fact]
        internal async Task ShouldReturnPermissionsByUser()
        {
            #region Arrange

            var mocker = new AutoMocker();
            var faker = new Faker();

            var userId = Guid.NewGuid();
            var userPermissions = faker.Random.EnumValues<PermissionType>(3);
            var expectedPermissions = userPermissions
                .Select(tipo => new AvaiblePermission(tipo.Description() ?? "", tipo));

            var service = mocker.CreateInstance<GetPermissions>();

            mocker.GetMock<IPermissionRepository>().Setup(r => r.GetByUserIdAsync(userId)).ReturnsAsync(userPermissions);

            #endregion

            var response = await service.GetByUserIdAsync(userId);

            #region Assert

            Assert.NotEmpty(response);
            Assert.Equal(expectedPermissions.Count(), response.Count());
            for (var i = 0; i <= expectedPermissions.Count() - 1; i++)
            {
                var expectedPermission = expectedPermissions.ToArray()[i];
                var returnedItem = response.ToArray()[i];

                Assert.Equal(expectedPermission.Name, returnedItem.Name);
                Assert.Equal(expectedPermission.Value, returnedItem.Value);
            }

            #endregion
        }

        [Fact]
        internal async Task ShouldReturnAllPermissionsByAdminUser()
        {
            #region Arrange

            var mocker = new AutoMocker();

            var userId = Guid.NewGuid();
            var expectedPermissions = Enum.GetValues<PermissionType>()
                .Select(tipo => new AvaiblePermission(tipo.Description() ?? "", tipo));

            var service = mocker.CreateInstance<GetPermissions>();

            mocker.GetMock<IUserRepository>().Setup(r => r.IsAdminAsync(userId)).ReturnsAsync(true);

            #endregion

            var response = await service.GetByUserIdAsync(userId);

            #region Assert

            Assert.NotEmpty(response);
            Assert.Equal(expectedPermissions.Count(), response.Count());
            for (var i = 0; i <= expectedPermissions.Count() - 1; i++)
            {
                var expectedPermission = expectedPermissions.ToArray()[i];
                var returnedItem = response.ToArray()[i];

                Assert.Equal(expectedPermission.Name, returnedItem.Name);
                Assert.Equal(expectedPermission.Value, returnedItem.Value);
            }

            #endregion
        }
    }
}

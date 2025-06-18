using AutenticacaoDoisFatores.Domain.Shared.Messages;
using AutenticacaoDoisFatores.Domain.Shared.Permissions;
using AutenticacaoDoisFatores.Domain.Repositories;
using AutenticacaoDoisFatores.Service.UseCases.Permissions;
using AutenticacaoDoisFatores.Tests.Shared;
using Bogus;
using Messenger;
using Moq;
using Moq.AutoMock;

namespace AutenticacaoDoisFatores.Tests.Service.Permissions
{
    public class AddUserPermissionTest
    {
        private readonly AutoMocker _mocker = new();
        private readonly Faker _faker = new();

        [Fact]
        internal async Task ShouldIncludePermissions()
        {
            #region Arrange

            var userId = Guid.NewGuid();
            var permissionsToInclude = _faker.Random.EnumValues<PermissionType>();

            var user = UserBuilderTest
                .GetBuilder(id: userId, active: true)
                .Build();

            var service = _mocker.CreateInstance<AddUserPermission>();

            _mocker.GetMock<IUserRepository>().Setup(r => r.GetByIdAsync(userId)).ReturnsAsync(user);

            #endregion

            await service.ExecuteAsync(userId, permissionsToInclude);

            _mocker.Verify<IPermissionRepository>(r => r.AddAsync(userId, permissionsToInclude), Times.Once);
        }

        [Fact]
        internal async Task ShouldNotIncludePermissionsWhenUserNotExists()
        {
            #region Arrange

            var userId = Guid.NewGuid();
            var permissionsToInclude = _faker.Random.EnumValues<PermissionType>();

            var service = _mocker.CreateInstance<AddUserPermission>();

            #endregion

            await service.ExecuteAsync(userId, permissionsToInclude);

            #region Assert

            _mocker.Verify<IPermissionRepository>(r => r.AddAsync(It.IsAny<Guid>(), It.IsAny<IEnumerable<PermissionType>>()), Times.Never);
            _mocker.Verify<IPermissionRepository>(r => r.UpdateAsync(It.IsAny<Guid>(), It.IsAny<IEnumerable<PermissionType>>()), Times.Never);
            _mocker.Verify<INotifier>(n => n.AddNotFoundMessage(UserValidationMessages.UserNotFound), Times.Once);

            #endregion
        }

        [Fact]
        internal async Task ShouldNotIncludePermissionsWhenUserNotActive()
        {
            #region Arrange

            var userId = Guid.NewGuid();
            var permissionsToInclude = _faker.Random.EnumValues<PermissionType>();

            var user = UserBuilderTest
                .GetBuilder(id: userId, active: false)
                .Build();

            var service = _mocker.CreateInstance<AddUserPermission>();

            _mocker.GetMock<IUserRepository>().Setup(r => r.GetByIdAsync(userId)).ReturnsAsync(user);

            #endregion

            await service.ExecuteAsync(userId, permissionsToInclude);

            #region Assert

            _mocker.Verify<IPermissionRepository>(r => r.AddAsync(It.IsAny<Guid>(), It.IsAny<IEnumerable<PermissionType>>()), Times.Never);
            _mocker.Verify<IPermissionRepository>(r => r.UpdateAsync(It.IsAny<Guid>(), It.IsAny<IEnumerable<PermissionType>>()), Times.Never);
            _mocker.Verify<INotifier>(n => n.AddNotFoundMessage(UserValidationMessages.UserNotFound), Times.Once);

            #endregion
        }

        [Fact]
        internal async Task ShouldNotIncludePermissionsWhenIsAdminUser()
        {
            #region Arrange

            var userId = Guid.NewGuid();
            var permissionsToInclude = _faker.Random.EnumValues<PermissionType>();

            var user = UserBuilderTest
                .GetBuilder(id: userId, active: true, isAdmin: true)
                .Build();

            var service = _mocker.CreateInstance<AddUserPermission>();

            _mocker.GetMock<IUserRepository>().Setup(r => r.GetByIdAsync(userId)).ReturnsAsync(user);

            #endregion

            await service.ExecuteAsync(userId, permissionsToInclude);

            #region Assert

            _mocker.Verify<IPermissionRepository>(r => r.AddAsync(It.IsAny<Guid>(), It.IsAny<IEnumerable<PermissionType>>()), Times.Never);
            _mocker.Verify<INotifier>(n => n.AddNotFoundMessage(UserValidationMessages.UserNotFound), Times.Once);

            #endregion
        }
    }
}

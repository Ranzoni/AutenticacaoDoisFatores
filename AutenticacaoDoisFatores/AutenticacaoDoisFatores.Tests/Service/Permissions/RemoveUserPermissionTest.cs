using AutenticacaoDoisFatores.Domain.Shared.Messages;
using AutenticacaoDoisFatores.Domain.Shared.Permissions;
using AutenticacaoDoisFatores.Domain.Repositories;
using AutenticacaoDoisFatores.Service.UseCases.Permissions;
using AutenticacaoDoisFatores.Tests.Shared;
using Messenger;
using Moq;
using Moq.AutoMock;

namespace AutenticacaoDoisFatores.Tests.Service.Permissions
{
    public class RemoveUserPermissionTest
    {
        private readonly AutoMocker _mocker = new();

        [Fact]
        internal async Task ShouldRemovePermissionsToUserActive()
        {
            #region Arrange

            var service = _mocker.CreateInstance<RemoveUserPermission>();

            var userId = Guid.NewGuid();
            var user = UserBuilderTest
                .GetBuilder(active: true, id: userId)
                .Build();
            var permissionsToRemove = new List<PermissionType>()
            {
                PermissionType.ActivateUser,
                PermissionType.InactivateUser
            };
            var includedPermissions = new List<PermissionType>
            {
                PermissionType.CreateUser,
                PermissionType.ActivateUser,
                PermissionType.InactivateUser
            };
            var expectedPermissions = includedPermissions.Except(permissionsToRemove);

            _mocker.GetMock<IUserRepository>().Setup(r => r.GetByIdAsync(userId)).ReturnsAsync(user);
            _mocker.GetMock<IPermissionRepository>().Setup(r => r.GetByUserIdAsync(userId)).ReturnsAsync(includedPermissions);

            #endregion

            await service.ExecuteAsync(userId, permissionsToRemove);

            #region Assert

            _mocker.Verify<IPermissionRepository>(r => r.UpdateAsync(userId, expectedPermissions), Times.Once);

            #endregion
        }

        [Fact]
        internal async Task ShouldNotRemovePermissionsToAdminUser()
        {
            #region Arrange

            var service = _mocker.CreateInstance<RemoveUserPermission>();

            var userId = Guid.NewGuid();
            var user = UserBuilderTest
                .GetBuilder(active: true, isAdmin: true, id: userId)
                .Build();
            var permissionsToRemove = new List<PermissionType>()
            {
                PermissionType.ActivateUser,
                PermissionType.InactivateUser
            };

            _mocker.GetMock<IUserRepository>().Setup(r => r.GetByIdAsync(userId)).ReturnsAsync(user);

            #endregion

            await service.ExecuteAsync(userId, permissionsToRemove);

            #region Assert

            _mocker.Verify<IPermissionRepository>(r => r.UpdateAsync(It.IsAny<Guid>(), It.IsAny<IEnumerable<PermissionType>>()), Times.Never);
            _mocker.Verify<INotifier>(n => n.AddNotFoundMessage(UserValidationMessages.UserNotFound), Times.Once);

            #endregion
        }

        [Fact]
        internal async Task ShouldNotRemovePermissionsToUserNotActive()
        {
            #region Arrange

            var service = _mocker.CreateInstance<RemoveUserPermission>();

            var userId = Guid.NewGuid();
            var user = UserBuilderTest
                .GetBuilder(active: false, id: userId)
                .Build();
            var permissionsToRemove = new List<PermissionType>()
            {
                PermissionType.ActivateUser,
                PermissionType.InactivateUser
            };

            _mocker.GetMock<IUserRepository>().Setup(r => r.GetByIdAsync(userId)).ReturnsAsync(user);

            #endregion

            await service.ExecuteAsync(userId, permissionsToRemove);

            #region Assert

            _mocker.Verify<IPermissionRepository>(r => r.UpdateAsync(It.IsAny<Guid>(), It.IsAny<IEnumerable<PermissionType>>()), Times.Never);
            _mocker.Verify<INotifier>(n => n.AddNotFoundMessage(UserValidationMessages.UserNotFound), Times.Once);

            #endregion
        }

        [Fact]
        internal async Task ShouldNotRemovePermissionsToUserNotRegistered()
        {
            #region Arrange

            var service = _mocker.CreateInstance<RemoveUserPermission>();

            var userId = Guid.NewGuid();
            var permissionsToRemove = new List<PermissionType>()
            {
                PermissionType.ActivateUser,
                PermissionType.InactivateUser
            };

            #endregion

            await service.ExecuteAsync(userId, permissionsToRemove);

            #region Assert

            _mocker.Verify<IPermissionRepository>(r => r.UpdateAsync(It.IsAny<Guid>(), It.IsAny<IEnumerable<PermissionType>>()), Times.Never);
            _mocker.Verify<INotifier>(n => n.AddNotFoundMessage(UserValidationMessages.UserNotFound), Times.Once);

            #endregion
        }
    }
}

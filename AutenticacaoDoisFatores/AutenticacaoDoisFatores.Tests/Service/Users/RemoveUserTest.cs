using AutenticacaoDoisFatores.Domain.Shared.Messages;
using AutenticacaoDoisFatores.Domain.Entities;
using AutenticacaoDoisFatores.Domain.Repositories;
using AutenticacaoDoisFatores.Service.UseCases.Users;
using AutenticacaoDoisFatores.Tests.Shared;
using Messenger;
using Moq;
using Moq.AutoMock;

namespace AutenticacaoDoisFatores.Tests.Service.Users
{
    public class RemoveUserTest
    {
        [Fact]
        internal async Task ShouldRemoveUser()
        {
            #region Arrange

            var mocker = new AutoMocker();

            var userId = Guid.NewGuid();
            var user = UserBuilderTest
                .GetBuilder(id: userId)
                .Build();

            var service = mocker.CreateInstance<RemoveUser>();
            mocker.GetMock<IUserRepository>().Setup(r => r.GetByIdAsync(userId)).ReturnsAsync(user);

            #endregion

            await service.ExecuteAsync(userId);

            #region Assert

            mocker.Verify<IUserRepository>(r => r.Remove(user), Times.Once);
            mocker.Verify<IUserRepository>(r => r.SaveChangesAsync(), Times.Once);

            #endregion
        }

        [Fact]
        internal async Task ShouldNotRemoveUserNotRegistered()
        {
            #region Arrange

            var mocker = new AutoMocker();

            var userId = Guid.NewGuid();

            var service = mocker.CreateInstance<RemoveUser>();

            #endregion

            await service.ExecuteAsync(userId);

            #region Assert

            mocker.Verify<IUserRepository>(r => r.Remove(It.IsAny<User>()), Times.Never);
            mocker.Verify<IUserRepository>(r => r.SaveChangesAsync(), Times.Never);
            mocker.Verify<INotifier>(n => n.AddNotFoundMessage(UserValidationMessages.UserNotFound), Times.Once);

            #endregion
        }

        [Fact]
        internal async Task ShouldNotRemoveAdminUser()
        {
            #region Arrange

            var mocker = new AutoMocker();

            var userId = Guid.NewGuid();
            var user = UserBuilderTest
                .GetBuilder(id: userId, isAdmin: true)
                .Build();

            var service = mocker.CreateInstance<RemoveUser>();
            mocker.GetMock<IUserRepository>().Setup(r => r.GetByIdAsync(userId)).ReturnsAsync(user);

            #endregion

            await service.ExecuteAsync(userId);

            #region Assert

            mocker.Verify<IUserRepository>(r => r.Remove(It.IsAny<User>()), Times.Never);
            mocker.Verify<IUserRepository>(r => r.SaveChangesAsync(), Times.Never);
            mocker.Verify<INotifier>(n => n.AddNotFoundMessage(UserValidationMessages.UserNotFound), Times.Once);

            #endregion
        }
    }
}

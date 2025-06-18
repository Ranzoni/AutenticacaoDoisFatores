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
    public class ActivateUserTest
    {
        private readonly AutoMocker _mocker = new();

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        internal async Task SouldActivateUser(bool value)
        {
            #region Arrange

            var userId = Guid.NewGuid();

            var registeredUser = UserBuilderTest
                .GetBuilder(id: userId, active: false)
                .Build();

            var service = _mocker.CreateInstance<ActivateUser>();

            _mocker.GetMock<IUserRepository>().Setup(r => r.GetByIdAsync(userId)).ReturnsAsync(registeredUser);

            #endregion

            await service.ActivateAsync(userId, value);

            #region Assert

            Assert.Equal(value, registeredUser.Active);
            _mocker.Verify<IUserRepository>(r => r.Update(registeredUser), Times.Once);
            _mocker.Verify<IUserRepository>(r => r.SaveChangesAsync(), Times.Once);

            #endregion
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        internal async Task SouldNotActivateUserWhenUserNotExists(bool value)
        {
            #region Arrange

            var userId = Guid.NewGuid();

            var service = _mocker.CreateInstance<ActivateUser>();

            #endregion

            await service.ActivateAsync(userId, value);

            #region Assert

            _mocker.Verify<IUserRepository>(r => r.Update(It.IsAny<User>()), Times.Never);
            _mocker.Verify<IUserRepository>(r => r.SaveChangesAsync(), Times.Never);
            _mocker.Verify<INotifier>(n => n.AddNotFoundMessage(UserValidationMessages.UserNotFound), Times.Once);

            #endregion
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        internal async Task SouldNotActivateUserWhenIsUserAdmin(bool value)
        {
            #region Arrange

            var userId = Guid.NewGuid();

            var registeredUser = UserBuilderTest
                .GetBuilder(id: userId, active: true, isAdmin: true)
                .Build();

            var service = _mocker.CreateInstance<ActivateUser>();

            _mocker.GetMock<IUserRepository>().Setup(r => r.GetByIdAsync(userId)).ReturnsAsync(registeredUser);

            #endregion

            await service.ActivateAsync(userId, value);

            #region Assert

            _mocker.Verify<IUserRepository>(r => r.Update(It.IsAny<User>()), Times.Never);
            _mocker.Verify<IUserRepository>(r => r.SaveChangesAsync(), Times.Never);
            _mocker.Verify<INotifier>(n => n.AddNotFoundMessage(UserValidationMessages.UserNotFound), Times.Once);

            #endregion
        }
    }
}

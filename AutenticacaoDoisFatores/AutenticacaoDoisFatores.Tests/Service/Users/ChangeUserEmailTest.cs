using AutenticacaoDoisFatores.Domain.Shared.Messages;
using AutenticacaoDoisFatores.Domain.Entities;
using AutenticacaoDoisFatores.Domain.Repositories;
using AutenticacaoDoisFatores.Service.UseCases.Users;
using AutenticacaoDoisFatores.Service.Dtos.Users;
using AutenticacaoDoisFatores.Tests.Shared;
using Messenger;
using Moq;
using Moq.AutoMock;

namespace AutenticacaoDoisFatores.Tests.Service.Users
{
    public class ChangeUserEmailTest
    {
        [Fact]
        internal async Task ShouldChangeUserEmail()
        {
            #region Arrange

            var mocker = new AutoMocker();

            var service = mocker.CreateInstance<ChangeUserEmail>();

            var userId = Guid.NewGuid();
            var newEmail = "teste@novoemail.com";
            var changeUserEmail = new UserEmailChange(newEmail);

            var registeredUser = UserBuilderTest
                .GetBuilder(id: userId, active: true, isAdmin: false)
                .Build();

            mocker.GetMock<IUserRepository>().Setup(r => r.GetByIdAsync(userId)).ReturnsAsync(registeredUser);

            #endregion

            await service.ExecuteAsync(userId, changeUserEmail);

            #region Assert

            Assert.Equal(newEmail, registeredUser.Email);
            mocker.Verify<IUserRepository>(r => r.Update(registeredUser), Times.Once);
            mocker.Verify<IUserRepository>(r => r.SaveChangesAsync(), Times.Once);

            #endregion
        }

        [Fact]
        internal async Task ShouldNotChangeUserEmailWhenUserNotRegistered()
        {
            #region Arrange

            var mocker = new AutoMocker();

            var service = mocker.CreateInstance<ChangeUserEmail>();

            var userId = Guid.NewGuid();
            var newEmail = "teste@novoemail.com";

            var changeUserEmail = new UserEmailChange(newEmail);

            mocker.GetMock<INotifier>().Setup(n => n.AnyMessage()).Returns(true);

            #endregion

            await service.ExecuteAsync(userId, changeUserEmail);

            #region Assert

            mocker.Verify<IUserRepository>(r => r.Update(It.IsAny<User>()), Times.Never);
            mocker.Verify<IUserRepository>(r => r.SaveChangesAsync(), Times.Never);
            mocker.Verify<INotifier>(n => n.AddNotFoundMessage(UserValidationMessages.UserNotFound), Times.Once);

            #endregion
        }

        [Fact]
        internal async Task ShouldNotChangeUserEmailWhenUserIsNotActive()
        {
            #region Arrange

            var mocker = new AutoMocker();

            var service = mocker.CreateInstance<ChangeUserEmail>();

            var userId = Guid.NewGuid();
            var newEmail = "teste@novoemail.com";
            var changeUserEmail = new UserEmailChange(newEmail);

            var registeredUser = UserBuilderTest
                .GetBuilder(id: userId, active: false, isAdmin: false)
                .Build();

            mocker.GetMock<IUserRepository>().Setup(r => r.GetByIdAsync(userId)).ReturnsAsync(registeredUser);
            mocker.GetMock<INotifier>().Setup(n => n.AnyMessage()).Returns(true);

            #endregion

            await service.ExecuteAsync(userId, changeUserEmail);

            #region Assert

            Assert.NotEqual(newEmail, registeredUser.Email);
            mocker.Verify<IUserRepository>(r => r.Update(It.IsAny<User>()), Times.Never);
            mocker.Verify<IUserRepository>(r => r.SaveChangesAsync(), Times.Never);
            mocker.Verify<INotifier>(n => n.AddNotFoundMessage(UserValidationMessages.UserNotFound), Times.Once);

            #endregion
        }

        [Fact]
        internal async Task ShouldNotChangeUserEmailWhenIsAdminUser()
        {
            #region Arrange

            var mocker = new AutoMocker();

            var service = mocker.CreateInstance<ChangeUserEmail>();

            var userId = Guid.NewGuid();
            var newEmail = "teste@novoemail.com";
            var changeUserEmail = new UserEmailChange(newEmail);

            var registeredUser = UserBuilderTest
                .GetBuilder(id: userId, active: true, isAdmin: true)
                .Build();

            mocker.GetMock<IUserRepository>().Setup(r => r.GetByIdAsync(userId)).ReturnsAsync(registeredUser);
            mocker.GetMock<INotifier>().Setup(n => n.AnyMessage()).Returns(true);

            #endregion

            await service.ExecuteAsync(userId, changeUserEmail);

            #region Assert

            Assert.NotEqual(newEmail, registeredUser.Email);
            mocker.Verify<IUserRepository>(r => r.Update(It.IsAny<User>()), Times.Never);
            mocker.Verify<IUserRepository>(r => r.SaveChangesAsync(), Times.Never);
            mocker.Verify<INotifier>(n => n.AddNotFoundMessage(UserValidationMessages.UserNotFound), Times.Once);

            #endregion
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("@")]
        [InlineData("a@")]
        [InlineData("a@.")]
        [InlineData("a@.com")]
        [InlineData("@.")]
        [InlineData("@.com")]
        [InlineData("@dominio.com")]
        [InlineData("abcdefghijklmnopqrstuvwxyz0123456789abcdefghijklmnopqrstuvwxyz0123456789abcdefghijklmnopqrstuvwxyz0123456789abcdefghijklmnopqrstuvwxyz0123456789abcdefghijklmnopqrstuvwxyz0123456789abcdefghijklmnopqrstuvwxyz0123456789abcdefghijklmnopqrstuvwxyz0123456789abcde")]
        internal async Task ShouldNotChangeUserEmailWhenEmailIsInvalid(string invalidEmail)
        {
            #region Arrange

            var mocker = new AutoMocker();

            var service = mocker.CreateInstance<ChangeUserEmail>();

            var userId = Guid.NewGuid();
            var changeUserEmail = new UserEmailChange(invalidEmail);

            var registeredUser = UserBuilderTest
                .GetBuilder(id: userId, active: true, isAdmin: false)
                .Build();

            mocker.GetMock<IUserRepository>().Setup(r => r.GetByIdAsync(userId)).ReturnsAsync(registeredUser);
            mocker.GetMock<INotifier>().Setup(n => n.AnyMessage()).Returns(true);

            #endregion

            await service.ExecuteAsync(userId, changeUserEmail);

            #region Assert

            Assert.NotEqual(invalidEmail, registeredUser.Email);
            mocker.Verify<IUserRepository>(r => r.Update(It.IsAny<User>()), Times.Never);
            mocker.Verify<IUserRepository>(r => r.SaveChangesAsync(), Times.Never);
            mocker.Verify<INotifier>(n => n.AddMessage(UserValidationMessages.InvalidEmail), Times.Once);

            #endregion
        }

        [Fact]
        internal async Task ShouldNotChangeEmailWhenEmailIsAlreadyRegistered()
        {
            #region Arrange

            var mocker = new AutoMocker();

            var service = mocker.CreateInstance<ChangeUserEmail>();

            var userId = Guid.NewGuid();
            string newEmail = "teste@novoemail.com";
            var changeUserEmail = new UserEmailChange(newEmail);

            var registeredUser = UserBuilderTest
                .GetBuilder(id: userId, active: true, isAdmin: false)
                .Build();

            mocker.GetMock<IUserRepository>().Setup(r => r.GetByIdAsync(userId)).ReturnsAsync(registeredUser);
            mocker.GetMock<IUserRepository>().Setup(r => r.EmailExistsAsync(newEmail, It.IsAny<Guid?>())).ReturnsAsync(true);
            mocker.GetMock<INotifier>().Setup(n => n.AnyMessage()).Returns(true);

            #endregion

            await service.ExecuteAsync(userId, changeUserEmail);

            #region Assert

            Assert.NotEqual(newEmail, registeredUser.Email);
            mocker.Verify<IUserRepository>(r => r.Update(It.IsAny<User>()), Times.Never);
            mocker.Verify<IUserRepository>(r => r.SaveChangesAsync(), Times.Never);
            mocker.Verify<INotifier>(n => n.AddMessage(UserValidationMessages.EmailAlreadyRegistered), Times.Once);

            #endregion
        }
    }
}

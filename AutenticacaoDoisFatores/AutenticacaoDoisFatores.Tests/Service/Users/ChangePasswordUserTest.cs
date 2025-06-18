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
    public class ChangePasswordUserTest
    {
        private readonly AutoMocker _mocker = new();

        [Fact]
        internal async Task ShouldChangePassword()
        {
            #region Arrange

            var userId = Guid.NewGuid();
            var currentPassword = "Senh4#Atual!!";
            var newPassword = "Teste.De_N0v@!!Password";

            var user = UserBuilderTest
                .GetBuilder(id: userId, password: currentPassword, active: true)
                .Build();

            var service = _mocker.CreateInstance<ChangeUserPassword>();

            _mocker.GetMock<IUserRepository>().Setup(r => r.GetByIdAsync(userId)).ReturnsAsync(user);

            #endregion

            await service.ExecuteAsync(userId, newPassword);

            #region Assert

            Assert.NotEqual(currentPassword, user.Password);
            Assert.NotNull(user.UpdatedAt);
            _mocker.Verify<IUserRepository>(r => r.Update(user), Times.Once);
            _mocker.Verify<IUserRepository>(r => r.SaveChangesAsync(), Times.Once);

            #endregion
        }

        [Theory]
        [InlineData("")]
        [InlineData("       ")]
        [InlineData("T&st.1")]
        [InlineData("Teste.de.senh@.que.é.muito.grand3.Teste.de.senh@.qu")]
        [InlineData("testedesenha.123")]
        [InlineData("@Password.para_teste")]
        [InlineData("TESTEDESENHA.123")]
        [InlineData("2senhaInvalida")]
        internal async Task ShouldNotChangePasswordWhenPasswordIsInvalid(string invalidPassword)
        {
            #region Arrange

            var userId = Guid.NewGuid();

            var service = _mocker.CreateInstance<ChangeUserPassword>();

            #endregion

            await service.ExecuteAsync(userId, invalidPassword);

            #region Assert

            _mocker.Verify<IUserRepository>(r => r.Update(It.IsAny<User>()), Times.Never);
            _mocker.Verify<IUserRepository>(r => r.SaveChangesAsync(), Times.Never);
            _mocker.Verify<INotifier>(n => n.AddMessage(UserValidationMessages.InvalidPassword), Times.Once);

            #endregion
        }

        [Fact]
        internal async Task ShouldNotChangePasswordWhenUserNotRegistered()
        {
            #region Arrange

            var userId = Guid.NewGuid();
            var newPassword = "Teste.De_N0v@!!Password";

            var service = _mocker.CreateInstance<ChangeUserPassword>();

            #endregion

            await service.ExecuteAsync(userId, newPassword);

            #region Assert

            _mocker.Verify<IUserRepository>(r => r.Update(It.IsAny<User>()), Times.Never);
            _mocker.Verify<IUserRepository>(r => r.SaveChangesAsync(), Times.Never);
            _mocker.Verify<INotifier>(n => n.AddNotFoundMessage(UserValidationMessages.UserNotFound), Times.Once);

            #endregion
        }

        [Fact]
        internal async Task ShouldNotChangePasswordWhenUserIsNotActive()
        {
            #region Arrange

            var userId = Guid.NewGuid();
            var currentPassword = "Senh4#Atual!!";
            var newPassword = "Teste.De_N0v@!!Password";

            var user = UserBuilderTest
                .GetBuilder(id: userId, password: currentPassword, active: false)
                .Build();

            var service = _mocker.CreateInstance<ChangeUserPassword>();

            _mocker.GetMock<IUserRepository>().Setup(r => r.GetByIdAsync(userId)).ReturnsAsync(user);

            #endregion

            await service.ExecuteAsync(userId, newPassword);

            #region Assert

            _mocker.Verify<IUserRepository>(r => r.Update(It.IsAny<User>()), Times.Never);
            _mocker.Verify<IUserRepository>(r => r.SaveChangesAsync(), Times.Never);
            _mocker.Verify<INotifier>(n => n.AddNotFoundMessage(UserValidationMessages.UserNotFound), Times.Once);

            #endregion
        }

        [Fact]
        internal async Task ShouldNotChangePasswordWhenIsAdminUser()
        {
            #region Arrange

            var userId = Guid.NewGuid();
            var currentPassword = "Senh4#Atual!!";
            var newPassword = "Teste.De_N0v@!!Password";

            var user = UserBuilderTest
                .GetBuilder(id: userId, password: currentPassword, active: true, isAdmin: true)
                .Build();

            var service = _mocker.CreateInstance<ChangeUserPassword>();

            _mocker.GetMock<IUserRepository>().Setup(r => r.GetByIdAsync(userId)).ReturnsAsync(user);

            #endregion

            await service.ExecuteAsync(userId, newPassword);

            #region Assert

            _mocker.Verify<IUserRepository>(r => r.Update(It.IsAny<User>()), Times.Never);
            _mocker.Verify<IUserRepository>(r => r.SaveChangesAsync(), Times.Never);
            _mocker.Verify<INotifier>(n => n.AddNotFoundMessage(UserValidationMessages.UserNotFound), Times.Once);

            #endregion
        }
    }
}

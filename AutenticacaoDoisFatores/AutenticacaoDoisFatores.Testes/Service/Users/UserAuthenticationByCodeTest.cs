using AutenticacaoDoisFatores.Domain.Shared.Messages;
using AutenticacaoDoisFatores.Domain.Shared.Users;
using AutenticacaoDoisFatores.Domain.Domains;
using AutenticacaoDoisFatores.Domain.Repositories;
using AutenticacaoDoisFatores.Service.UseCases.Users;
using AutenticacaoDoisFatores.Service.Shared;
using AutenticacaoDoisFatores.Tests.Shared;
using Messenger;
using Moq;
using Moq.AutoMock;
using AutenticacaoDoisFatores.Domain.Services;

namespace AutenticacaoDoisFatores.Tests.Service.Users
{
    public class UserAuthenticationByCodeTest
    {
        [Fact]
        internal async Task ShouldAuthenticate()
        {
            #region Arrange

            var mocker = new AutoMocker();

            var service = mocker.CreateInstance<UserAuthenticationByCode>();

            var userId = Guid.NewGuid();
            var authCode = Security.GenerateAuthCode();
            var encryptedAuthCode = Encrypt.EncryptWithSha512(authCode);

            var user = UserBuilderTest
                .GetBuilder(active: true, id: userId)
                .Build();

            mocker.GetMock<IUserRepository>().Setup(r => r.GetByIdAsync(userId)).ReturnsAsync(user);
            mocker.GetMock<IAuthCodeRepository>().Setup(r => r.GetCodeByUserIdAsync(userId)).ReturnsAsync(encryptedAuthCode);

            #endregion

            var response = await service.ExecuteAsync(userId, authCode);

            #region Assert

            Assert.NotNull(response);
            Assert.NotEmpty(response.Token);
            Assert.NotNull(user.LastAccess);

            #endregion
        }

        [Fact]
        internal async Task ShouldAuthenticateWithApp()
        {
            #region Arrange

            var mocker = new AutoMocker();

            var service = mocker.CreateInstance<UserAuthenticationByCode>();

            var userId = Guid.NewGuid();
            var authCode = Security.GenerateAuthCode();
            var encryptedAuthCode = Encrypt.EncryptWithSha512(authCode);

            var user = UserBuilderTest
                .GetBuilder(active: true, id: userId, authType: AuthType.AuthApp)
                .Build();

            mocker.GetMock<IUserRepository>().Setup(r => r.GetByIdAsync(userId)).ReturnsAsync(user);
            mocker.GetMock<IAuthService>().Setup(s => s.IsCodeValid(authCode, user.SecretKey)).Returns(true);

            #endregion

            var response = await service.ExecuteAsync(userId, authCode);

            #region Assert

            Assert.NotNull(response);
            Assert.NotEmpty(response.Token);
            Assert.NotNull(user.LastAccess);

            #endregion
        }

        [Fact]
        internal async Task ShouldNotAuthenticateWhenUserNotExists()
        {
            #region Arrange

            var mocker = new AutoMocker();

            var service = mocker.CreateInstance<UserAuthenticationByCode>();

            var userId = Guid.NewGuid();
            var authCode = Security.GenerateAuthCode();

            #endregion

            var response = await service.ExecuteAsync(userId, authCode);

            #region Assert

            Assert.Null(response);
            mocker.Verify<INotifier>(n => n.AddUnauthorizedMessage(UserValidationMessages.UserNotFound));

            #endregion
        }

        [Fact]
        internal async Task ShouldNotAuthenticateWhenUserNotActive()
        {
            #region Arrange

            var mocker = new AutoMocker();

            var service = mocker.CreateInstance<UserAuthenticationByCode>();

            var userId = Guid.NewGuid();
            var authCode = Security.GenerateAuthCode();
            var encryptedAuthCode = Encrypt.EncryptWithSha512(authCode);

            var user = UserBuilderTest
                .GetBuilder(active: false, id: userId)
                .Build();

            mocker.GetMock<IUserRepository>().Setup(r => r.GetByIdAsync(userId)).ReturnsAsync(user);
            mocker.GetMock<IAuthCodeRepository>().Setup(r => r.GetCodeByUserIdAsync(userId)).ReturnsAsync(encryptedAuthCode);

            #endregion

            var response = await service.ExecuteAsync(userId, authCode);

            #region Assert

            Assert.Null(response);
            mocker.Verify<INotifier>(n => n.AddUnauthorizedMessage(UserValidationMessages.UserNotFound));

            #endregion
        }

        [Fact]
        internal async Task ShouldNotAuthenticateWhenCodeNotExists()
        {
            #region Arrange

            var mocker = new AutoMocker();

            var service = mocker.CreateInstance<UserAuthenticationByCode>();

            var userId = Guid.NewGuid();
            var authCode = Security.GenerateAuthCode();

            var user = UserBuilderTest
                .GetBuilder(active: true, id: userId)
                .Build();

            mocker.GetMock<IUserRepository>().Setup(r => r.GetByIdAsync(userId)).ReturnsAsync(user);

            #endregion

            var response = await service.ExecuteAsync(userId, authCode);

            #region Assert

            Assert.Null(response);
            mocker.Verify<INotifier>(n => n.AddUnauthorizedMessage(UserValidationMessages.Unauthorized));

            #endregion
        }

        [Fact]
        internal async Task ShouldNotAuthenticateWhenCodeIsIncorrect()
        {
            #region Arrange

            var mocker = new AutoMocker();

            var service = mocker.CreateInstance<UserAuthenticationByCode>();

            var userId = Guid.NewGuid();
            var authCode = Security.GenerateAuthCode();
            var encryptedAuthCode = Encrypt.EncryptWithSha512(authCode);

            var user = UserBuilderTest
                .GetBuilder(active: true, id: userId)
                .Build();

            mocker.GetMock<IUserRepository>().Setup(r => r.GetByIdAsync(userId)).ReturnsAsync(user);
            mocker.GetMock<IAuthCodeRepository>().Setup(r => r.GetCodeByUserIdAsync(userId)).ReturnsAsync(encryptedAuthCode);

            #endregion

            var response = await service.ExecuteAsync(userId, authCode + "1");

            #region Assert

            Assert.Null(response);
            mocker.Verify<INotifier>(n => n.AddUnauthorizedMessage(UserValidationMessages.Unauthorized));

            #endregion
        }

        [Fact]
        internal async Task ShouldNotAuthenticateByAppWhenCodeIsIncorrect()
        {
            #region Arrange

            var mocker = new AutoMocker();

            var service = mocker.CreateInstance<UserAuthenticationByCode>();

            var userId = Guid.NewGuid();
            var authCode = Security.GenerateAuthCode();
            var encryptedAuthCode = Encrypt.EncryptWithSha512(authCode);

            var user = UserBuilderTest
                .GetBuilder(active: true, id: userId, authType: AuthType.AuthApp)
                .Build();

            mocker.GetMock<IUserRepository>().Setup(r => r.GetByIdAsync(userId)).ReturnsAsync(user);
            mocker.GetMock<IAuthService>().Setup(s => s.IsCodeValid(authCode, user.SecretKey)).Returns(true);

            #endregion

            var response = await service.ExecuteAsync(userId, authCode + "1");

            #region Assert

            Assert.Null(response);
            mocker.Verify<INotifier>(n => n.AddUnauthorizedMessage(UserValidationMessages.Unauthorized));

            #endregion
        }
    }
}

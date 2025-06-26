using AutenticacaoDoisFatores.Domain.Shared;
using AutenticacaoDoisFatores.Domain.Shared.Messages;
using AutenticacaoDoisFatores.Domain.Shared.Users;
using AutenticacaoDoisFatores.Domain.Domains;
using AutenticacaoDoisFatores.Domain.Repositories;
using AutenticacaoDoisFatores.Service.UseCases.Users;
using AutenticacaoDoisFatores.Service.UseCases.Users.Authenticators;
using AutenticacaoDoisFatores.Service.UseCases.Users.Authenticators.TwoFactorAuths;
using AutenticacaoDoisFatores.Service.Dtos.Users;
using AutenticacaoDoisFatores.Tests.Shared;
using Bogus;
using Messenger;
using Moq;
using Moq.AutoMock;

namespace AutenticacaoDoisFatores.Tests.Service.Users
{
    public class AuthenticateUserTest
    {
        private readonly AutoMocker _mocker = new();
        private readonly Faker _faker = new();

        [Fact]
        internal async Task ShouldAuthenticateWithUsername()
        {
            #region Arrange

            var password = _faker.Random.AlphaNumeric(20);
            var encryptedPassword = Encrypt.EncryptWithSha512(password);

            var user = UserBuilderTest
                .GetBuilder(active: true, password: encryptedPassword)
                .Build();

            var service = _mocker.CreateInstance<AuthenticateUser>();
            var baseUserAuthenticator = _mocker.CreateInstance<BaseUserAuthenticator>();
            _mocker.GetMock<IUserRepository>().Setup(r => r.GetByUsernameAsync(user.Username)).ReturnsAsync(user);
            _mocker.GetMock<IServiceProvider>().Setup(s => s.GetService(typeof(BaseUserAuthenticator))).Returns(baseUserAuthenticator);

            var authData = new AuthData(
                usernameOrEmail: user.Username,
                password: password);

            var fakeTokenIssuer = _faker.Random.AlphaNumeric(40);
            Environment.SetEnvironmentVariable("ADF_EMISSOR_TOKEN", fakeTokenIssuer);
            var fakeTokenAudience = _faker.Random.AlphaNumeric(40);
            Environment.SetEnvironmentVariable("ADF_DESTINATARIO_TOKEN", fakeTokenAudience);
            var fakerAuthKey = _faker.Random.AlphaNumeric(40);
            Environment.SetEnvironmentVariable("ADF_CHAVE_AUTENTICACAO", fakerAuthKey);

            #endregion

            var response = await service.ExecuteAsync(authData);

            #region Assert

            var authenticatedUser = (AuthenticatedUser?)response;
            Assert.NotNull(authenticatedUser);
            Assert.Equal(user.Name, authenticatedUser.User.Name);
            Assert.Equal(user.Username, authenticatedUser.User.Username);
            Assert.Equal(user.Email, authenticatedUser.User.Email);
            Assert.False(authenticatedUser.Token.IsNullOrEmptyOrWhiteSpaces());

            #endregion
        }

        [Fact]
        internal async Task ShouldAuthenticateWithEmail()
        {
            #region Arrange

            var password = _faker.Random.AlphaNumeric(20);
            var encryptedPassword = Encrypt.EncryptWithSha512(password);

            var user = UserBuilderTest
                .GetBuilder(active: true, password: encryptedPassword)
                .Build();

            var service = _mocker.CreateInstance<AuthenticateUser>();
            var baseUserAuthenticator = _mocker.CreateInstance<BaseUserAuthenticator>();
            _mocker.GetMock<IUserRepository>().Setup(r => r.GetByEmailAsync(user.Email)).ReturnsAsync(user);
            _mocker.GetMock<IServiceProvider>().Setup(s => s.GetService(typeof(BaseUserAuthenticator))).Returns(baseUserAuthenticator);

            var authData = new AuthData(
                usernameOrEmail: user.Email,
                password: password);

            var fakeTokenIssuer = _faker.Random.AlphaNumeric(40);
            Environment.SetEnvironmentVariable("ADF_EMISSOR_TOKEN", fakeTokenIssuer);
            var fakeTokenAudience = _faker.Random.AlphaNumeric(40);
            Environment.SetEnvironmentVariable("ADF_DESTINATARIO_TOKEN", fakeTokenAudience);
            var fakerAuthKey = _faker.Random.AlphaNumeric(40);
            Environment.SetEnvironmentVariable("ADF_CHAVE_AUTENTICACAO", fakerAuthKey);

            #endregion

            var response = await service.ExecuteAsync(authData);

            #region Assert

            var authenticatedUser = (AuthenticatedUser?)response;
            Assert.NotNull(authenticatedUser);
            Assert.Equal(user.Name, authenticatedUser.User.Name);
            Assert.Equal(user.Username, authenticatedUser.User.Username);
            Assert.Equal(user.Email, authenticatedUser.User.Email);
            Assert.False(authenticatedUser.Token.IsNullOrEmptyOrWhiteSpaces());

            #endregion
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("  ")]
        internal async Task ShouldNotAuthenticateWhenUsernameAndEmailNotInformed(string? emptyUsernameAndEmail)
        {
            #region Arrange

            var service = _mocker.CreateInstance<AuthenticateUser>();

            var authData = new AuthData(
                usernameOrEmail: emptyUsernameAndEmail,
                password: "teste.de_senh4@@");

            #endregion

            var response = await service.ExecuteAsync(authData);

            #region Assert

            Assert.Null(response);
            _mocker.Verify<INotifier>(n => n.AddMessage(UserValidationMessages.UsernameOrEmailRequired), Times.Once);

            #endregion
        }

        [Fact]
        internal async Task ShouldNotAuthenticateByUsernameWhenUserNotExists()
        {
            #region Arrange

            var service = _mocker.CreateInstance<AuthenticateUser>();

            var authData = new AuthData(
                usernameOrEmail: _faker.Person.UserName,
                password: "teste.de_senh4@@");

            #endregion

            var response = await service.ExecuteAsync(authData);

            #region Assert

            Assert.Null(response);
            _mocker.Verify<INotifier>(n => n.AddUnauthorizedMessage(UserValidationMessages.Unauthorized), Times.Once);

            #endregion
        }

        [Fact]
        internal async Task ShouldNotAuthenticateByEmailWhenUserNotExists()
        {
            #region Arrange

            var service = _mocker.CreateInstance<AuthenticateUser>();

            var authData = new AuthData(
                usernameOrEmail: _faker.Person.Email,
                password: "teste.de_senh4@@");

            #endregion

            var response = await service.ExecuteAsync(authData);

            #region Assert

            Assert.Null(response);
            _mocker.Verify<INotifier>(n => n.AddUnauthorizedMessage(UserValidationMessages.Unauthorized), Times.Once);

            #endregion
        }

        [Fact]
        internal async Task ShouldNotAuthenticateByUsernameWhenUserNotActive()
        {
            #region Arrange

            var userId = Guid.NewGuid();

            var user = UserBuilderTest
                .GetBuilder(id: userId, active: false)
                .Build();

            var service = _mocker.CreateInstance<AuthenticateUser>();

            _mocker.GetMock<IUserRepository>().Setup(r => r.GetByUsernameAsync(user.Username)).ReturnsAsync(user);

            var authData = new AuthData(
                usernameOrEmail: user.Username,
                password: "teste.de_senh4@@");

            #endregion

            var response = await service.ExecuteAsync(authData);

            #region Assert

            Assert.Null(response);
            _mocker.Verify<INotifier>(n => n.AddUnauthorizedMessage(UserValidationMessages.Unauthorized), Times.Once);

            #endregion
        }

        [Fact]
        internal async Task ShouldNotAuthenticateByEmailWhenUserNotActive()
        {
            #region Arrange

            var user = UserBuilderTest
                .GetBuilder(active: false)
                .Build();

            var service = _mocker.CreateInstance<AuthenticateUser>();

            _mocker.GetMock<IUserRepository>().Setup(r => r.GetByEmailAsync(user.Email)).ReturnsAsync(user);

            var authData = new AuthData(
                usernameOrEmail: user.Email,
                password: "teste.de_senh4@@");

            #endregion

            var response = await service.ExecuteAsync(authData);

            #region Assert

            Assert.Null(response);
            _mocker.Verify<INotifier>(n => n.AddUnauthorizedMessage(UserValidationMessages.Unauthorized), Times.Once);

            #endregion
        }

        [Fact]
        internal async Task ShouldNotAuthenticateByUsernameWhenPasswordIsIncorrect()
        {
            #region Arrange

            var userId = Guid.NewGuid();

            var user = UserBuilderTest
                .GetBuilder(id: userId, active: true, password: _faker.Random.AlphaNumeric(30))
                .Build();

            var service = _mocker.CreateInstance<AuthenticateUser>();

            _mocker.GetMock<IUserRepository>().Setup(r => r.GetByUsernameAsync(user.Username)).ReturnsAsync(user);

            var authData = new AuthData(
                usernameOrEmail: user.Username,
                password: "teste.de_senh4@@");

            #endregion

            var response = await service.ExecuteAsync(authData);

            #region Assert

            Assert.Null(response);
            _mocker.Verify<INotifier>(n => n.AddUnauthorizedMessage(UserValidationMessages.Unauthorized), Times.Once);

            #endregion
        }

        [Fact]
        internal async Task ShouldNotAuthenticateByEmailWhenPasswordIsIncorrect()
        {
            #region Arrange

            var userId = Guid.NewGuid();

            var user = UserBuilderTest
                .GetBuilder(id: userId, active: true, password: _faker.Random.AlphaNumeric(30))
                .Build();

            var service = _mocker.CreateInstance<AuthenticateUser>();

            _mocker.GetMock<IUserRepository>().Setup(r => r.GetByEmailAsync(user.Email)).ReturnsAsync(user);

            var authData = new AuthData(
                usernameOrEmail: user.Email,
                password: "teste.de_senh4@@");

            #endregion

            var response = await service.ExecuteAsync(authData);

            #region Assert

            Assert.Null(response);
            _mocker.Verify<INotifier>(n => n.AddUnauthorizedMessage(UserValidationMessages.Unauthorized), Times.Once);

            #endregion
        }


        [Fact]
        internal async Task ShouldAuthenticateSendingTwoFactorCode()
        {
            #region Arrange

            var password = _faker.Random.AlphaNumeric(20);
            var encryptedPassword = Encrypt.EncryptWithSha512(password);

            var user = UserBuilderTest
                .GetBuilder(active: true, password: encryptedPassword, authType: AuthType.Email)
                .Build();

            var service = _mocker.CreateInstance<AuthenticateUser>();
            var twoFactorUserAuthenticator = _mocker.CreateInstance<UserTwoFactorAuthentication>();
            var twoFactorUserAuthByEmail = _mocker.CreateInstance<UserTwoFactorAuthByEmail>();
            _mocker.GetMock<IUserRepository>().Setup(r => r.GetByEmailAsync(user.Email)).ReturnsAsync(user);
            _mocker.GetMock<IServiceProvider>().Setup(s => s.GetService(typeof(UserTwoFactorAuthentication))).Returns(twoFactorUserAuthenticator);
            _mocker.GetMock<IServiceProvider>().Setup(s => s.GetService(typeof(UserTwoFactorAuthByEmail))).Returns(twoFactorUserAuthByEmail);

            var authData = new AuthData(
                usernameOrEmail: user.Email,
                password: password);

            var fakeTokenIssuer = _faker.Random.AlphaNumeric(40);
            Environment.SetEnvironmentVariable("ADF_EMISSOR_TOKEN", fakeTokenIssuer);
            var fakeTokenAudience = _faker.Random.AlphaNumeric(40);
            Environment.SetEnvironmentVariable("ADF_DESTINATARIO_TOKEN", fakeTokenAudience);
            var fakerAuthKey = _faker.Random.AlphaNumeric(40);
            Environment.SetEnvironmentVariable("ADF_CHAVE_AUTENTICACAO", fakerAuthKey);

            #endregion

            var response = await service.ExecuteAsync(authData);

            #region Assert

            var authenticatedUser = (TwoFactorAuthResponse?)response;
            Assert.NotNull(authenticatedUser);
            Assert.NotEmpty(authenticatedUser.Token);

            #endregion
        }
    }
}

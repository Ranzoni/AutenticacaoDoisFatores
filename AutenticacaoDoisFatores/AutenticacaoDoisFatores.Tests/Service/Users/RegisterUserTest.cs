using AutenticacaoDoisFatores.Domain.Shared.Messages;
using AutenticacaoDoisFatores.Domain.Domains;
using AutenticacaoDoisFatores.Domain.Entities;
using AutenticacaoDoisFatores.Domain.Repositories;
using AutenticacaoDoisFatores.Service.UseCases.Users;
using AutenticacaoDoisFatores.Tests.Shared;
using Bogus;
using Messenger;
using Moq;
using Moq.AutoMock;

namespace AutenticacaoDoisFatores.Tests.Service.Users
{
    public class RegisterUserTest
    {
        private readonly AutoMocker _mocker = new();
        private readonly Faker _faker = new();

        [Fact]
        internal async Task ShouldRegisterUser()
        {
            #region Arrange

            var name = _faker.Person.FullName;
            var username = _faker.Person.UserName;
            var email = _faker.Person.Email;
            var phone = 55016993880077;

            var newUser = UserBuilderTest
                .GetBuilderOfNew(name: name, username: username, email: email, phone: phone)
                .Build();

            _mocker.CreateInstance<UserDomain>();

            var service = _mocker.CreateInstance<RegisterUser>();

            #endregion

            var registeredUser = await service.RegisterAsync(newUser);

            #region Assert

            Assert.NotNull(registeredUser);
            Assert.Equal(name, registeredUser.Name);
            Assert.Equal(username, registeredUser.Username);
            Assert.Equal(email, registeredUser.Email);
            Assert.False(registeredUser.Active);
            Assert.Equal(phone, registeredUser.Phone);

            _mocker.Verify<IUserRepository>(r => r.Add(It.IsAny<User>()), Times.Once);
            _mocker.Verify<IUserRepository>(r => r.SaveChangesAsync(), Times.Once);

            #endregion
        }

        [Theory]
        [InlineData("")]
        [InlineData("         ")]
        [InlineData("ab")]
        [InlineData("Teste de name grande Teste de name grande Teste de ")]
        internal async Task ShouldNotRegisterUserWhenInvalidName(string invalidName)
        {
            #region Arrange

            var newUser = UserBuilderTest
                .GetBuilderOfNew(name: invalidName)
                .Build();

            _mocker.CreateInstance<UserDomain>();

            _mocker.GetMock<INotifier>().Setup(n => n.AnyMessage()).Returns(true);

            var service = _mocker.CreateInstance<RegisterUser>();

            #endregion

            var registeredUser = await service.RegisterAsync(newUser);

            #region Assert

            Assert.Null(registeredUser);
            _mocker.Verify<IUserRepository>(r => r.Add(It.IsAny<User>()), Times.Never);
            _mocker.Verify<IUserRepository>(r => r.SaveChangesAsync(), Times.Never);
            _mocker.Verify<INotifier>(n => n.AddMessage(UserValidationMessages.InvalidName), Times.Once);

            #endregion
        }

        [Theory]
        [InlineData("")]
        [InlineData("         ")]
        [InlineData("abcd")]
        [InlineData("Teste de name grande ")]
        internal async Task ShouldNotRegisterUserWhenInvalidUsername(string invalidUsername)
        {
            #region Arrange

            var newUser = UserBuilderTest
                .GetBuilderOfNew(username: invalidUsername)
                .Build();

            _mocker.CreateInstance<UserDomain>();

            _mocker.GetMock<INotifier>().Setup(n => n.AnyMessage()).Returns(true);

            var service = _mocker.CreateInstance<RegisterUser>();

            #endregion

            var registeredUser = await service.RegisterAsync(newUser);

            #region Assert

            Assert.Null(registeredUser);
            _mocker.Verify<IUserRepository>(r => r.Add(It.IsAny<User>()), Times.Never);
            _mocker.Verify<IUserRepository>(r => r.SaveChangesAsync(), Times.Never);
            _mocker.Verify<INotifier>(n => n.AddMessage(UserValidationMessages.InvalidUsername), Times.Once);

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
        internal async Task ShouldNotRegisterUserWhenInvalidEmail(string invalidEmail)
        {
            #region Arrange
            
            var newUser = UserBuilderTest
                .GetBuilderOfNew(email: invalidEmail)
                .Build();

            _mocker.CreateInstance<UserDomain>();

            _mocker.GetMock<INotifier>().Setup(n => n.AnyMessage()).Returns(true);

            var service = _mocker.CreateInstance<RegisterUser>();

            #endregion

            var registeredUser = await service.RegisterAsync(newUser);

            #region Assert

            Assert.Null(registeredUser);
            _mocker.Verify<IUserRepository>(r => r.Add(It.IsAny<User>()), Times.Never);
            _mocker.Verify<IUserRepository>(r => r.SaveChangesAsync(), Times.Never);
            _mocker.Verify<INotifier>(n => n.AddMessage(UserValidationMessages.InvalidEmail), Times.Once);

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
        internal async Task ShouldNotRegisterUserWhenInvalidPassword(string invalidPassword)
        {
            #region Arrange

            var newUser = UserBuilderTest
                .GetBuilderOfNew(password: invalidPassword)
                .Build();

            _mocker.CreateInstance<UserDomain>();

            _mocker.GetMock<INotifier>().Setup(n => n.AnyMessage()).Returns(true);

            var service = _mocker.CreateInstance<RegisterUser>();

            #endregion

            var registeredUser = await service.RegisterAsync(newUser);

            #region Assert

            Assert.Null(registeredUser);
            _mocker.Verify<IUserRepository>(r => r.Add(It.IsAny<User>()), Times.Never);
            _mocker.Verify<IUserRepository>(r => r.SaveChangesAsync(), Times.Never);
            _mocker.Verify<INotifier>(n => n.AddMessage(UserValidationMessages.InvalidPassword), Times.Once);

            #endregion
        }

        [Fact]
        internal async Task ShouldNotRegisterUserWhenUsernameAlreadyRegistered()
        {
            #region Arrange

            var newUser = UserBuilderTest
                .GetBuilderOfNew()
                .Build();

            _mocker.CreateInstance<UserDomain>();
            _mocker.GetMock<IUserRepository>().Setup(r => r.UsernameExistsAsync(newUser.Username, It.IsAny<Guid?>())).ReturnsAsync(true);
            _mocker.GetMock<INotifier>().Setup(n => n.AnyMessage()).Returns(true);

            var service = _mocker.CreateInstance<RegisterUser>();

            #endregion

            var registeredUser = await service.RegisterAsync(newUser);

            #region Assert

            Assert.Null(registeredUser);
            _mocker.Verify<IUserRepository>(r => r.Add(It.IsAny<User>()), Times.Never);
            _mocker.Verify<IUserRepository>(r => r.SaveChangesAsync(), Times.Never);
            _mocker.Verify<INotifier>(n => n.AddMessage(UserValidationMessages.UsernameAlreadyRegistered), Times.Once);

            #endregion
        }

        [Fact]
        internal async Task ShouldNotRegisterUserWhenEmailAlreadyRegistered()
        {
            #region Arrange

            var newUser = UserBuilderTest
                .GetBuilderOfNew()
                .Build();

            _mocker.CreateInstance<UserDomain>();
            _mocker.GetMock<IUserRepository>().Setup(r => r.EmailExistsAsync(newUser.Email, It.IsAny<Guid?>())).ReturnsAsync(true);
            _mocker.GetMock<INotifier>().Setup(n => n.AnyMessage()).Returns(true);

            var service = _mocker.CreateInstance<RegisterUser>();

            #endregion

            var registeredUser = await service.RegisterAsync(newUser);

            #region Assert

            Assert.Null(registeredUser);
            _mocker.Verify<IUserRepository>(r => r.Add(It.IsAny<User>()), Times.Never);
            _mocker.Verify<IUserRepository>(r => r.SaveChangesAsync(), Times.Never);
            _mocker.Verify<INotifier>(n => n.AddMessage(UserValidationMessages.EmailAlreadyRegistered), Times.Once);

            #endregion
        }
    }
}

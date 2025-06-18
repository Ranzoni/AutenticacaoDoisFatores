using AutenticacaoDoisFatores.Domain.Shared;
using AutenticacaoDoisFatores.Domain.Shared.Messages;
using AutenticacaoDoisFatores.Domain.Domains;
using AutenticacaoDoisFatores.Domain.Entities;
using AutenticacaoDoisFatores.Domain.Exceptions;
using AutenticacaoDoisFatores.Domain.Filters;
using AutenticacaoDoisFatores.Domain.Repositories;
using AutenticacaoDoisFatores.Tests.Shared;
using Bogus;
using Moq;
using Moq.AutoMock;

namespace AutenticacaoDoisFatores.Tests.Domain.Domains
{
    public class UserDomainTest
    {
        private readonly AutoMocker _mocker = new();
        private readonly Faker _faker = new();

        #region Creation tests

        [Fact]
        internal async Task ShouldCreateUser()
        {
            #region Arrange

            var domain = _mocker.CreateInstance<UserDomain>();

            var newUser = UserBuilderTest
                .GetBuilder()
                .BuildNew();

            #endregion

            await domain.CreateAsync(newUser);

            #region Assert

            _mocker.Verify<IUserRepository>(r => r.Add(newUser), Times.Once);
            _mocker.Verify<IUserRepository>(r => r.SaveChangesAsync(), Times.Once);

            #endregion
        }

        [Fact]
        internal async Task ShouldCreateUserByDomain()
        {
            #region Arrange

            var domain = _mocker.CreateInstance<UserDomain>();

            var newUser = UserBuilderTest
                .GetBuilder()
                .BuildNew();

            var domainName = _faker.Internet.DomainName();

            #endregion

            await domain.CreateByDomainAsync(newUser, domainName);

            #region Assert

            _mocker.Verify<IUserRepository>(r => r.Add(newUser, domainName), Times.Once);
            _mocker.Verify<IUserRepository>(r => r.SaveChangesAsync(), Times.Once);

            #endregion
        }

        [Fact]
        internal async Task ShouldReturnExceptionWhenCreateUserWithExistingUsername()
        {
            #region Arrange

            var domain = _mocker.CreateInstance<UserDomain>();

            var newUser = UserBuilderTest
                .GetBuilder()
                .BuildNew();

            _mocker.GetMock<IUserRepository>().Setup(r => r.UsernameExistsAsync(newUser.Username, It.IsAny<Guid>())).ReturnsAsync(true);

            #endregion

            var exception = await Assert.ThrowsAsync<UserException>(() => domain.CreateAsync(newUser));

            #region Assert

            Assert.Equal(UserValidationMessages.UsernameAlreadyRegistered.Description(), exception.Message);
            _mocker.Verify<IUserRepository>(r => r.Add(It.IsAny<User>()), Times.Never);
            _mocker.Verify<IUserRepository>(r => r.SaveChangesAsync(), Times.Never);

            #endregion
        }

        [Fact]
        internal async Task ShouldReturnExceptionWhenCreateUserWithDomainNameWithExistingUsername()
        {
            #region Arrange

            var domain = _mocker.CreateInstance<UserDomain>();

            var newUser = UserBuilderTest
                .GetBuilder()
                .BuildNew();

            var domainName = _faker.Internet.DomainName();

            _mocker.GetMock<IUserRepository>().Setup(r => r.UsernameExistsAsync(newUser.Username, domainName)).ReturnsAsync(true);

            #endregion

            var exception = await Assert.ThrowsAsync<UserException>(() => domain.CreateByDomainAsync(newUser, domainName));

            #region Assert

            Assert.Equal(UserValidationMessages.UsernameAlreadyRegistered.Description(), exception.Message);
            _mocker.Verify<IUserRepository>(r => r.Add(It.IsAny<User>(), It.IsAny<string>()), Times.Never);
            _mocker.Verify<IUserRepository>(r => r.SaveChangesAsync(), Times.Never);

            #endregion
        }

        [Fact]
        internal async Task ShouldReturnExceptionWhenCreateUserWithExistingEmail()
        {
            #region Arrange

            var domain = _mocker.CreateInstance<UserDomain>();

            var newUser = UserBuilderTest
                .GetBuilder()
                .BuildNew();

            _mocker.GetMock<IUserRepository>().Setup(r => r.EmailExistsAsync(newUser.Email, It.IsAny<Guid>())).ReturnsAsync(true);

            #endregion

            var exception = await Assert.ThrowsAsync<UserException>(() => domain.CreateAsync(newUser));

            #region Assert

            Assert.Equal(UserValidationMessages.EmailAlreadyRegistered.Description(), exception.Message);
            _mocker.Verify<IUserRepository>(r => r.Add(It.IsAny<User>()), Times.Never);
            _mocker.Verify<IUserRepository>(r => r.SaveChangesAsync(), Times.Never);

            #endregion
        }

        [Fact]
        internal async Task ShouldReturnExceptionWhenCreateUserWithDomainNameWithExistingEmail()
        {
            #region Arrange

            var domain = _mocker.CreateInstance<UserDomain>();

            var newUser = UserBuilderTest
                .GetBuilder()
                .BuildNew();

            var domainName = _faker.Internet.DomainName();

            _mocker.GetMock<IUserRepository>().Setup(r => r.EmailExistsAsync(newUser.Email, domainName)).ReturnsAsync(true);

            #endregion

            var exception = await Assert.ThrowsAsync<UserException>(() => domain.CreateByDomainAsync(newUser, domainName));

            #region Assert

            Assert.Equal(UserValidationMessages.EmailAlreadyRegistered.Description(), exception.Message);
            _mocker.Verify<IUserRepository>(r => r.Add(It.IsAny<User>(), It.IsAny<string>()), Times.Never);
            _mocker.Verify<IUserRepository>(r => r.SaveChangesAsync(), Times.Never);

            #endregion
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        internal async Task ShouldReturnIfUsernameAlreadyExists(bool expectedResult)
        {
            #region Arrange

            var domain = _mocker.CreateInstance<UserDomain>();

            var fakeUsername = _faker.Person.UserName;

            _mocker.GetMock<IUserRepository>().Setup(r => r.UsernameExistsAsync(fakeUsername, It.IsAny<Guid?>())).ReturnsAsync(expectedResult);

            #endregion

            var usernameExists = await domain.UsernameExistsAsync(fakeUsername);

            #region Assert

            Assert.Equal(expectedResult, usernameExists);
            _mocker.Verify<IUserRepository>(r => r.UsernameExistsAsync(fakeUsername, It.IsAny<Guid?>()), Times.Once);

            #endregion
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        internal async Task ShouldReturnIfEmailAlreadyExists(bool expectedResult)
        {
            #region Arrange

            var domain = _mocker.CreateInstance<UserDomain>();

            var fakeUsername = _faker.Person.Email;

            _mocker.GetMock<IUserRepository>().Setup(r => r.EmailExistsAsync(fakeUsername, It.IsAny<Guid?>())).ReturnsAsync(expectedResult);

            #endregion

            var emailExists = await domain.EmailExistsAsync(fakeUsername);

            #region Assert

            Assert.Equal(expectedResult, emailExists);
            _mocker.Verify<IUserRepository>(r => r.EmailExistsAsync(fakeUsername, It.IsAny<Guid?>()), Times.Once);

            #endregion
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        internal async Task ShouldReturnIfIsUserAdmin(bool expectedResult)
        {
            #region Arrange

            var userId = Guid.NewGuid();

            var domain = _mocker.CreateInstance<UserDomain>();

            _mocker.GetMock<IUserRepository>().Setup(r => r.IsAdminAsync(userId)).ReturnsAsync(expectedResult);

            #endregion

            var result = await domain.IsAdminAsync(userId);

            #region Assert

            Assert.Equal(expectedResult, result);
            _mocker.Verify<IUserRepository>(r => r.IsAdminAsync(userId), Times.Once);

            #endregion
        }

        #endregion

        #region Change tests

        [Fact]
        internal async Task ShouldUpdateUser()
        {
            #region Arrange

            var domain = _mocker.CreateInstance<UserDomain>();

            var userToUpdate = UserBuilderTest
                .GetBuilder()
                .BuildNew();

            _mocker.GetMock<IUserRepository>().Setup(r => r.GetByIdAsync(userToUpdate.Id)).ReturnsAsync(userToUpdate);

            #endregion

            await domain.UpdateAsync(userToUpdate);

            #region Assert

            _mocker.Verify<IUserRepository>(r => r.Update(userToUpdate), Times.Once);
            _mocker.Verify<IUserRepository>(r => r.SaveChangesAsync(), Times.Once);

            #endregion
        }

        [Fact]
        internal async Task ShouldReturnExceptionWhenUpdateUserWithExistingUsername()
        {
            #region Arrange

            var domain = _mocker.CreateInstance<UserDomain>();

            var userToUpdate = UserBuilderTest
                .GetBuilder()
                .Build();

            _mocker.GetMock<IUserRepository>().Setup(r => r.UsernameExistsAsync(userToUpdate.Username, userToUpdate.Id)).ReturnsAsync(true);

            #endregion

            var exception = await Assert.ThrowsAsync<UserException>(() => domain.UpdateAsync(userToUpdate));

            #region Assert

            Assert.Equal(UserValidationMessages.UsernameAlreadyRegistered.Description(), exception.Message);
            _mocker.Verify<IUserRepository>(r => r.Update(It.IsAny<User>()), Times.Never);
            _mocker.Verify<IUserRepository>(r => r.SaveChangesAsync(), Times.Never);

            #endregion
        }

        [Fact]
        internal async Task ShouldReturnExceptionWhenUpdateUserWithExistingEmail()
        {
            #region Arrange

            var domain = _mocker.CreateInstance<UserDomain>();

            var userToUpdate = UserBuilderTest
                .GetBuilder()
                .Build();

            _mocker.GetMock<IUserRepository>().Setup(r => r.EmailExistsAsync(userToUpdate.Email, userToUpdate.Id)).ReturnsAsync(true);

            #endregion

            var exception = await Assert.ThrowsAsync<UserException>(() => domain.UpdateAsync(userToUpdate));

            #region Assert

            Assert.Equal(UserValidationMessages.EmailAlreadyRegistered.Description(), exception.Message);
            _mocker.Verify<IUserRepository>(r => r.Update(It.IsAny<User>()), Times.Never);
            _mocker.Verify<IUserRepository>(r => r.SaveChangesAsync(), Times.Never);

            #endregion
        }

        #endregion

        #region Search tests

        [Fact]
        internal async Task ShouldGetById()
        {
            #region Arrange

            var userId = Guid.NewGuid();
            var domain = _mocker.CreateInstance<UserDomain>();

            var registeredUser = UserBuilderTest
                .GetBuilder()
                .Build();

            _mocker.GetMock<IUserRepository>().Setup(r => r.GetByIdAsync(userId)).ReturnsAsync(registeredUser);

            #endregion

            var user = await domain.GetByIdAsync(userId);

            #region Assert

            Assert.NotNull(user);
            Assert.Equal(registeredUser, user);

            #endregion
        }

        [Fact]
        internal async Task ShouldReturnNullWhenGetByIdAndNotFound()
        {
            #region Arrange

            var userId = Guid.NewGuid();
            var domain = _mocker.CreateInstance<UserDomain>();

            #endregion

            var user = await domain.GetByIdAsync(userId);

            #region Assert

            Assert.Null(user);

            #endregion
        }

        [Fact]
        internal async Task ShouldGetAll()
        {
            #region Arrange

            var mocker = new AutoMocker();

            var filters = new UserFilter();
            var maximoRegistros = _faker.Random.Int(2, filters.Quantity);

            var listaDeUsuarios = GenerateMany(maximoRegistros, active: true);

            var maximoPaginacao = _faker.Random.Int(1, maximoRegistros);

            var domain = mocker.CreateInstance<UserDomain>();
            mocker.GetMock<IUserRepository>().Setup(r => r.GetAllAsync(filters)).ReturnsAsync(listaDeUsuarios);

            #endregion

            var result = await domain.GetAllAsync(filters);

            #region Assert

            Assert.NotNull(result);
            Assert.Equal(listaDeUsuarios, result);
            mocker.Verify<IUserRepository>(r => r.GetAllAsync(filters), Times.Once);

            #endregion
        }

        [Fact]
        internal async Task ShouldReturnEmptyListWhenGetAllAndNotFound()
        {
            #region Arrange

            var mocker = new AutoMocker();

            var filters = new UserFilter();

            var domain = mocker.CreateInstance<UserDomain>();

            #endregion

            var result = await domain.GetAllAsync(filters);

            #region Assert

            Assert.Empty(result);
            mocker.Verify<IUserRepository>(r => r.GetAllAsync(filters), Times.Once);

            #endregion
        }

        [Fact]
        internal async Task ShouldGetByEmail()
        {
            #region Arrange

            var mocker = new AutoMocker();

            var email = _faker.Person.Email;
            var user = UserBuilderTest
                .GetBuilder(email: email)
                .Build();

            var domain = mocker.CreateInstance<UserDomain>();
            mocker.GetMock<IUserRepository>().Setup(r => r.GetByEmailAsync(email)).ReturnsAsync(user);

            #endregion

            var result = await domain.GetByEmailAsync(email);

            #region Assert

            Assert.NotNull(result);
            Assert.Equal(user, result);
            mocker.Verify<IUserRepository>(r => r.GetByEmailAsync(email), Times.Once);

            #endregion
        }

        [Fact]
        internal async Task ShouldReturnNullWhenGetByEmailAndNotFound()
        {
            #region Arrange

            var mocker = new AutoMocker();

            var email = _faker.Person.Email;

            var domain = mocker.CreateInstance<UserDomain>();

            #endregion

            var result = await domain.GetByEmailAsync(email);

            #region Assert

            Assert.Null(result);
            mocker.Verify<IUserRepository>(r => r.GetByEmailAsync(email), Times.Once);

            #endregion
        }

        [Fact]
        internal async Task ShouldGetByUsername()
        {
            #region Arrange

            var mocker = new AutoMocker();

            var username = "user_12345@";
            var user = UserBuilderTest
                .GetBuilder(username: username)
                .Build();

            var domain = mocker.CreateInstance<UserDomain>();
            mocker.GetMock<IUserRepository>().Setup(r => r.GetByUsernameAsync(username)).ReturnsAsync(user);

            #endregion

            var result = await domain.GetByUsernameAsync(username);

            #region Assert

            Assert.NotNull(result);
            Assert.Equal(user, result);
            mocker.Verify<IUserRepository>(r => r.GetByUsernameAsync(username), Times.Once);

            #endregion
        }

        [Fact]
        internal async Task ShouldReturnNullWhenGetByUsernameAndNotFound()
        {
            #region Arrange

            var mocker = new AutoMocker();

            var username = "user_12345@";

            var domain = mocker.CreateInstance<UserDomain>();

            #endregion

            var result = await domain.GetByUsernameAsync(username);

            #region Assert

            Assert.Null(result);
            mocker.Verify<IUserRepository>(r => r.GetByUsernameAsync(username), Times.Once);

            #endregion
        }

        #endregion

        #region Deletion tests

        [Fact]
        internal async Task ShouldRemoveUser()
        {
            #region Arrange

            var domain = _mocker.CreateInstance<UserDomain>();

            var userToRemove = UserBuilderTest
                .GetBuilder()
                .BuildNew();

            _mocker.GetMock<IUserRepository>().Setup(r => r.GetByIdAsync(userToRemove.Id)).ReturnsAsync(userToRemove);

            #endregion

            await domain.RemoveAsync(userToRemove.Id);

            #region Assert

            _mocker.Verify<IUserRepository>(r => r.Remove(userToRemove), Times.Once);
            _mocker.Verify<IUserRepository>(r => r.SaveChangesAsync(), Times.Once);

            #endregion
        }

        [Fact]
        internal async Task ShouldReturnExceptionWhenRemoveInexistingUser()
        {
            #region Arrange

            var domain = _mocker.CreateInstance<UserDomain>();

            #endregion

            var exception = await Assert.ThrowsAsync<UserException>(() => domain.RemoveAsync(Guid.NewGuid()));

            #region Assert

            Assert.Equal(UserValidationMessages.UserNotFound.Description(), exception.Message);
            _mocker.Verify<IUserRepository>(r => r.Remove(It.IsAny<User>()), Times.Never);
            _mocker.Verify<IUserRepository>(r => r.SaveChangesAsync(), Times.Never);

            #endregion
        }

        #endregion

        private static List<User> GenerateMany(int quantity, bool? active = null)
        {
            var users = new List<User>();

            for (var i = 1; i <= quantity; i++)
            {
                var user = UserBuilderTest
                    .GetBuilder(active: active)
                    .Build();

                users.Add(user);
            }

            return users;
        }
    }
}

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
    public class UpdateUserTest
    {
        [Fact]
        internal async Task ShouldUpdateUser()
        {
            #region Arrange

            var mocker = new AutoMocker();
            var faker = new Faker();

            mocker.CreateInstance<UserDomain>();

            var service = mocker.CreateInstance<UpdateUser>();

            var userId = Guid.NewGuid();

            var userToUpdate = UserBuilderTest
                .GetBuilder(id: userId, name: "Fulano de Tal", username: "user_test_12345", phone: 993000333, active: true)
                .Build();

            mocker.GetMock<IUserRepository>().Setup(r => r.GetByIdAsync(userId)).ReturnsAsync(userToUpdate);

            var newName = faker.Person.FullName;
            var newUsername = "user_test_54321";
            var newPhone = 992222001;

            var newData = UserBuilderTest
                .GetBuilderOfNewData(name: newName, username: newUsername, phone: newPhone)
                .Build();

            #endregion

            var response = await service.ExecuteAsync(userId, newData);

            #region Assert

            Assert.NotNull(response);
            Assert.Equal(newName, response.Name);
            Assert.Equal(newUsername, response.Username);
            Assert.Equal(newPhone, response.Phone);

            mocker.Verify<IUserRepository>(r => r.Update(userToUpdate), Times.Once);
            mocker.Verify<IUserRepository>(r => r.SaveChangesAsync(), Times.Once);

            #endregion
        }

        [Fact]
        internal async Task ShouldNotUpdateUserNotRegistered()
        {
            #region Arrange

            var mocker = new AutoMocker();
            var faker = new Faker();

            var service = mocker.CreateInstance<UpdateUser>();

            var userId = Guid.NewGuid();

            var newData = UserBuilderTest
                .GetBuilderOfNewData()
                .Build();

            mocker.GetMock<INotifier>().Setup(n => n.AnyMessage()).Returns(true);

            #endregion

            var response = await service.ExecuteAsync(userId, newData);

            #region Assert

            Assert.Null(response);

            mocker.Verify<INotifier>(n => n.AddNotFoundMessage(UserValidationMessages.UserNotFound), Times.Once);
            mocker.Verify<IUserRepository>(r => r.Update(It.IsAny<User>()), Times.Never);
            mocker.Verify<IUserRepository>(r => r.SaveChangesAsync(), Times.Never);

            #endregion
        }

        [Fact]
        internal async Task ShouldNotUpdateUserNotActive()
        {
            #region Arrange

            var mocker = new AutoMocker();
            var faker = new Faker();

            var service = mocker.CreateInstance<UpdateUser>();

            var userId = Guid.NewGuid();

            var userToUpdate = UserBuilderTest
                .GetBuilder(id: userId, name: "Fulano de Tal", username: "user_test_12345", phone: 993000333, active: false)
                .Build();

            mocker.GetMock<IUserRepository>().Setup(r => r.GetByIdAsync(userId)).ReturnsAsync(userToUpdate);

            var newName = faker.Person.FullName;
            var newUsername = "user_test_54321";
            var newPhone = 992222001;

            var newData = UserBuilderTest
                .GetBuilderOfNewData(name: newName, username: newUsername, phone: newPhone)
                .Build();

            mocker.GetMock<INotifier>().Setup(n => n.AnyMessage()).Returns(true);

            #endregion

            var response = await service.ExecuteAsync(userId, newData);

            #region Assert

            Assert.Null(response);
            Assert.NotEqual(newName, userToUpdate.Name);
            Assert.NotEqual(newUsername, userToUpdate.Username);
            Assert.NotEqual(newPhone, userToUpdate.Phone);

            mocker.Verify<INotifier>(n => n.AddNotFoundMessage(UserValidationMessages.UserNotFound), Times.Once);
            mocker.Verify<IUserRepository>(r => r.Update(It.IsAny<User>()), Times.Never);
            mocker.Verify<IUserRepository>(r => r.SaveChangesAsync(), Times.Never);

            #endregion
        }

        [Fact]
        internal async Task ShouldNotUpdateUserWhenIsAdminUser()
        {
            #region Arrange

            var mocker = new AutoMocker();
            var faker = new Faker();

            var service = mocker.CreateInstance<UpdateUser>();

            var userId = Guid.NewGuid();

            var userToUpdate = UserBuilderTest
                .GetBuilder(id: userId, name: "Fulano de Tal", username: "user_test_12345", active: true, phone: 993000333, isAdmin: true)
                .Build();

            mocker.GetMock<IUserRepository>().Setup(r => r.GetByIdAsync(userId)).ReturnsAsync(userToUpdate);

            var newName = faker.Person.FullName;
            var newUsername = "user_test_54321";
            var newPhone = 992222001;

            var newData = UserBuilderTest
                .GetBuilderOfNewData(name: newName, username: newUsername, phone: newPhone)
                .Build();

            mocker.GetMock<INotifier>().Setup(n => n.AnyMessage()).Returns(true);

            #endregion

            var response = await service.ExecuteAsync(userId, newData);

            #region Assert

            Assert.Null(response);
            Assert.NotEqual(newName, userToUpdate.Name);
            Assert.NotEqual(newUsername, userToUpdate.Username);
            Assert.NotEqual(newPhone, userToUpdate.Phone);

            mocker.Verify<INotifier>(n => n.AddNotFoundMessage(UserValidationMessages.UserNotFound), Times.Once);
            mocker.Verify<IUserRepository>(r => r.Update(It.IsAny<User>()), Times.Never);
            mocker.Verify<IUserRepository>(r => r.SaveChangesAsync(), Times.Never);

            #endregion
        }

        [Theory]
        [InlineData("")]
        [InlineData("         ")]
        [InlineData("ab")]
        [InlineData("Teste de nome grande Teste de nome grande Teste de ")]
        internal async Task ShouldNotUpdateWithInvalidName(string invalidName)
        {
            #region Arrange

            var mocker = new AutoMocker();
            var faker = new Faker();

            var service = mocker.CreateInstance<UpdateUser>();

            var userId = Guid.NewGuid();

            var userToUpdate = UserBuilderTest
                .GetBuilder(id: userId, name: "Fulano de Tal", username: "user_test_12345", active: true)
                .Build();

            mocker.GetMock<IUserRepository>().Setup(r => r.GetByIdAsync(userId)).ReturnsAsync(userToUpdate);

            var newUsername = "user_test_54321";

            var newData = UserBuilderTest
                .GetBuilderOfNewData(name: invalidName, username: newUsername)
                .Build();

            mocker.GetMock<INotifier>().Setup(n => n.AnyMessage()).Returns(true);

            #endregion

            var response = await service.ExecuteAsync(userId, newData);

            #region Assert

            Assert.Null(response);
            Assert.NotEqual(invalidName, userToUpdate.Name);
            Assert.NotEqual(newUsername, userToUpdate.Username);

            mocker.Verify<INotifier>(n => n.AddMessage(UserValidationMessages.InvalidName), Times.Once);
            mocker.Verify<IUserRepository>(r => r.Update(It.IsAny<User>()), Times.Never);
            mocker.Verify<IUserRepository>(r => r.SaveChangesAsync(), Times.Never);

            #endregion
        }

        [Theory]
        [InlineData("")]
        [InlineData("         ")]
        [InlineData("abcd")]
        [InlineData("Teste de nome grande ")]
        internal async Task ShouldNotUpdateWithInvalidUsername(string invalidUsername)
        {
            #region Arrange

            var mocker = new AutoMocker();
            var faker = new Faker();

            var service = mocker.CreateInstance<UpdateUser>();

            var userId = Guid.NewGuid();

            var userToUpdate = UserBuilderTest
                .GetBuilder(id: userId, name: "Fulano de Tal", username: "user_test_12345", active: true)
                .Build();

            mocker.GetMock<IUserRepository>().Setup(r => r.GetByIdAsync(userId)).ReturnsAsync(userToUpdate);

            var newName = faker.Person.FullName;

            var newData = UserBuilderTest
                .GetBuilderOfNewData(name: newName, username: invalidUsername)
                .Build();

            mocker.GetMock<INotifier>().Setup(n => n.AnyMessage()).Returns(true);

            #endregion

            var response = await service.ExecuteAsync(userId, newData);

            #region Assert

            Assert.Null(response);
            Assert.NotEqual(newName, userToUpdate.Name);
            Assert.NotEqual(invalidUsername, userToUpdate.Username);

            mocker.Verify<INotifier>(n => n.AddMessage(UserValidationMessages.InvalidUsername), Times.Once);
            mocker.Verify<IUserRepository>(r => r.Update(It.IsAny<User>()), Times.Never);
            mocker.Verify<IUserRepository>(r => r.SaveChangesAsync(), Times.Never);

            #endregion
        }

        [Theory]
        [InlineData((long)-9999)]
        [InlineData((long)1)]
        [InlineData((long)10334)]
        internal async Task ShouldNotUpdateWithInvalidPhone(long? invalidPhone)
        {
            #region Arrange

            var mocker = new AutoMocker();
            var faker = new Faker();

            var service = mocker.CreateInstance<UpdateUser>();

            var userId = Guid.NewGuid();

            var userToUpdate = UserBuilderTest
                .GetBuilder(id: userId, name: "Fulano de Tal", phone: 993000333, active: true)
                .Build();

            mocker.GetMock<IUserRepository>().Setup(r => r.GetByIdAsync(userId)).ReturnsAsync(userToUpdate);

            var newName = faker.Person.FullName;
            var newUsername = "user_test_12345";

            var newData = UserBuilderTest
                .GetBuilderOfNewData(name: newName, username: newUsername, phone: invalidPhone)
                .Build();

            mocker.GetMock<INotifier>().Setup(n => n.AnyMessage()).Returns(true);

            #endregion

            var response = await service.ExecuteAsync(userId, newData);

            #region Assert

            Assert.Null(response);
            Assert.NotEqual(newName, userToUpdate.Name);
            Assert.NotEqual(newUsername, userToUpdate.Username);
            Assert.NotEqual(invalidPhone, userToUpdate.Phone);

            mocker.Verify<INotifier>(n => n.AddMessage(UserValidationMessages.InvalidPhone), Times.Once);
            mocker.Verify<IUserRepository>(r => r.Update(It.IsAny<User>()), Times.Never);
            mocker.Verify<IUserRepository>(r => r.SaveChangesAsync(), Times.Never);

            #endregion
        }

        [Fact]
        internal async Task ShouldNotUpdateUserWithNameAlreadyRegistered()
        {
            #region Arrange

            var mocker = new AutoMocker();
            var faker = new Faker();

            var userId = Guid.NewGuid();

            var userToUpdate = UserBuilderTest
                .GetBuilder(id: userId, name: "Fulano de Tal", username: "user_test_12345", active: true)
                .Build();

            var service = mocker.CreateInstance<UpdateUser>();

            var newName = faker.Person.FullName;
            var newUsername = "user_test_54321";

            var newData = UserBuilderTest
                .GetBuilderOfNewData(name: newName, username: newUsername)
                .Build();

            mocker.GetMock<IUserRepository>().Setup(r => r.UsernameExistsAsync(newUsername, userId)).ReturnsAsync(true);
            mocker.GetMock<IUserRepository>().Setup(r => r.GetByIdAsync(userId)).ReturnsAsync(userToUpdate);
            mocker.GetMock<INotifier>().Setup(n => n.AnyMessage()).Returns(true);

            #endregion

            var response = await service.ExecuteAsync(userId, newData);

            #region Assert

            Assert.Null(response);
            Assert.NotEqual(newName, userToUpdate.Name);
            Assert.NotEqual(newUsername, userToUpdate.Username);

            mocker.Verify<INotifier>(n => n.AddMessage(UserValidationMessages.UsernameAlreadyRegistered), Times.Once);
            mocker.Verify<IUserRepository>(r => r.Update(It.IsAny<User>()), Times.Never);
            mocker.Verify<IUserRepository>(r => r.SaveChangesAsync(), Times.Never);

            #endregion
        }
    }
}

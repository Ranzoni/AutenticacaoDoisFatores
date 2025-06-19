using AutenticacaoDoisFatores.Domain.Shared;
using AutenticacaoDoisFatores.Domain.Shared.Messages;
using AutenticacaoDoisFatores.Domain.Shared.Users;
using AutenticacaoDoisFatores.Domain.Exceptions;
using AutenticacaoDoisFatores.Tests.Shared;
using Bogus;

namespace AutenticacaoDoisFatores.Tests.Domain.Entities
{
    public class UserTest
    {
        private readonly Faker _faker = new();

        [Theory]
        [InlineData("Teste.De.Senha_1")]
        [InlineData("2senha@Valida")]
        [InlineData("2senhaéValida")]
        [InlineData("S3nha.m")]
        [InlineData("Teste.de.senh@.que.é.muito.grand3.Teste.de.senh@.que.é.muito.grand3.Teste.de.senh@.que.é.muito.grand3.Teste.de.senh@.que.é.muito.grand3.Teste.de.senh@.que.é.muito.grand3.Teste.de.senh@.que.é.muito.grand3.Teste.de.senh@.que.é.muito.grand3.Teste.de.senh@.que")]
        internal void ShouldCreateUser(string password)
        {
            #region Arrange

            var name = _faker.Person.FullName;
            var username = "teste_user_12398";
            var email = _faker.Person.Email;
            var phone = 5516993388778;

            #endregion

            var user = UserBuilderTest
                .GetBuilder(name: name, username: username, email: email, password: password, phone: phone)
                .BuildNew();

            #region Assert

            Assert.Equal(name, user.Name);
            Assert.Equal(username, user.Username);
            Assert.Equal(email, user.Email);
            Assert.Equal(password, user.Password);
            Assert.Equal(phone, user.Phone);
            Assert.Null(user.LastAccess);
            Assert.NotEmpty(user.LastDataChange);

            #endregion
        }

        [Theory]
        [InlineData("")]
        [InlineData("         ")]
        [InlineData("ab")]
        [InlineData("Teste de name grande Teste de name grande Teste de ")]
        internal void ShouldNotCreateUserWithInvalidName(string invalidName)
        {
            var exception = Assert.Throws<UserException>
                (() => UserBuilderTest
                    .GetBuilder(name: invalidName)
                    .BuildNew()
                );

            Assert.Equal(UserValidationMessages.InvalidName.Description(), exception.Message);
        }

        [Theory]
        [InlineData("")]
        [InlineData("         ")]
        [InlineData("abcd")]
        [InlineData("Teste de name grande ")]
        internal void ShouldNotCreateUserWithInvalidUsername(string invalidUsername)
        {
            var exception = Assert.Throws<UserException>
                (() => UserBuilderTest
                    .GetBuilder(username: invalidUsername)
                    .BuildNew()
                );

            Assert.Equal(UserValidationMessages.InvalidUsername.Description(), exception.Message);
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
        internal void ShouldNotCreateUserWithInvalidEmail(string invalidEmail)
        {
            var exception = Assert.Throws<UserException>
                (() => UserBuilderTest
                    .GetBuilder(email: invalidEmail)
                    .BuildNew()
                );

            Assert.Equal(UserValidationMessages.InvalidEmail.Description(), exception.Message);
        }

        [Theory]
        [InlineData("")]
        [InlineData("       ")]
        [InlineData("Teste.de.senh@.que.é.muito.grand3.Teste.de.senh@.que.é.muito.grand3.Teste.de.senh@.que.é.muito.grand3.Teste.de.senh@.que.é.muito.grand3.Teste.de.senh@.que.é.muito.grand3.Teste.de.senh@.que.é.muito.grand3.Teste.de.senh@.que.é.muito.grand3.Teste.de.senh@.que.")]
        internal void ShouldNotCreateUserWithInvalidPassword(string invalidPassword)
        {
            var exception = Assert.Throws<UserException>
                (() => UserBuilderTest
                    .GetBuilder(password: invalidPassword)
                    .BuildNew()
                );

            Assert.Equal(UserValidationMessages.InvalidPassword.Description(), exception.Message);
        }

        [Fact]
        internal void ShouldUpdateName()
        {
            #region Arrange

            var currentName = "Fulano de Tal";
            var newName = _faker.Person.FullName;

            var user = UserBuilderTest
                .GetBuilder(name: currentName)
                .Build();

            #endregion

            user.UpdateName(newName);

            #region Assert

            Assert.Equal(newName, user.Name);
            Assert.NotNull(user.UpdatedAt);

            #endregion
        }

        [Theory]
        [InlineData("")]
        [InlineData("         ")]
        [InlineData("ab")]
        [InlineData("Teste de name grande Teste de name grande Teste de ")]
        internal void ShouldNotUpdateWithInvalidName(string invalidName)
        {
            #region Arrange

            var currentName = "Fulano de Tal";

            var user = UserBuilderTest
                .GetBuilder(name: currentName)
                .Build();

            #endregion

            var exception = Assert.Throws<UserException>(() => user.UpdateName(invalidName));

            #region Assert

            Assert.Equal(UserValidationMessages.InvalidName.Description(), exception.Message);

            #endregion
        }

        [Fact]
        internal void ShouldUpdateUsername()
        {
            #region Arrange

            var currentUsername = "user_test_12345";
            var newUsername = "user_test_54321";

            var user = UserBuilderTest
                .GetBuilder(username: currentUsername)
                .Build();

            #endregion

            user.UpdateUsername(newUsername);

            #region Assert

            Assert.Equal(newUsername, user.Username);
            Assert.NotNull(user.UpdatedAt);

            #endregion
        }

        [Theory]
        [InlineData("")]
        [InlineData("         ")]
        [InlineData("abcd")]
        [InlineData("Teste de name grande ")]
        internal void ShouldNotUpdateWithInvalidUsername(string invalidUsername)
        {
            #region Arrange

            var currentUsername = "user_test_12345";

            var user = UserBuilderTest
                .GetBuilder(username: currentUsername)
                .Build();

            #endregion

            var exception = Assert.Throws<UserException>(() => user.UpdateUsername(invalidUsername));

            #region Assert

            Assert.Equal(UserValidationMessages.InvalidUsername.Description(), exception.Message);

            #endregion
        }

        [Fact]
        internal void ShouldUpdateEmail()
        {
            #region Arrange

            var currentEmail = "fulano@test.com";
            var newEmail = _faker.Person.Email;

            var user = UserBuilderTest
                .GetBuilder(email: currentEmail)
                .Build();

            #endregion

            user.UpdateEmail(newEmail);

            #region Assert

            Assert.Equal(newEmail, user.Email);
            Assert.NotNull(user.UpdatedAt);
            Assert.NotEmpty(user.LastDataChange);

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
        internal void ShouldNotUpdateWithInvalidEmail(string invalidEmail)
        {
            #region Arrange

            var currentEmail = "fulano@test.com";

            var user = UserBuilderTest
                .GetBuilder(email: currentEmail)
                .Build();

            #endregion

            var exception = Assert.Throws<UserException>(() => user.UpdateEmail(invalidEmail));

            #region Assert

            Assert.Equal(UserValidationMessages.InvalidEmail.Description(), exception.Message);

            #endregion
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        internal void ShouldActivateOrInactivateUser(bool activate)
        {
            #region Arrange

            var user = UserBuilderTest
                .GetBuilder(active: !activate)
                .Build();

            #endregion

            user.SetActive(activate);

            #region Assert

            Assert.Equal(activate, user.Active);
            Assert.NotNull(user.UpdatedAt);

            #endregion
        }

        [Fact]
        internal void ShouldUpdatePassword()
        {
            #region Arrange

            var currentPassword = "Senh4#Atu@al";
            var newPassword = "Nov@Senh4_!";

            var user = UserBuilderTest
                .GetBuilder(password: currentPassword)
                .Build();

            #endregion

            user.UpdatePassword(newPassword);

            #region Assert

            Assert.Equal(newPassword, user.Password);
            Assert.NotNull(user.UpdatedAt);
            Assert.NotEmpty(user.LastDataChange);

            #endregion
        }

        [Theory]
        [InlineData("")]
        [InlineData("       ")]
        [InlineData("Teste.de.senh@.que.é.muito.grand3.Teste.de.senh@.que.é.muito.grand3.Teste.de.senh@.que.é.muito.grand3.Teste.de.senh@.que.é.muito.grand3.Teste.de.senh@.que.é.muito.grand3.Teste.de.senh@.que.é.muito.grand3.Teste.de.senh@.que.é.muito.grand3.Teste.de.senh@.que.")]
        internal void ShouldNotUpdateWithInvalidPassword(string invalidPassword)
        {
            #region Arrange

            var currentPassword = "Senh4#Atu@al";

            var user = UserBuilderTest
                .GetBuilder(password: currentPassword)
                .Build();

            #endregion

            var exception = Assert.Throws<UserException>(() => user.UpdatePassword(invalidPassword));

            #region Assert

            Assert.Equal(UserValidationMessages.InvalidPassword.Description(), exception.Message);

            #endregion
        }

        [Fact]
        internal void ShouldUpdatePhone()
        {
            #region Arrange

            var currentPhone = 55016990011001;
            var newPhone = 16990222200;

            var user = UserBuilderTest
                .GetBuilder(phone: currentPhone)
                .Build();

            #endregion

            user.UpdatePhone(newPhone);

            #region Assert

            Assert.Equal(newPhone, user.Phone);
            Assert.NotNull(user.UpdatedAt);

            #endregion
        }

        [Theory]
        [InlineData(-99999)]
        [InlineData(1)]
        [InlineData(10092)]
        internal void ShouldNotUpdatePhoneWithInvalidNumber(long invalidPhone)
        {
            #region Arrange

            var currentPhone = 55016990011001;

            var user = UserBuilderTest
                .GetBuilder(phone: currentPhone)
                .Build();

            #endregion

            var exception = Assert.Throws<UserException>(() => user.UpdatePhone(invalidPhone));

            Assert.Equal(UserValidationMessages.InvalidPhone.Description(), exception.Message);
        }

        [Fact]
        internal void ShouldUpdateLastAccess()
        {
            #region Arrange

            var user = UserBuilderTest
                .GetBuilder()
                .Build();

            #endregion

            user.UpdateLastAccess();

            #region Assert

            Assert.NotNull(user.LastAccess);
            Assert.True(user.LastAccess <= DateTime.Now);
            Assert.NotNull(user.UpdatedAt);

            #endregion
        }

        [Fact]
        internal void ShouldUpdateAuthTypeToUser()
        {
            #region Arrange

            var authType = _faker.Random.Enum<AuthType>();

            var user = UserBuilderTest
                .GetBuilder()
                .Build();

            #endregion

            user.ConfigureAuthType(authType);

            #region Assert

            Assert.Equal(authType, user.AuthType);
            Assert.NotNull(user.UpdatedAt);

            #endregion
        }

        [Fact]
        internal void ShouldReturnTrueWhenExistsAuthTypeConfigured()
        {
            var user = UserBuilderTest
                .GetBuilder(authType: AuthType.Email)
                .Build();

            var value = user.AnyAuthTypeConfigured();

            Assert.True(value);
        }

        [Fact]
        internal void ShouldReturnFalseWhenNotExistsAuthTypeConfigured()
        {
            var user = UserBuilderTest
                .GetBuilder()
                .Build();

            var value = user.AnyAuthTypeConfigured();

            Assert.False(value);
        }

        [Theory]
        [InlineData("")]
        [InlineData("       ")]
        [InlineData("012345678901")]
        [InlineData("0123456789012345678")]
        internal void ShouldNotCreateUsesrWithInvalidSecretKey(string invalidSecretKey)
        {
            var user = UserBuilderTest
                .GetBuilder(secretKey: invalidSecretKey);
            
            var exception = Assert.Throws<UserException>(user.Build);

            Assert.Equal(UserValidationMessages.InvalidSecretKey.Description(), exception.Message);
        }
    }
}

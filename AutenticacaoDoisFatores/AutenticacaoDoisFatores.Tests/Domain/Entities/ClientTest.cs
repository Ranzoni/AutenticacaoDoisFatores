using AutenticacaoDoisFatores.Domain.Validators;
using AutenticacaoDoisFatores.Domain.Exceptions;
using Bogus;
using AutenticacaoDoisFatores.Domain.Shared.Messages;
using AutenticacaoDoisFatores.Domain.Shared;
using AutenticacaoDoisFatores.Tests.Shared;

namespace AutenticacaoDoisFatores.Tests.Domain.Entities
{
    public class ClientTest
    {
        private readonly Faker _faker = new();

        [Fact]
        internal void ShouldCreateClient()
        {
            #region Arrange

            var fakeName = _faker.Company.CompanyName();
            var fakeEmail = _faker.Internet.Email();
            var fakeDomainName = "dominio_cliente";
            var fakeAccessKey = _faker.Random.AlphaNumeric(20);

            var builder = ClientBuilderTest.GetBuilder
                (
                    name: fakeName,
                    email: fakeEmail,
                    domainName: fakeDomainName,
                    accessKey: fakeAccessKey
                );

            #endregion

            var client = builder.BuildNew();

            #region Assert

            Assert.NotNull(client);

            Assert.Equal(fakeName, client.Name);
            Assert.True(ClientValidator.IsNameValid(client.Name));

            Assert.Equal(fakeEmail, client.Email);
            Assert.True(ClientValidator.IsEmailValid(client.Email));

            Assert.Equal(fakeDomainName, client.DomainName);
            Assert.True(ClientValidator.IsDomainNameValid(client.DomainName));

            Assert.Equal(fakeAccessKey, client.AccessKey);
            Assert.True(ClientValidator.IsAccessKeyValid(client.AccessKey));

            #endregion
        }

        [Fact]
        internal void ShouldCreateRegisteredClient()
        {
            #region Arrange

            var fakeName = _faker.Company.CompanyName();
            var fakeEmail = _faker.Internet.Email();
            var fakeDomainName = "dominio_cliente";
            var fakeAccessKey = _faker.Random.AlphaNumeric(20);
            var fakeActiveValue = _faker.Random.Bool();
            var fakeCreatedAt = _faker.Date.Past(1);
            var fakeUpdatedAt = _faker.Date.Past(1, fakeCreatedAt);

            var builder = ClientBuilderTest.GetBuilder
                (
                    name: fakeName,
                    email: fakeEmail,
                    domainName: fakeDomainName,
                    accessKey: fakeAccessKey,
                    active: fakeActiveValue,
                    createdAt: fakeCreatedAt,
                    updatedAt: fakeUpdatedAt
                );

            #endregion

            var client = builder.Build();

            #region Assert

            Assert.NotNull(client);

            Assert.Equal(fakeName, client.Name);
            Assert.True(ClientValidator.IsNameValid(client.Name));

            Assert.Equal(fakeEmail, client.Email);
            Assert.True(ClientValidator.IsEmailValid(client.Email));

            Assert.Equal(fakeDomainName, client.DomainName);
            Assert.True(ClientValidator.IsDomainNameValid(client.DomainName));

            Assert.Equal(fakeAccessKey, client.AccessKey);
            Assert.True(ClientValidator.IsAccessKeyValid(client.AccessKey));

            Assert.Equal(fakeActiveValue, client.Active);

            Assert.Equal(fakeCreatedAt, client.CreatedAt);

            Assert.Equal(fakeUpdatedAt, client.UpdatedAt);

            #endregion
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("ab")]
        [InlineData("abcdefghijklmnopqrstuvwxyz0123456789abcdefghijklmno")]
        internal void ShouldNotCreateClientWithInvalidName(string invalidName)
        {
            var builder = ClientBuilderTest.GetBuilder(name: invalidName);

            var exception = Assert.Throws<ClientException>(builder.BuildNew);

            Assert.Equal(ClientValidationMessages.InvalidName.Description(), exception.Message);
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
        internal void ShouldNotCreateClientWithInvalidEmail(string invalidEmail)
        {
            var builder = ClientBuilderTest.GetBuilder(email: invalidEmail);

            var exception = Assert.Throws<ClientException>(builder.BuildNew);

            Assert.Equal(ClientValidationMessages.InvalidEmail.Description(), exception.Message);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("ab")]
        [InlineData("abcdefghijklmnop")]
        [InlineData("teste dominio")]
        [InlineData("domínio")]
        [InlineData("dominio.")]
        [InlineData("dominio@")]
        [InlineData("dominio!")]
        internal void ShouldNotCreateClientWithInvalidDomainName(string invalidDomainName)
        {
            var builder = ClientBuilderTest.GetBuilder(domainName: invalidDomainName);

            var exception = Assert.Throws<ClientException>(builder.BuildNew);

            Assert.Equal(ClientValidationMessages.InvalidDomainName.Description(), exception.Message);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("ab")]
        [InlineData("abcdefghijklmnopqrstuvwxyz0123456789abcdefghijklmnopqrstuvwxyz0123456789abcdefghijklmnopqrstuvwxyz0123456789abcdefghijklmnopqrstuvwxyz0123456789abcdefghijklmnopqrstuvwxyz0123456789abcdefghijklmnopqrstuvwxyz0123456789abcdefghijklmnopqrstuvwxyz0123456789abcdefghijklmnopqrstuvwxyz0123456789abcdefghijklmnopqrstuvwxyz0123456789abcdefghijklmnopqrstuvwxyz0123456789abcdefghijklmnopqrstuvwxyz0123456789abcdefghijklmnopqrstuvwxyz0123456789abcdefghijklmnopqrstuvwxyz0123456789abcdefghijklmnopqrstuvwxyz0123456789abcdefghijklmnopqrstuvwxyz0123456789abcdefghijklmnopqrstuvwxyz0123456789abcdefghijklmnopqrstuvwxyz0123456789abcdefghijklmnopqrstuvwxyz0123456789abcdefghijklmnopqrstuvwxyz0123456789abcdefghijklmnopqrstuvwxyz0123456789abcdefghijklmnopqrstuvwxyz0123456789abcdefghijklmnopqrstuvwxyz0123456789abcdefghijklmnopqrstuvwxyz0123456789abcdefghijklmnopqrstuvwxyz0123456789abcdefghijklmnopqrstuvwxyz0123456789abcdefghijklmnopqrstuvwxyz0123456789abcdefghijklmnopqrstuvwxyz0123456789abcdefghijklmnopqrstuvwxyz0123456789abcdefghijklmnopqrstuvwxyz0123456789abcdefghijklmnopqrstuvwxyz0123456789abcdefghijklmnopqrstuvwxyz0123456789abcdefghijklmnopqrstuvwxyz0123456789abcdefghijklmnopqrstuvwxyz0123456789abcdefghijklmnopqrstuvwxyz0123456789abcdefghijklmnopqrstuvwxyz0123456789abcdefghijklmnopqrstuvwxyz0123456789abcdefghijklmnopqrstuvwxyz0123456789abcdefghijklmnopqrstuvwxyz0123456789abcdefghijklmnopqrstuvwxyz0123456789abcdefghijklmnopqrstuvwxyz0123456789abcdefghijklmnopqrstuvwxyz0123456789abcdefghijklmnopqrstuvwxyz0123456789abcdefghijklmnopqrstuvwxyz0123456789abcdefghijklmnopqrstuvwxyz0123456789abcdefghijklmnopqrstuvwxyz0123456789abcdefghijklmnopqrstuvwxyz0123456789abcdefghijklmnopqrstuvwxyz0123456789abcdefghijklmnopqrstuvwxyz0123456789abcdefghijklmnopqrstuvwxyz0123456789abcdefghijklmnopqrstuvwxyz0123456789abcdefghijklmnopqrstuvwxyz0123456789abcdefghijklmnopqrstuvwxyz0123456789abcdefghijklmnopqrstuvwxyz0123456789abcdefghijklmnopqrstuvwxyz0123456789abcdefghijklmnopqrstuvwxyz0123456789abcdefghijklmnopqrstu")]
        internal void ShouldNotCreateClientWithInvalidAccessKey(string chaveAcessoInvalida)
        {
            var builder = ClientBuilderTest.GetBuilder(accessKey: chaveAcessoInvalida);

            var exception = Assert.Throws<ClientException>(builder.BuildNew);

            Assert.Equal(ClientValidationMessages.InvalidAccessKey.Description(), exception.Message);
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        internal void ShouldActiveOrInactiveClient(bool value)
        {
            #region Arrange

            var builder = ClientBuilderTest.GetBuilder();

            var client = builder.BuildNew();

            #endregion

            client.SetActivate(value);

            #region Assert

            Assert.Equal(value, client.Active);
            Assert.Null(client.UpdatedAt);

            #endregion
        }

        [Fact]
        internal void ShouldUpdateAccessKeyClient()
        {
            #region Arrange

            var builder = ClientBuilderTest.GetBuilder();

            var newAccessKey = _faker.Random.AlphaNumeric(32);

            var client = builder.BuildNew();

            #endregion

            client.UpdateAccessKey(newAccessKey);

            #region Assert

            Assert.Equal(newAccessKey, client.AccessKey);
            Assert.Null(client.UpdatedAt);

            #endregion
        }
    }
}

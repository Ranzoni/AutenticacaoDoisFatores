using AutenticacaoDoisFatores.Domain.Shared;
using AutenticacaoDoisFatores.Domain.Shared.Messages;
using AutenticacaoDoisFatores.Domain.Domains;
using AutenticacaoDoisFatores.Domain.Entities;
using AutenticacaoDoisFatores.Domain.Repositories;
using AutenticacaoDoisFatores.Domain.Services;
using AutenticacaoDoisFatores.Service.UseCases.Clients;
using AutenticacaoDoisFatores.Service.Exceptions;
using AutenticacaoDoisFatores.Tests.Shared;
using Bogus;
using Messenger;
using Moq;
using Moq.AutoMock;

namespace AutenticacaoDoisFatores.Tests.Service.Clients
{
    public class CreateClientTest
    {
        private readonly Faker _faker = new();
        private readonly AutoMocker _mocker = new();

        [Fact]
        internal async Task ShouldExecute()
        {
            #region Arrange

            var dtoBuilder = ClientBuilderTest.GetBuilderOfNew();
            var newClient = dtoBuilder.Build();
            var clientBuilder = ClientBuilderTest
                .GetBuilder
                (
                    name: newClient.Name,
                    email: newClient.Email,
                    domainName: newClient.DomainName,
                    accessKey: newClient.DecryptedKey()
                );
            var client = clientBuilder.BuildNew();

            _mocker.CreateInstance<ClientDomain>();
            _mocker.GetMock<IClientRepository>().Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(client);

            var fakeConfirmationLink = _faker.Internet.UrlWithPath();

            var fakeAuthKey = _faker.Random.AlphaNumeric(40);
            Environment.SetEnvironmentVariable("ADF_CHAVE_AUTENTICACAO", fakeAuthKey);

            var service = _mocker.CreateInstance<CreateClient>();

            #endregion Arrange

            var registeredClient = await service.ExecuteAsync(newClient, fakeConfirmationLink);

            #region Assert

            Assert.NotNull(registeredClient);
            Assert.Equal(newClient.Name, registeredClient.Name);
            Assert.Equal(newClient.Email, registeredClient.Email);
            Assert.Equal(newClient.DomainName, registeredClient.DomainName);
            Assert.NotEqual(newClient.AccessKey, newClient.DecryptedKey());
            _mocker.Verify<IClientRepository>(r => r.NewDomainAsync(client.DomainName), Times.Once);
            _mocker.Verify<IClientRepository>(r => r.Add(It.IsAny<Client>()), Times.Once);
            _mocker.Verify<IUserRepository>(r => r.Add(It.IsAny<User>(), client.DomainName), Times.Once);
            _mocker.Verify<IEmailService>(s =>
                s.Send
                    (
                        client.Email,
                        EmailMessages.ClientConfirmationSubject.Description() ?? "",
                        It.IsAny<string>()
                    ),
                Times.Once);
            _mocker.Verify<INotifier>(n => n.AddMessage(It.IsAny<ClientValidationMessages>()), Times.Never);

            #endregion Arrange
        }

        [Fact]
        internal void ShouldMapEncryptedAccessKey()
        {
            #region Arrange

            var dtoBuilder = ClientBuilderTest.GetBuilderOfNew();
            var newClient = dtoBuilder.Build();

            #endregion Arrange

            var client = (Client)(newClient);

            #region Assert

            Assert.NotNull(client);
            Assert.NotEqual(newClient.DecryptedKey(), client.AccessKey);
            Assert.Equal(newClient.AccessKey, client.AccessKey);

            #endregion Arrange
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("ab")]
        [InlineData("abcdefghijklmnopqrstuvwxyz0123456789abcdefghijklmno")]
        internal async Task ShouldReturnNullAndMessageWhenNomeIsInvalid(string nomeInvalido)
        {
            #region Arrange

            var builder = ClientBuilderTest.GetBuilderOfNew(name: nomeInvalido);
            var newClient = builder.Build();

            _mocker.CreateInstance<ClientDomain>();
            _mocker.GetMock<INotifier>().Setup(n => n.AnyMessage()).Returns(true);

            var fakeConfirmationLink = _faker.Internet.UrlWithPath();

            var service = _mocker.CreateInstance<CreateClient>();

            #endregion Arrange

            var registeredClient = await service.ExecuteAsync(newClient, fakeConfirmationLink);

            #region Assert

            Assert.Null(registeredClient);
            _mocker.Verify<IClientRepository>(r => r.Add(It.IsAny<Client>()), Times.Never);
            _mocker.Verify<IEmailService>(s => s.Send(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
            _mocker.Verify<INotifier>(n => n.AddMessage(ClientValidationMessages.InvalidName), Times.Once);

            #endregion Arrange
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("ab")]
        [InlineData("abcdefghijklmnopqrstuvwxyz0123456789abcdefghijklmnopqrstuvwxyz0123456789abcdefghijklmnopqrstuvwxyz0123456789abcdefghijklmnopqrstuvwxyz0123456789abcdefghijklmnopqrstuvwxyz0123456789abcdefghijklmnopqrstuvwxyz0123456789abcdefghijklmnopqrstuvwxyz0123456789abcde")]
        internal async Task ShouldReturnNullAndMessageWhenEmailIsInvalid(string invalidEmail)
        {
            #region Arrange

            var builder = ClientBuilderTest.GetBuilderOfNew(email: invalidEmail);
            var newClient = builder.Build();

            _mocker.CreateInstance<ClientDomain>();
            _mocker.GetMock<INotifier>().Setup(n => n.AnyMessage()).Returns(true);

            var fakeConfirmationLink = _faker.Internet.UrlWithPath();

            var service = _mocker.CreateInstance<CreateClient>();

            #endregion Arrange

            var registeredClient = await service.ExecuteAsync(newClient, fakeConfirmationLink);

            #region Assert

            Assert.Null(registeredClient);
            _mocker.Verify<IClientRepository>(r => r.Add(It.IsAny<Client>()), Times.Never);
            _mocker.Verify<IEmailService>(s => s.Send(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
            _mocker.Verify<INotifier>(n => n.AddMessage(ClientValidationMessages.InvalidEmail), Times.Once);

            #endregion Arrange
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
        internal async Task ShouldReturnNullAndMessageWhenDomainNameIsInvalid(string invalidDomainName)
        {
            #region Arrange

            var builder = ClientBuilderTest.GetBuilderOfNew(domainName: invalidDomainName);
            var newClient = builder.Build();

            _mocker.CreateInstance<ClientDomain>();
            _mocker.GetMock<INotifier>().Setup(n => n.AnyMessage()).Returns(true);

            var fakeConfirmationLink = _faker.Internet.UrlWithPath();

            var service = _mocker.CreateInstance<CreateClient>();

            #endregion Arrange

            var registeredClient = await service.ExecuteAsync(newClient, fakeConfirmationLink);

            #region Assert

            Assert.Null(registeredClient);
            _mocker.Verify<IClientRepository>(r => r.Add(It.IsAny<Client>()), Times.Never);
            _mocker.Verify<IEmailService>(s => s.Send(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
            _mocker.Verify<INotifier>(n => n.AddMessage(ClientValidationMessages.InvalidDomainName), Times.Once);

            #endregion Arrange
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
        internal async Task ShouldReturnNullAndMessageWhenPasswordIsInvalid(string password)
        {
            #region Arrange

            var builder = ClientBuilderTest.GetBuilderOfNew(adminPassword: password);
            var newClient = builder.Build();

            _mocker.CreateInstance<ClientDomain>();
            _mocker.GetMock<INotifier>().Setup(n => n.AnyMessage()).Returns(true);

            var fakeConfirmationLink = _faker.Internet.UrlWithPath();

            var service = _mocker.CreateInstance<CreateClient>();

            #endregion Arrange

            var registeredClient = await service.ExecuteAsync(newClient, fakeConfirmationLink);

            #region Assert

            Assert.Null(registeredClient);
            _mocker.Verify<IClientRepository>(r => r.Add(It.IsAny<Client>()), Times.Never);
            _mocker.Verify<IEmailService>(s => s.Send(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
            _mocker.Verify<INotifier>(n => n.AddMessage(UserValidationMessages.InvalidPassword), Times.Once);

            #endregion Arrange
        }

        [Fact]
        internal async Task ShouldReturnNullAndMessageWhenEmailAlreadyRegistered()
        {
            #region Arrange

            var existingEmail = _faker.Internet.Email();

            var builder = ClientBuilderTest.GetBuilderOfNew(email: existingEmail);
            var newClient = builder.Build();

            _mocker.CreateInstance<ClientDomain>();
            _mocker.GetMock<IClientRepository>().Setup(r => r.EmailExistsAsync(existingEmail)).ReturnsAsync(true);
            _mocker.GetMock<INotifier>().Setup(n => n.AnyMessage()).Returns(true);

            var fakeConfirmationLink = _faker.Internet.UrlWithPath();

            var service = _mocker.CreateInstance<CreateClient>();

            #endregion Arrange

            var registeredClient = await service.ExecuteAsync(newClient, fakeConfirmationLink);

            #region Assert

            Assert.Null(registeredClient);
            _mocker.Verify<IClientRepository>(r => r.Add(It.IsAny<Client>()), Times.Never);
            _mocker.Verify<IEmailService>(s => s.Send(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
            _mocker.Verify<INotifier>(n => n.AddMessage(ClientValidationMessages.EmailAlreadyRegistered), Times.Once);

            #endregion Arrange
        }

        [Fact]
        internal async Task ShouldReturnNullAndMessageWhenDomainNameAlreadyRegistered()
        {
            #region Arrange

            var existingDomainName = _faker.Internet.DomainWord();

            var builder = ClientBuilderTest.GetBuilderOfNew(domainName: existingDomainName);
            var newClient = builder.Build();

            _mocker.CreateInstance<ClientDomain>();
            _mocker.GetMock<IClientRepository>().Setup(r => r.DomainExistsAsync(existingDomainName)).ReturnsAsync(true);
            _mocker.GetMock<INotifier>().Setup(n => n.AnyMessage()).Returns(true);

            var fakeConfirmationLink = _faker.Internet.UrlWithPath();

            var service = _mocker.CreateInstance<CreateClient>();

            #endregion Arrange

            var registeredClient = await service.ExecuteAsync(newClient, fakeConfirmationLink);

            #region Assert

            Assert.Null(registeredClient);
            _mocker.Verify<IClientRepository>(r => r.Add(It.IsAny<Client>()), Times.Never);
            _mocker.Verify<IEmailService>(s => s.Send(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
            _mocker.Verify<INotifier>(n => n.AddMessage(ClientValidationMessages.DomainNameAlreadyRegistered), Times.Once);

            #endregion Arrange
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        internal async Task ShouldReturnExceptionWhenConfirmationLinkNotInformed(string emptyConfirmationLink)
        {
            #region Arrange

            var builder = ClientBuilderTest.GetBuilderOfNew();
            var newClient = builder.Build();

            _mocker.CreateInstance<ClientDomain>();

            var service = _mocker.CreateInstance<CreateClient>();

            #endregion Arrange

            var exception = await Assert.ThrowsAsync<ClientRegisterException>(() => service.ExecuteAsync(newClient, emptyConfirmationLink));

            #region Assert

            Assert.Equal(ClientValidationMessages.ConfirmationLinkNotInformed.Description(), exception.Message);
            _mocker.Verify<IClientRepository>(r => r.Add(It.IsAny<Client>()), Times.Never);
            _mocker.Verify<IEmailService>(s => s.Send(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);

            #endregion Arrange
        }
    }
}

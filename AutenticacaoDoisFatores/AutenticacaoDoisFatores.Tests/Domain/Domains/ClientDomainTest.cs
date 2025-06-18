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
    public class ClientDomainTest
    {
        private readonly Faker _faker = new();
        private readonly AutoMocker _mocker = new();

        #region Creation tests

        [Fact]
        internal async Task ShouldCreateClient()
        {
            #region Arrange

            var builder = ClientBuilderTest.GetBuilder();
            var client = builder.BuildNew();

            var domain = _mocker.CreateInstance<ClientDomain>();

            #endregion Arrange

            var result = await domain.CreateAsync(client);

            #region Assert

            Assert.NotNull(result);
            Assert.Equal(client, result);
            _mocker.Verify<IClientRepository>(r => r.Add(client), Times.Once);
            _mocker.Verify<IClientRepository>(r => r.SaveChangesAsync(), Times.Once);

            #endregion Assert
        }

        [Fact]
        internal async Task ShouldCreateDomainClient()
        {
            #region Arrange

            var builder = ClientBuilderTest.GetBuilder();
            var client = builder.BuildNew();

            var domain = _mocker.CreateInstance<ClientDomain>();

            _mocker.GetMock<IClientRepository>().Setup(r => r.GetByIdAsync(client.Id)).ReturnsAsync(client);

            #endregion Arrange

            await domain.CreateDomainAsync(client.Id);

            #region Assert

            _mocker.Verify<IClientRepository>(r => r.GetByIdAsync(client.Id), Times.Once);
            _mocker.Verify<IClientRepository>(r => r.NewDomainAsync(client.DomainName), Times.Once);

            #endregion Assert
        }

        [Fact]
        internal async Task ShouldReturnExceptionWhenCreateDomainWithInexistingClient()
        {
            #region Arrange

            var testId = Guid.NewGuid();

            var domain = _mocker.CreateInstance<ClientDomain>();

            #endregion Arrange

            var exception = await Assert.ThrowsAsync<ClientException>(() => domain.CreateDomainAsync(testId));

            #region Assert

            Assert.Equal(ClientValidationMessages.ClientNotFound.Description(), exception.Message);
            _mocker.Verify<IClientRepository>(r => r.GetByIdAsync(testId), Times.Once);
            _mocker.Verify<IClientRepository>(r => r.NewDomainAsync(It.IsAny<string>()), Times.Never);

            #endregion Assert
        }

        [Fact]
        internal async Task ShouldReturnExceptionWhenCreateClientWithExistingEmail()
        {
            #region Arrange

            var existentEmail = _faker.Internet.Email();

            var builder = ClientBuilderTest.GetBuilder(email: existentEmail);
            var client = builder.BuildNew();

            var domain = _mocker.CreateInstance<ClientDomain>();
            _mocker.GetMock<IClientRepository>().Setup(r => r.EmailExistsAsync(existentEmail)).ReturnsAsync(true);

            #endregion Arrange

            var exception = await Assert.ThrowsAsync<ClientException>(() => domain.CreateAsync(client));

            #region Assert

            Assert.Equal(ClientValidationMessages.EmailAlreadyRegistered.Description(), exception.Message);
            _mocker.Verify<IClientRepository>(r => r.Add(It.IsAny<Client>()), Times.Never);
            _mocker.Verify<IClientRepository>(r => r.SaveChangesAsync(), Times.Never);

            #endregion Assert
        }

        [Fact]
        internal async Task ShouldReturnExceptionWhenCreateClientWithExistingDomainName()
        {
            #region Arrange

            var existentDomainName = _faker.Internet.DomainWord();

            var builder = ClientBuilderTest.GetBuilder(domainName: existentDomainName);
            var client = builder.BuildNew();

            var domain = _mocker.CreateInstance<ClientDomain>();
            _mocker.GetMock<IClientRepository>().Setup(r => r.DomainExistsAsync(existentDomainName)).ReturnsAsync(true);

            #endregion Arrange

            var exception = await Assert.ThrowsAsync<ClientException>(() => domain.CreateAsync(client));

            #region Assert

            Assert.Equal(ClientValidationMessages.DomainNameAlreadyRegistered.Description(), exception.Message);
            _mocker.Verify<IClientRepository>(r => r.Add(It.IsAny<Client>()), Times.Never);
            _mocker.Verify<IClientRepository>(r => r.SaveChangesAsync(), Times.Never);

            #endregion Assert
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        internal async Task ShouldReturnTrueOrFalseForExistingEmail(bool registeredEmail)
        {
            #region Arrange

            var testEmail = _faker.Internet.Email();

            var domain = _mocker.CreateInstance<ClientDomain>();
            _mocker.GetMock<IClientRepository>().Setup(r => r.EmailExistsAsync(testEmail)).ReturnsAsync(registeredEmail);

            #endregion Arrange

            var result = await domain.EmailExistsAsync(testEmail);

            #region Assert

            Assert.Equal(result, registeredEmail);
            _mocker.Verify<IClientRepository>(r => r.EmailExistsAsync(testEmail), Times.Once);

            #endregion Assert
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        internal async Task ShouldReturnTrueOrFalseForExistingDomainName(bool registeredDomainName)
        {
            #region Arrange

            var testDomainName = _faker.Internet.DomainName();

            var domain = _mocker.CreateInstance<ClientDomain>();
            _mocker.GetMock<IClientRepository>().Setup(r => r.DomainExistsAsync(testDomainName)).ReturnsAsync(registeredDomainName);

            #endregion Arrange

            var result = await domain.DomainExistsAsync(testDomainName);

            #region Assert

            Assert.Equal(result, registeredDomainName);
            _mocker.Verify<IClientRepository>(r => r.DomainExistsAsync(testDomainName), Times.Once);

            #endregion Assert
        }

        #endregion

        #region Change tests

        [Fact]
        internal async Task ShouldUpdateClient()
        {
            #region Arrange

            var builder = ClientBuilderTest.GetBuilder();
            var client = builder.Build();

            var domain = _mocker.CreateInstance<ClientDomain>();

            #endregion Arrange

            var result = await domain.UpdateAsync(client);

            #region Assert

            Assert.NotNull(result);
            Assert.Equal(client, result);
            _mocker.Verify<IClientRepository>(r => r.Update(client), Times.Once);
            _mocker.Verify<IClientRepository>(r => r.SaveChangesAsync(), Times.Once);

            #endregion Assert
        }

        #endregion

        #region Search tests

        [Fact]
        internal async Task ShouldGetById()
        {
            #region Arrange

            var mocker = new AutoMocker();

            var clientId = Guid.NewGuid();
            var client = ClientBuilderTest
                .GetBuilder(id: clientId)
                .Build();

            var domain = mocker.CreateInstance<ClientDomain>();
            mocker.GetMock<IClientRepository>().Setup(r => r.GetByIdAsync(clientId)).ReturnsAsync(client);

            #endregion

            var result = await domain.GetByIdAsync(clientId);

            #region Assert

            Assert.NotNull(result);
            Assert.Equal(client, result);
            mocker.Verify<IClientRepository>(r => r.GetByIdAsync(clientId), Times.Once);

            #endregion
        }

        [Fact]
        internal async Task ShouldReturnNullWhenGetByIdInexistingClient()
        {
            #region Arrange

            var mocker = new AutoMocker();

            var clientId = Guid.NewGuid();

            var domain = mocker.CreateInstance<ClientDomain>();

            #endregion

            var result = await domain.GetByIdAsync(clientId);

            #region Assert

            Assert.Null(result);
            mocker.Verify<IClientRepository>(r => r.GetByIdAsync(clientId), Times.Once);

            #endregion
        }

        [Fact]
        internal async Task ShouldGetAll()
        {
            #region Arrange

            var mocker = new AutoMocker();

            var filters = new ClientFilter();
            var maxRegisters = _faker.Random.Int(2, filters.Quantity);

            var clients = GenerateMany(maxRegisters, active: true);

            var domain = mocker.CreateInstance<ClientDomain>();
            mocker.GetMock<IClientRepository>().Setup(r => r.GetAllAsync(filters)).ReturnsAsync(clients);

            #endregion

            var result = await domain.GetAllAsync(filters);

            #region Assert

            Assert.NotNull(result);
            Assert.Equal(clients, result);
            mocker.Verify<IClientRepository>(r => r.GetAllAsync(filters), Times.Once);

            #endregion
        }

        [Fact]
        internal async Task ShouldReturnEmptyListWhenGetAllWithNoneRegisterFound()
        {
            #region Arrange

            var mocker = new AutoMocker();

            var filters = new ClientFilter();

            var domain = mocker.CreateInstance<ClientDomain>();

            #endregion

            var result = await domain.GetAllAsync(filters);

            #region Assert

            Assert.Empty(result);
            mocker.Verify<IClientRepository>(r => r.GetAllAsync(filters), Times.Once);

            #endregion
        }

        [Fact]
        internal async Task ShouldGetByEmail()
        {
            #region Arrange

            var mocker = new AutoMocker();

            var email = _faker.Person.Email;
            var client = ClientBuilderTest
                .GetBuilder(email: email)
                .Build();

            var domain = mocker.CreateInstance<ClientDomain>();
            mocker.GetMock<IClientRepository>().Setup(r => r.GetByEmailAsync(email)).ReturnsAsync(client);

            #endregion

            var result = await domain.GetByEmailAsync(email);

            #region Assert

            Assert.NotNull(result);
            Assert.Equal(client, result);
            mocker.Verify<IClientRepository>(r => r.GetByEmailAsync(email), Times.Once);

            #endregion
        }

        [Fact]
        internal async Task ShouldReturnNullWhenGetByEmailWithInexistingClient()
        {
            #region Arrange

            var mocker = new AutoMocker();

            var email = _faker.Person.Email;

            var domain = mocker.CreateInstance<ClientDomain>();

            #endregion

            var result = await domain.GetByEmailAsync(email);

            #region Assert

            Assert.Null(result);
            mocker.Verify<IClientRepository>(r => r.GetByEmailAsync(email), Times.Once);

            #endregion
        }

        #endregion

        private static List<Client> GenerateMany(int quantity, bool? active = null)
        {
            var clients = new List<Client>();

            for (var i = 1; i <= quantity; i++)
            {
                var client = ClientBuilderTest
                    .GetBuilder(active: active)
                    .Build();

                clients.Add(client);
            }

            return clients;
        }
    }
}

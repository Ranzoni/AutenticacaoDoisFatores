using AutenticacaoDoisFatores.Domain.Shared.Messages;
using AutenticacaoDoisFatores.Domain.Entities;
using AutenticacaoDoisFatores.Domain.Repositories;
using AutenticacaoDoisFatores.Domain.Services;
using AutenticacaoDoisFatores.Service.UseCases.Clients;
using AutenticacaoDoisFatores.Tests.Shared;
using Bogus;
using Messenger;
using Moq;
using Moq.AutoMock;

namespace AutenticacaoDoisFatores.Tests.Service.Clients
{
    public class ResendClientKeyTest
    {
        private readonly Faker _faker = new();
        private readonly AutoMocker _mock = new();

        [Fact]
        internal async Task ShouldResendClientKey()
        {
            #region Arrange

            var fakeEmail = _faker.Person.Email;
            var fakeUrl = _faker.Internet.UrlRootedPath();
            var client = ClientBuilderTest
                .GetBuilder(email: fakeEmail, active: false)
                .Build();

            _mock.GetMock<IClientRepository>().Setup(r => r.GetByEmailAsync(fakeEmail)).ReturnsAsync(client);

            var fakeTokenIssuer = _faker.Random.AlphaNumeric(40);
            Environment.SetEnvironmentVariable("ADF_EMISSOR_TOKEN", fakeTokenIssuer);
            var fakeTokenAudience = _faker.Random.AlphaNumeric(40);
            Environment.SetEnvironmentVariable("ADF_DESTINATARIO_TOKEN", fakeTokenAudience);
            var fakerAuthKey = _faker.Random.AlphaNumeric(40);
            Environment.SetEnvironmentVariable("ADF_CHAVE_AUTENTICACAO", fakerAuthKey);

            var service = _mock.CreateInstance<ResendClientKey>();

            #endregion

            await service.ResendAsync(fakeEmail, fakeUrl);

            #region Assert

            _mock.Verify<IClientRepository>(r => r.Update(client), Times.Once);
            _mock.Verify<IClientRepository>(r => r.SaveChangesAsync(), Times.Once);
            _mock.Verify<IEmailService>(s => s.Send(client.Email, It.IsAny<string>(), It.IsAny<string>()), Times.Once);

            #endregion
        }

        [Fact]
        internal async Task ShouldNotResendClientKeyWhenClientIdNotExists()
        {
            #region Arrange

            var fakeEmail = _faker.Person.Email;
            var fakeUrl = _faker.Internet.UrlRootedPath();
            var service = _mock.CreateInstance<ResendClientKey>();

            #endregion

            await service.ResendAsync(fakeEmail, fakeUrl);

            #region Assert

            _mock.Verify<IClientRepository>(r => r.Update(It.IsAny<Client>()), Times.Never);
            _mock.Verify<IClientRepository>(r => r.SaveChangesAsync(), Times.Never);
            _mock.Verify<IEmailService>(s => s.Send(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
            _mock.Verify<INotifier>(n => n.AddNotFoundMessage(ClientValidationMessages.ClientNotFound), Times.Once);

            #endregion
        }

        [Fact]
        internal async Task ShouldNotResendClientKeyWhenAlreadyActive()
        {
            #region Arrange

            var fakeEmail = _faker.Person.Email;
            var fakeUrl = _faker.Internet.UrlRootedPath();
            var client = ClientBuilderTest
                .GetBuilder(email: fakeEmail, active: true)
                .Build();

            _mock.GetMock<IClientRepository>().Setup(r => r.GetByEmailAsync(fakeEmail)).ReturnsAsync(client);
            _mock.GetMock<INotifier>().Setup(n => n.AnyMessage()).Returns(true);

            var service = _mock.CreateInstance<ResendClientKey>();

            #endregion

            await service.ResendAsync(fakeEmail, fakeUrl);

            #region Assert

            _mock.Verify<IClientRepository>(r => r.Update(It.IsAny<Client>()), Times.Never);
            _mock.Verify<IClientRepository>(r => r.SaveChangesAsync(), Times.Never);
            _mock.Verify<IEmailService>(s => s.Send(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
            _mock.Verify<INotifier>(n => n.AddMessage(ClientValidationMessages.ClientAlreadyActivated), Times.Once);

            #endregion
        }
    }
}

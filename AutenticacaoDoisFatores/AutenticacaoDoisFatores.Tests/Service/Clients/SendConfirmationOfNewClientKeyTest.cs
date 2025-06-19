using AutenticacaoDoisFatores.Domain.Shared.Messages;
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
    public class SendConfirmationOfNewClientKeyTest
    {
        private readonly AutoMocker _mocker = new();
        private readonly Faker _faker = new();

        [Fact]
        internal async Task ShouldSendConfirmationOfNewClientKey()
        {
            #region Arrange

            var service = _mocker.CreateInstance<SendConfirmationOfNewClientKey>();
            var fakeEmail = _faker.Person.Email;
            var fakeUrl = _faker.Internet.UrlWithPath();
            var client = ClientBuilderTest
                .GetBuilder(email: fakeEmail, active: true)
                .Build();

            _mocker.GetMock<IClientRepository>().Setup(r => r.GetByEmailAsync(fakeEmail)).ReturnsAsync(client);

            var fakeTokenIssuer = _faker.Random.AlphaNumeric(40);
            Environment.SetEnvironmentVariable("ADF_EMISSOR_TOKEN", fakeTokenIssuer);
            var fakeTokenAudience = _faker.Random.AlphaNumeric(40);
            Environment.SetEnvironmentVariable("ADF_DESTINATARIO_TOKEN", fakeTokenAudience);
            var fakerAuthKey = _faker.Random.AlphaNumeric(40);
            Environment.SetEnvironmentVariable("ADF_CHAVE_AUTENTICACAO", fakerAuthKey);

            #endregion

            await service.SendAsync(fakeEmail, fakeUrl);

            #region Assert

            _mocker.Verify<IEmailService>(e => e.Send(fakeEmail, It.IsAny<string>(), It.IsAny<string>()), Times.Once);

            #endregion
        }

        [Fact]
        internal async Task ShouldNotSendConfirmationOfNewClientKeyWhenClientNotExists()
        {
            #region Arrange

            var service = _mocker.CreateInstance<SendConfirmationOfNewClientKey>();
            var fakeEmail = _faker.Person.Email;
            var fakeUrl = _faker.Internet.UrlWithPath();

            _mocker.GetMock<INotifier>().Setup(n => n.AnyMessage()).Returns(true);

            #endregion

            await service.SendAsync(fakeEmail, fakeUrl);

            #region Assert

            _mocker.Verify<IEmailService>(e => e.Send(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
            _mocker.Verify<INotifier>(n => n.AddNotFoundMessage(ClientValidationMessages.ClientNotFound), Times.Once);

            #endregion
        }

        [Fact]
        internal async Task ShouldNotSendConfirmationOfNewClientKeyWhenClientIsEmpty()
        {
            #region Arrange

            var service = _mocker.CreateInstance<SendConfirmationOfNewClientKey>();
            var fakeEmail = _faker.Person.Email;
            var fakeUrl = _faker.Internet.UrlWithPath();
            var client = ClientBuilderTest
                .GetBuilder(email: fakeEmail, active: false)
                .Build();

            _mocker.GetMock<IClientRepository>().Setup(r => r.GetByEmailAsync(fakeEmail)).ReturnsAsync(client);
            _mocker.GetMock<INotifier>().Setup(n => n.AnyMessage()).Returns(true);

            #endregion

            await service.SendAsync(fakeEmail, fakeUrl);

            #region Assert

            _mocker.Verify<IEmailService>(e => e.Send(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
            _mocker.Verify<INotifier>(n => n.AddMessage(ClientValidationMessages.ClientNotActive), Times.Once);

            #endregion
        }
    }
}

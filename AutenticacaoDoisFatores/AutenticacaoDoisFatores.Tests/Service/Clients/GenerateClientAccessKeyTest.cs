using AutenticacaoDoisFatores.Domain.Shared.Messages;
using AutenticacaoDoisFatores.Domain.Entities;
using AutenticacaoDoisFatores.Domain.Repositories;
using AutenticacaoDoisFatores.Domain.Services;
using AutenticacaoDoisFatores.Service.UseCases.Clients;
using AutenticacaoDoisFatores.Tests.Shared;
using Messenger;
using Moq;
using Moq.AutoMock;

namespace AutenticacaoDoisFatores.Tests.Service.Clients
{
    public class GenerateClientAccessKeyTest
    {
        private readonly AutoMocker _mocker = new();

        [Fact]
        internal async Task ShouldGenerateNewAccessKey()
        {
            #region Arrange

            var service = _mocker.CreateInstance<GenerateClientAccessKey>();
            var fakeClientId = Guid.NewGuid();
            var client = ClientBuilderTest
                .GetBuilder(id: fakeClientId, active: true)
                .Build();

            _mocker.GetMock<IClientRepository>().Setup(r => r.GetByIdAsync(fakeClientId)).ReturnsAsync(client);

            #endregion

            await service.GenerateAsync(fakeClientId);

            #region Assert

            _mocker.Verify<IClientRepository>(r => r.Update(client), Times.Once);
            _mocker.Verify<IClientRepository>(r => r.SaveChangesAsync(), Times.Once);
            _mocker.Verify<IEmailService>(e => e.Send(client.Email, It.IsAny<string>(), It.IsAny<string>()), Times.Once);

            #endregion
        }

        [Fact]
        internal async Task ShouldNotGenerateNewAccessKeyWhenClientNotExists()
        {
            #region Arrange

            var service = _mocker.CreateInstance<GenerateClientAccessKey>();
            var fakeClientId = Guid.NewGuid();

            #endregion

            await service.GenerateAsync(fakeClientId);

            #region Assert

            _mocker.Verify<IClientRepository>(r => r.Update(It.IsAny<Client>()), Times.Never);
            _mocker.Verify<IClientRepository>(r => r.SaveChangesAsync(), Times.Never);
            _mocker.Verify<IEmailService>(e => e.Send(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
            _mocker.Verify<INotifier>(n => n.AddNotFoundMessage(ClientValidationMessages.ClientNotFound), Times.Once);

            #endregion
        }

        [Fact]
        internal async Task ShouldNotGenerateNewAccessKeyWhenClientIsNotActive()
        {
            #region Arrange

            var service = _mocker.CreateInstance<GenerateClientAccessKey>();
            var fakeClientId = Guid.NewGuid();
            var client = ClientBuilderTest
                .GetBuilder(id: fakeClientId, active: false)
                .Build();

            _mocker.GetMock<IClientRepository>().Setup(r => r.GetByIdAsync(fakeClientId)).ReturnsAsync(client);
            _mocker.GetMock<INotifier>().Setup(n => n.AnyMessage()).Returns(true);

            #endregion

            await service.GenerateAsync(fakeClientId);

            #region Assert

            _mocker.Verify<IClientRepository>(r => r.Update(It.IsAny<Client>()), Times.Never);
            _mocker.Verify<IClientRepository>(r => r.SaveChangesAsync(), Times.Never);
            _mocker.Verify<IEmailService>(e => e.Send(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
            _mocker.Verify<INotifier>(n => n.AddMessage(ClientValidationMessages.ClientNotActive), Times.Once);

            #endregion
        }
    }
}

using AutenticacaoDoisFatores.Domain.Entities;
using AutenticacaoDoisFatores.Domain.Repositories;
using AutenticacaoDoisFatores.Service.UseCases.Clients;
using AutenticacaoDoisFatores.Tests.Shared;
using AutenticacaoDoisFatores.Domain.Shared.Messages;
using Messenger;
using Moq;
using Moq.AutoMock;

namespace AutenticacaoDoisFatores.Tests.Service.Clients
{
    public class ActivateClientTest
    {
        private readonly AutoMocker _mocker = new();

        [Fact]
        internal async Task ShouldActivateClient()
        {
            #region Arrange

            var client = ClientBuilderTest.GetBuilder(active: false).Build();

            _mocker.GetMock<IClientRepository>().Setup(r => r.GetByIdAsync(client.Id)).ReturnsAsync(client);

            var service = _mocker.CreateInstance<ActivateClient>();

            #endregion

            await service.ActivateAsync(client.Id);

            #region Assert

            Assert.True(client.Active);
            _mocker.Verify<IClientRepository>(r => r.Update(client), Times.Once);
            _mocker.Verify<IClientRepository>(r => r.SaveChangesAsync(), Times.Once);

            #endregion
        }

        [Fact]
        internal async Task ShouldNotActivateClientWhenClientIdNotExists()
        {
            #region Arrange

            _mocker.GetMock<INotifier>().Setup(n => n.AnyMessage()).Returns(true);

            var service = _mocker.CreateInstance<ActivateClient>();

            #endregion

            await service.ActivateAsync(Guid.NewGuid());

            #region Assert

            _mocker.Verify<IClientRepository>(r => r.Update(It.IsAny<Client>()), Times.Never);
            _mocker.Verify<IClientRepository>(r => r.SaveChangesAsync(), Times.Never);
            _mocker.Verify<INotifier>(n => n.AddNotFoundMessage(ClientValidationMessages.ClientNotFound), Times.Once);

            #endregion
        }

        [Fact]
        internal async Task ShouldNotActivateClientWhenClientIsActivated()
        {
            #region Arrange

            var client = ClientBuilderTest.GetBuilder(active: true).Build();

            _mocker.GetMock<IClientRepository>().Setup(r => r.GetByIdAsync(client.Id)).ReturnsAsync(client);
            _mocker.GetMock<INotifier>().Setup(n => n.AnyMessage()).Returns(true);

            var service = _mocker.CreateInstance<ActivateClient>();

            #endregion

            await service.ActivateAsync(client.Id);

            #region Assert

            _mocker.Verify<IClientRepository>(r => r.Update(It.IsAny<Client>()), Times.Never);
            _mocker.Verify<IClientRepository>(r => r.SaveChangesAsync(), Times.Never);
            _mocker.Verify<INotifier>(n => n.AddMessage(ClientValidationMessages.ClientAlreadyActivated), Times.Once);

            #endregion
        }
    }
}

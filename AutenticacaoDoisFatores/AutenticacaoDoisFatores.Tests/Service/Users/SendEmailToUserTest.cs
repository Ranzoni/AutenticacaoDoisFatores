using AutenticacaoDoisFatores.Domain.Shared.Messages;
using AutenticacaoDoisFatores.Domain.Repositories;
using AutenticacaoDoisFatores.Domain.Services;
using AutenticacaoDoisFatores.Service.UseCases.Users;
using AutenticacaoDoisFatores.Tests.Shared;
using Bogus;
using Messenger;
using Moq;
using Moq.AutoMock;

namespace AutenticacaoDoisFatores.Tests.Service.Users
{
    public class SendEmailToUserTest
    {
        [Fact]
        internal async Task ShouldSendEmail()
        {
            #region Arrange

            var mocker = new AutoMocker();

            var service = mocker.CreateInstance<SendEmailToUser>();

            var faker = new Faker();

            var userId = Guid.NewGuid();

            var user = UserBuilderTest
                .GetBuilder(id: userId, active: false)
                .Build();

            var senderUserEmail = SenderUserEmailBuilderTest
                .GetBuilder()
                .Build();

            mocker.GetMock<IUserRepository>().Setup(r => r.GetByIdAsync(userId)).ReturnsAsync(user);

            #endregion

            await service.ExecuteAsync(userId, senderUserEmail);

            #region Assert

            mocker.Verify<IEmailService>(s => s.Send(user.Email, It.IsAny<string>(), It.IsAny<string>()), Times.Once);

            #endregion
        }

        [Fact]
        internal async Task ShouldNotSendEmailToUserNotRegistered()
        {
            #region Arrange

            var mocker = new AutoMocker();

            var service = mocker.CreateInstance<SendEmailToUser>();

            var faker = new Faker();

            var userId = Guid.NewGuid();

            var senderUserEmail = SenderUserEmailBuilderTest
                .GetBuilder()
                .Build();

            #endregion

            await service.ExecuteAsync(userId, senderUserEmail);

            #region Assert

            mocker.Verify<IEmailService>(s => s.Send(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
            mocker.Verify<INotifier>(n => n.AddNotFoundMessage(UserValidationMessages.UserNotFound), Times.Once);

            #endregion
        }

        [Fact]
        internal async Task ShouldNotSendEmailToAdminUser()
        {
            #region Arrange

            var mocker = new AutoMocker();

            var service = mocker.CreateInstance<SendEmailToUser>();

            var faker = new Faker();

            var userId = Guid.NewGuid();

            var senderUserEmail = SenderUserEmailBuilderTest
                .GetBuilder()
                .Build();

            var user = UserBuilderTest
                .GetBuilder(id: userId, isAdmin: true)
                .Build();

            mocker.GetMock<IUserRepository>().Setup(r => r.GetByIdAsync(userId)).ReturnsAsync(user);

            #endregion

            await service.ExecuteAsync(userId, senderUserEmail);

            #region Assert

            mocker.Verify<IEmailService>(s => s.Send(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
            mocker.Verify<INotifier>(n => n.AddNotFoundMessage(UserValidationMessages.UserNotFound), Times.Once);

            #endregion
        }
    }
}

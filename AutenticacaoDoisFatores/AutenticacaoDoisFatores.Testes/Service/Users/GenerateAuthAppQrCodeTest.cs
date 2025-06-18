using AutenticacaoDoisFatores.Domain.Domains;
using AutenticacaoDoisFatores.Tests.Shared;
using Bogus;
using Messenger;
using Moq.AutoMock;
using Moq;
using AutenticacaoDoisFatores.Service.UseCases.Users.Authenticators;
using AutenticacaoDoisFatores.Domain.Repositories;
using AutenticacaoDoisFatores.Domain.Shared.Messages;
using AutenticacaoDoisFatores.Domain.Services;

namespace AutenticacaoDoisFatores.Tests.Service.Users
{
    public class GenerateAuthAppQrCodeTest
    {
        [Fact]
        internal async Task ShouldGenerateQrCode()
        {
            #region Arrange

            var mocker = new AutoMocker();

            var authApp = mocker.GetMock<AuthApp>().Object;
            var userDomain = mocker.GetMock<UserDomain>().Object;
            var email = mocker.GetMock<EmailSender>().Object;
            var notifier = mocker.GetMock<INotifier>().Object;
            var baseLinkToGenerateQrCode = "https://example.com/qrcode";

            var service = new GenerateAuthAppQrCode(authApp, userDomain, email, notifier, baseLinkToGenerateQrCode);

            var user = UserBuilderTest
                .GetBuilder(active: true)
                .Build();

            var faker = new Faker();

            var qrCode = faker.Commerce.Ean13();

            mocker.GetMock<IUserRepository>().Setup(r => r.GetByIdAsync(user.Id)).ReturnsAsync(user);
            mocker.GetMock<IAuthService>().Setup(s => s.GenerateQrCode(user.Email, user.SecretKey)).Returns(qrCode);

            #endregion

            await service.ExecuteAsync(user.Id);

            #region Assert

            mocker.Verify<IEmailService>(s => s.Send(user.Email, It.IsAny<string>(), It.IsAny<string>()), Times.Once);
            mocker.Verify<IAuthService>(s => s.GenerateQrCode(user.Email, user.SecretKey), Times.Once);

            #endregion
        }

        [Fact]
        internal async Task ShouldNotAuthenticateByAppWhenUserNotRegistered()
        {
            #region Arrange

            var mocker = new AutoMocker();

            var authApp = mocker.GetMock<AuthApp>().Object;
            var userDomain = mocker.GetMock<UserDomain>().Object;
            var email = mocker.GetMock<EmailSender>().Object;
            var notifier = mocker.GetMock<INotifier>().Object;
            var baseLinkToGenerateQrCode = "https://example.com/qrcode";

            var service = new GenerateAuthAppQrCode(authApp, userDomain, email, notifier, baseLinkToGenerateQrCode);

            mocker.GetMock<INotifier>().Setup(n => n.AnyMessage()).Returns(true);

            #endregion

            await service.ExecuteAsync(Guid.NewGuid());

            #region Assert

            mocker.Verify<IAuthService>(s => s.GenerateQrCode(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
            mocker.Verify<IEmailService>(s => s.Send(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
            mocker.Verify<INotifier>(n => n.AddNotFoundMessage(UserValidationMessages.UserNotFound), Times.Once);

            #endregion
        }

        [Fact]
        internal async Task ShouldNotAuthenticateByAppWhenUserNotActive()
        {
            #region Arrange

            var mocker = new AutoMocker();

            var authApp = mocker.GetMock<AuthApp>().Object;
            var userDomain = mocker.GetMock<UserDomain>().Object;
            var email = mocker.GetMock<EmailSender>().Object;
            var notifier = mocker.GetMock<INotifier>().Object;
            var baseLinkToGenerateQrCode = "https://example.com/qrcode";

            var service = new GenerateAuthAppQrCode(authApp, userDomain, email, notifier, baseLinkToGenerateQrCode);

            var user = UserBuilderTest
                .GetBuilder(active: false)
                .Build();

            mocker.GetMock<IUserRepository>().Setup(r => r.GetByIdAsync(user.Id)).ReturnsAsync(user);
            mocker.GetMock<INotifier>().Setup(n => n.AnyMessage()).Returns(true);

            #endregion

            await service.ExecuteAsync(user.Id);

            #region Assert

            mocker.Verify<IAuthService>(s => s.GenerateQrCode(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
            mocker.Verify<IEmailService>(s => s.Send(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
            mocker.Verify<INotifier>(n => n.AddNotFoundMessage(UserValidationMessages.UserNotFound), Times.Once);

            #endregion
        }
    }
}

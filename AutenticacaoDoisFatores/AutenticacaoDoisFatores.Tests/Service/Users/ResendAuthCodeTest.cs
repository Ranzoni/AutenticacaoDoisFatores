using AutenticacaoDoisFatores.Domain.Domains;
using AutenticacaoDoisFatores.Domain.Repositories;
using AutenticacaoDoisFatores.Domain.Services;
using AutenticacaoDoisFatores.Domain.Shared.Messages;
using AutenticacaoDoisFatores.Service.UseCases.Users.Authenticators;
using AutenticacaoDoisFatores.Service.UseCases.Users.Authenticators.TwoFactorAuths;
using AutenticacaoDoisFatores.Tests.Shared;
using Messenger;
using Moq;
using Moq.AutoMock;

namespace AutenticacaoDoisFatores.Tests.Service.Users
{
    public class ResendAuthCodeTest
    {
        [Fact]
        internal async Task ShouldResendAuthCode()
        {
            #region Arrange

            var mocker = new AutoMocker();

            mocker.Use(mocker.CreateInstance<UserDomain>());
            mocker.Use(mocker.CreateInstance<EmailSender>());
            mocker.Use(mocker.CreateInstance<AuthCodeManager>());
            mocker.Use(mocker.CreateInstance<UserTwoFactorAuthByEmail>());
            var service = mocker.CreateInstance<ResendAuthCode>();

            var user = UserBuilderTest
                .GetBuilder(active: true)
                .Build();

            mocker.GetMock<IUserRepository>()
                .Setup(r => r.GetByIdAsync(user.Id))
                .ReturnsAsync(user);

            #endregion

            var response = await service.ResendAsync(user.Id);

            #region Assert

            Assert.NotNull(response);
            Assert.NotEmpty(response.Token);
            mocker.Verify<IAuthCodeRepository>(r => r.SaveAsync(user.Id, It.IsAny<string>()), Times.Once);
            mocker.Verify<IEmailService>(s => s.Send(user.Email, It.IsAny<string>(), It.IsAny<string>()), Times.Once);

            #endregion
        }

        [Fact]
        internal async Task ShouldNotResendAuthCodeWhenUserNotExists()
        {
            #region Arrange

            var mocker = new AutoMocker();

            mocker.Use(mocker.CreateInstance<UserDomain>());
            mocker.Use(mocker.CreateInstance<EmailSender>());
            mocker.Use(mocker.CreateInstance<AuthCodeManager>());
            mocker.Use(mocker.CreateInstance<UserTwoFactorAuthByEmail>());
            var service = mocker.CreateInstance<ResendAuthCode>();

            #endregion

            var response = await service.ResendAsync(Guid.NewGuid());

            #region Assert

            Assert.Null(response);
            mocker.Verify<INotifier>(n => n.AddNotFoundMessage(UserValidationMessages.UserNotFound), Times.Once);
            mocker.Verify<IAuthCodeRepository>(r => r.SaveAsync(It.IsAny<Guid>(), It.IsAny<string>()), Times.Never);
            mocker.Verify<IEmailService>(s => s.Send(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);

            #endregion
        }

        [Fact]
        internal async Task ShouldNotResendAuthCodeWhenUserIsNotActive()
        {
            #region Arrange

            var mocker = new AutoMocker();

            mocker.Use(mocker.CreateInstance<UserDomain>());
            mocker.Use(mocker.CreateInstance<EmailSender>());
            mocker.Use(mocker.CreateInstance<AuthCodeManager>());
            mocker.Use(mocker.CreateInstance<UserTwoFactorAuthByEmail>());
            var service = mocker.CreateInstance<ResendAuthCode>();

            var user = UserBuilderTest
                .GetBuilder(active: false)
                .Build();

            mocker.GetMock<IUserRepository>()
                .Setup(r => r.GetByIdAsync(user.Id))
                .ReturnsAsync(user);

            #endregion

            var response = await service.ResendAsync(user.Id);

            #region Assert

            Assert.Null(response);
            mocker.Verify<INotifier>(n => n.AddNotFoundMessage(UserValidationMessages.UserNotFound), Times.Once);
            mocker.Verify<IAuthCodeRepository>(r => r.SaveAsync(It.IsAny<Guid>(), It.IsAny<string>()), Times.Never);
            mocker.Verify<IEmailService>(s => s.Send(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);

            #endregion
        }
    }
}

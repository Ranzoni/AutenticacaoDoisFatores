using AutenticacaoDoisFatores.Domain.Repositories;
using AutenticacaoDoisFatores.Domain.Services;
using AutenticacaoDoisFatores.Domain.Shared;
using AutenticacaoDoisFatores.Domain.Shared.Messages;
using AutenticacaoDoisFatores.Service.UseCases.Users.Authenticators.TwoFactorAuths;
using AutenticacaoDoisFatores.Tests.Shared;
using Bogus;
using Messenger;
using Moq;
using Moq.AutoMock;

namespace AutenticacaoDoisFatores.Tests.Service.Users
{
    public class UserTwoFactorAuthByEmailTest
    {
        [Fact]
        internal async Task ShouldSendEmail()
        {
            #region Arrange

            var mocker = new AutoMocker();

            var service = mocker.CreateInstance<UserTwoFactorAuthByEmail>();

            var user = UserBuilderTest
                .GetBuilder(active: true)
                .Build();

            var faker = new Faker();
            var fakeTokenIssuer = faker.Random.AlphaNumeric(40);
            Environment.SetEnvironmentVariable("ADF_EMISSOR_TOKEN", fakeTokenIssuer);
            var fakeTokenAudience = faker.Random.AlphaNumeric(40);
            Environment.SetEnvironmentVariable("ADF_DESTINATARIO_TOKEN", fakeTokenAudience);
            var fakerAuthKey = faker.Random.AlphaNumeric(40);
            Environment.SetEnvironmentVariable("ADF_CHAVE_AUTENTICACAO", fakerAuthKey);

            #endregion

            var response = await service.SendAsync(user);

            #region Assert

            Assert.NotNull(response);
            Assert.False(response.Token.IsNullOrEmptyOrWhiteSpaces());
            mocker.Verify<IAuthCodeRepository>(r => r.SaveAsync(user.Id, It.IsAny<string>()), Times.Once);
            mocker.Verify<IEmailService>(s => s.Send(user.Email, It.IsAny<string>(), It.IsAny<string>()), Times.Once);

            #endregion
        }

        [Fact]
        internal async Task ShouldNotSendEmailToInactiveUser()
        {
            #region Arrange

            var mocker = new AutoMocker();

            var service = mocker.CreateInstance<UserTwoFactorAuthByEmail>();

            var user = UserBuilderTest
                .GetBuilder(active: false)
                .Build();

            #endregion

            var response = await service.SendAsync(user);

            #region Assert

            Assert.Null(response);
            mocker.Verify<IAuthCodeRepository>(r => r.SaveAsync(It.IsAny<Guid>(), It.IsAny<string>()), Times.Never);
            mocker.Verify<IEmailService>(s => s.Send(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
            mocker.Verify<INotifier>(n => n.AddNotFoundMessage(UserValidationMessages.UserNotFound), Times.Once);

            #endregion
        }
    }
}

using AutenticacaoDoisFatores.Service.UseCases.Users.Authenticators.TwoFactorAuths;
using AutenticacaoDoisFatores.Tests.Shared;
using Bogus;
using Messenger;
using Moq.AutoMock;

namespace AutenticacaoDoisFatores.Tests.Service.Users
{
    public class UserTwoFactorAuthByAppTest
    {
        [Fact]
        internal async Task ShouldAuthenticate()
        {
            #region Arrange

            var mocker = new AutoMocker();

            var notifier = mocker.GetMock<INotifier>().Object;
            
            var service = new UserTwoFactorAuthByApp(notifier);

            var user = UserBuilderTest
                .GetBuilder(active: true)
                .Build();

            var faker = new Faker();
            var fakeTokenIssuer = faker.Random.AlphaNumeric(40);
            Environment.SetEnvironmentVariable("ADF_EMISSOR_TOKEN", fakeTokenIssuer);
            var fakeTokenAudience = faker.Random.AlphaNumeric(40);
            Environment.SetEnvironmentVariable("ADF_DESTINATARIO_TOKEN", fakeTokenAudience);

            #endregion

            var response = await service.SendAsync(user);

            #region Assert

            Assert.NotNull(response);
            Assert.NotEmpty(response.Token);

            #endregion
        }

        [Fact]
        internal async Task ShouldNotAuthenticateWhenUserIsNotActive()
        {
            #region Arrange

            var mocker = new AutoMocker();

            var notifier = mocker.GetMock<INotifier>().Object;

            var service = new UserTwoFactorAuthByApp(notifier);

            var user = UserBuilderTest
                .GetBuilder(active: false)
                .Build();

            mocker.GetMock<INotifier>().Setup(n => n.AnyMessage()).Returns(true);

            #endregion

            var response = await service.SendAsync(user);

            #region Assert

            Assert.Null(response);

            #endregion
        }
    }
}

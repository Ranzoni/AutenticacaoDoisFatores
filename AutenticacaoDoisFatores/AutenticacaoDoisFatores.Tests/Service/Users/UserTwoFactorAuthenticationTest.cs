using AutenticacaoDoisFatores.Domain.Repositories;
using AutenticacaoDoisFatores.Domain.Shared.Users;
using AutenticacaoDoisFatores.Service.Dtos.Users;
using AutenticacaoDoisFatores.Service.UseCases.Users.Authenticators;
using AutenticacaoDoisFatores.Service.UseCases.Users.Authenticators.TwoFactorAuths;
using AutenticacaoDoisFatores.Tests.Shared;
using Bogus;
using Moq;
using Moq.AutoMock;

namespace AutenticacaoDoisFatores.Tests.Service.Users
{
    public class UserTwoFactorAuthenticationTest
    {
        [Fact]
        internal async Task ShouldSendCode()
        {
            #region Arrange

            var mocker = new AutoMocker();

            var service = mocker.CreateInstance<UserTwoFactorAuthentication>();

            var userTwoFactorAuthByEmail= mocker.CreateInstance<UserTwoFactorAuthByEmail>();
            mocker.GetMock<IServiceProvider>().Setup(s => s.GetService(typeof(UserTwoFactorAuthByEmail))).Returns(userTwoFactorAuthByEmail);

            var usuario = UserBuilderTest
                .GetBuilder(active: true, authType: AuthType.Email)
                .Build();

            var faker = new Faker();
            var fakeTokenIssuer = faker.Random.AlphaNumeric(40);
            Environment.SetEnvironmentVariable("ADF_EMISSOR_TOKEN", fakeTokenIssuer);

            #endregion

            var response =  await service.ExecuteAsync(usuario);

            #region Assert

            Assert.NotNull(response);
            Assert.NotEmpty((response as TwoFactorAuthResponse)!.Token);
            mocker.Verify<IAuthCodeRepository>(r => r.SaveAsync(usuario.Id, It.IsAny<string>()), Times.Once);

            #endregion
        }
    }
}

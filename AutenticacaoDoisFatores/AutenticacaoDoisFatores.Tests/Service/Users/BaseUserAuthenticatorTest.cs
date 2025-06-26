using AutenticacaoDoisFatores.Domain.Entities;
using AutenticacaoDoisFatores.Domain.Repositories;
using AutenticacaoDoisFatores.Domain.Shared.Messages;
using AutenticacaoDoisFatores.Service.Dtos.Users;
using AutenticacaoDoisFatores.Service.UseCases.Users.Authenticators;
using AutenticacaoDoisFatores.Tests.Shared;
using Bogus;
using Messenger;
using Moq;
using Moq.AutoMock;

namespace AutenticacaoDoisFatores.Tests.Service.Users
{
    public class BaseUserAuthenticatorTest
    {
        [Fact]
        internal async Task ShouldReturnAuthenticatedUser()
        {
            #region Arrange

            var mocker = new AutoMocker();

            var service = mocker.CreateInstance<BaseUserAuthenticator>();

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

            var response = await service.ExecuteAsync(user);

            #region Assert

            Assert.NotNull(response);
            Assert.IsType<AuthenticatedUser>(response);
            Assert.NotEmpty((response as AuthenticatedUser)!.Token);
            mocker.Verify<IUserRepository>(r => r.Update(user), Times.Once);
            mocker.Verify<IUserRepository>(r => r.SaveChangesAsync(), Times.Once);

            #endregion
        }

        [Fact]
        internal async Task ShouldNotReturnAuhtenticatedUserWhenIsNotActive()
        {
            #region Arrange

            var mocker = new AutoMocker();

            var service = mocker.CreateInstance<BaseUserAuthenticator>();

            var user = UserBuilderTest
                .GetBuilder(active: false)
                .Build();

            #endregion

            var response = await service.ExecuteAsync(user);

            #region Assert

            Assert.Null(response);
            mocker.Verify<IUserRepository>(r => r.Update(It.IsAny<User>()), Times.Never);
            mocker.Verify<IUserRepository>(r => r.SaveChangesAsync(), Times.Never);
            mocker.Verify<INotifier>(n => n.AddNotFoundMessage(UserValidationMessages.UserNotFound), Times.Once);

            #endregion
        }
    }
}

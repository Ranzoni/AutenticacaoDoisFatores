using AutenticacaoDoisFatores.Domain.Shared;
using AutenticacaoDoisFatores.Domain.Shared.Messages;
using AutenticacaoDoisFatores.Domain.Domains;
using AutenticacaoDoisFatores.Domain.Exceptions;
using AutenticacaoDoisFatores.Domain.Services;
using AutenticacaoDoisFatores.Tests.Shared;
using Bogus;
using Moq;
using Moq.AutoMock;

namespace AutenticacaoDoisFatores.Tests.Domain.Domains
{
    public class AuthAppDomainTest
    {
        [Fact]
        internal void ShouldGenerateQrCode()
        {
            #region Arrange

            var mocker = new AutoMocker();

            var domain = mocker.CreateInstance<AuthApp>();

            var user = UserBuilderTest
                .GetBuilder()
                .Build();

            var faker = new Faker();

            var qrCode = faker.Commerce.Ean13();

            mocker.GetMock<IAuthService>().Setup(s => s.GenerateQrCode(user.Email, user.SecretKey)).Returns(qrCode);

            #endregion

            var result = domain.GenerateQrCode(user);

            #region Assert

            Assert.NotNull(result);
            Assert.Equal(qrCode, result);
            mocker.Verify<IAuthService>(s => s.GenerateQrCode(user.Email, user.SecretKey), Times.Once);

            #endregion
        }

        [Fact]
        internal void ShouldNotGenerateQrCodeToNullUser()
        {
            #region Arrange

            var mocker = new AutoMocker();

            var domain = mocker.CreateInstance<AuthApp>();

            #endregion

            var exception = Assert.Throws<UserException>(() => domain.GenerateQrCode(null));

            #region Assert

            Assert.Equal(UserValidationMessages.UserNotFound.Description(), exception.Message);
            mocker.Verify<IAuthService>(s => s.GenerateQrCode(It.IsAny<string>(), It.IsAny<string>()), Times.Never);

            #endregion
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        internal void ShouldReturnIfCodeIsValid(bool expectedValue)
        {
            #region Arrange

            var mocker = new AutoMocker();

            var domain = mocker.CreateInstance<AuthApp>();

            var user = UserBuilderTest
                .GetBuilder()
                .Build();

            var code = "123456";
            mocker.GetMock<IAuthService>().Setup(s => s.IsCodeValid(code, user.SecretKey)).Returns(expectedValue);

            #endregion

            var result = domain.IsCodeValid(code, user);

            #region Assert

            Assert.Equal(expectedValue, result);
            mocker.Verify<IAuthService, bool>(s => s.IsCodeValid(code, user.SecretKey), Times.Once);

            #endregion
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        internal void ShouldReturnExceptionWheCheckCodeIsValidToEmptyValue(string? code)
        {
            #region Arrange

            var mocker = new AutoMocker();

            var domain = mocker.CreateInstance<AuthApp>();

            var user = UserBuilderTest
                .GetBuilder()
                .Build();

            #endregion

            var exception = Assert.Throws<AuthAppException>(() => domain.IsCodeValid(code, user));

            #region Assert

            Assert.Equal(AuthAppValidationMessages.CodeNotInformed.Description(), exception.Message);
            mocker.Verify<IAuthService, bool>(s => s.IsCodeValid(It.IsAny<string>(), It.IsAny<string>()), Times.Never);

            #endregion
        }
    }
}

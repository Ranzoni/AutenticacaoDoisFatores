using AutenticacaoDoisFatores.Domain.Shared;
using AutenticacaoDoisFatores.Domain.Shared.Messages;
using AutenticacaoDoisFatores.Domain.Domains;
using AutenticacaoDoisFatores.Domain.Exceptions;
using AutenticacaoDoisFatores.Domain.Repositories;
using AutenticacaoDoisFatores.Service.Shared;
using Moq;
using Moq.AutoMock;

namespace AutenticacaoDoisFatores.Tests.Domain.Domains
{
    public class AuthCodeManagerTest
    {
        [Fact]
        internal async Task ShouldSave()
        {
            #region Arrange

            var mocker = new AutoMocker();

            var domain = mocker.CreateInstance<AuthCodeManager>();

            var userId = Guid.NewGuid();
            var authCode = Security.GenerateAuthCode();

            #endregion

            await domain.SaveAsync(userId, authCode);

            #region Assert

            mocker.Verify<IAuthCodeRepository>(r => r.SaveAsync(userId, authCode), Times.Once);

            #endregion
        }

        [Theory]
        [InlineData("")]
        [InlineData("  ")]
        internal async Task ShouldReturnExceptionWhenSaveWithEmptyCode(string emptyCode)
        {
            #region Arrange

            var mocker = new AutoMocker();

            var domain = mocker.CreateInstance<AuthCodeManager>();

            var userId = Guid.NewGuid();

            #endregion

            var exception = await Assert.ThrowsAsync<AuthCodeException>(() => domain.SaveAsync(userId, emptyCode));

            #region Assert

            Assert.NotNull(exception);
            Assert.Equal(AuthCodeValidationMessages.EmptyAuthCode.Description(), exception.Message);
            mocker.Verify<IAuthCodeRepository>(r => r.SaveAsync(It.IsAny<Guid>(), It.IsAny<string>()), Times.Never);

            #endregion
        }

        [Fact]
        internal async Task ShouldGetCode()
        {
            #region Arrange

            var mocker = new AutoMocker();

            var domain = mocker.CreateInstance<AuthCodeManager>();

            var userId = Guid.NewGuid();
            var authCode = Security.GenerateAuthCode();

            mocker.GetMock<IAuthCodeRepository>().Setup(r => r.GetCodeByUserIdAsync(userId)).ReturnsAsync(authCode);

            #endregion

            var result = await domain.GetCodeAsync(userId);

            #region Assert

            Assert.NotNull(result);
            Assert.Equal(authCode, result);
            mocker.Verify<IAuthCodeRepository>(r => r.GetCodeByUserIdAsync(userId), Times.Once);

            #endregion
        }

        [Fact]
        internal async Task ShouldReturnNullWhenGetInexistingCode()
        {
            #region Arrange

            var mocker = new AutoMocker();

            var domain = mocker.CreateInstance<AuthCodeManager>();

            var userId = Guid.NewGuid();

            #endregion

            var result = await domain.GetCodeAsync(userId);

            #region Assert

            Assert.Null(result);
            mocker.Verify<IAuthCodeRepository>(r => r.GetCodeByUserIdAsync(userId), Times.Once);

            #endregion
        }
    }
}

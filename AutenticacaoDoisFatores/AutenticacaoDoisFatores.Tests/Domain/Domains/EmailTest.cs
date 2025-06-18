using AutenticacaoDoisFatores.Domain.Shared;
using AutenticacaoDoisFatores.Domain.Shared.Messages;
using AutenticacaoDoisFatores.Domain.Domains;
using AutenticacaoDoisFatores.Domain.Exceptions;
using AutenticacaoDoisFatores.Domain.Services;
using Bogus;
using Moq;
using Moq.AutoMock;

namespace AutenticacaoDoisFatores.Tests.Domain.Domains
{
    public class EmailTest
    {
        private readonly Faker _faker = new();
        private readonly AutoMocker _mocker = new();

        [Fact]
        internal void ShouldSendConfirmationClientEmail()
        {
            #region Arrenge

            var fakeDestinationEmail = _faker.Internet.Email();
            var fakeAccessKey = _faker.Random.AlphaNumeric(20);
            var fakeConfirmationLink = _faker.Internet.UrlWithPath();
            var token = _faker.Random.AlphaNumeric(32);

            var email = _mocker.CreateInstance<EmailSender>();

            #endregion

            email.SendClientConfirmation(fakeDestinationEmail, fakeAccessKey, fakeConfirmationLink, token);

            #region Assert

            _mocker.Verify<IEmailService>(s =>
                s.Send
                    (
                        fakeDestinationEmail,
                        EmailMessages.ClientConfirmationSubject.Description() ?? "",
                        It.IsAny<string>()
                    ), Times.Once);

            #endregion
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("@")]
        [InlineData("a@")]
        [InlineData("a@.")]
        [InlineData("a@.com")]
        [InlineData("@.")]
        [InlineData("@.com")]
        [InlineData("@dominio.com")]
        [InlineData("abcdefghijklmnopqrstuvwxyz0123456789abcdefghijklmnopqrstuvwxyz0123456789abcdefghijklmnopqrstuvwxyz0123456789abcdefghijklmnopqrstuvwxyz0123456789abcdefghijklmnopqrstuvwxyz0123456789abcdefghijklmnopqrstuvwxyz0123456789abcdefghijklmnopqrstuvwxyz0123456789abcde")]
        internal void ShouldReturnExceptionWhenSendConfirmationClientEmailWithEmptyEmail(string invalidEmail)
        {
            #region Arrenge

            var email = _mocker.CreateInstance<EmailSender>();
            var fakeAccessKey = _faker.Random.AlphaNumeric(20);
            var fakeConfirmationLink = _faker.Internet.UrlWithPath();
            var token = _faker.Random.AlphaNumeric(32);

            #endregion

            var exception = Assert.Throws<EmailException>(() => email.SendClientConfirmation(invalidEmail, fakeAccessKey, fakeConfirmationLink, token));

            #region Assert

            Assert.Equal(EmailValidationMessages.InvalidDestinationEmail.Description(), exception.Message);
            _mocker.Verify<IEmailService>(s => s.Send(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);

            #endregion
        }
    }
}

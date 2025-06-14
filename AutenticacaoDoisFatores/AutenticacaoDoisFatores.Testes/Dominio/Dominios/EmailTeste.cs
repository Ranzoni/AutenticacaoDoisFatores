using AutenticacaoDoisFatores.Dominio.Compartilhados;
using AutenticacaoDoisFatores.Dominio.Compartilhados.Mensagens;
using AutenticacaoDoisFatores.Dominio.Dominios;
using AutenticacaoDoisFatores.Dominio.Excecoes;
using AutenticacaoDoisFatores.Dominio.Servicos;
using Bogus;
using Moq;
using Moq.AutoMock;

namespace AutenticacaoDoisFatores.Testes.Dominio.Dominios
{
    public class EmailTeste
    {
        private readonly Faker _faker = new();
        private readonly AutoMocker _mocker = new();

        [Fact]
        internal void DeveEnviarEmailConfirmacaoCadastroCliente()
        {
            #region Preparação do teste

            var emailDestinoParaTeste = _faker.Internet.Email();
            var chaveAcessoParaTeste = _faker.Random.AlphaNumeric(20);
            var linkDeConfirmacaoParaTeste = _faker.Internet.UrlWithPath();
            var token = _faker.Random.AlphaNumeric(32);

            var email = _mocker.CreateInstance<EmailSender>();

            #endregion

            email.SendClientConfirmation(emailDestinoParaTeste, chaveAcessoParaTeste, linkDeConfirmacaoParaTeste, token);

            #region Verificação do teste

            _mocker.Verify<IEmailService>(s =>
                s.Send
                    (
                        emailDestinoParaTeste,
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
        internal void DeveRetornarExcecaoAoEnviarEmailConfirmacaoCadastroClienteComEmailVazio(string emailInvalido)
        {
            #region Preparação do teste

            var email = _mocker.CreateInstance<EmailSender>();
            var chaveAcessoParaTeste = _faker.Random.AlphaNumeric(20);
            var linkDeConfirmacaoParaTeste = _faker.Internet.UrlWithPath();
            var token = _faker.Random.AlphaNumeric(32);

            #endregion

            var excecao = Assert.Throws<EmailException>(() => email.SendClientConfirmation(emailInvalido, chaveAcessoParaTeste, linkDeConfirmacaoParaTeste, token));

            #region Verificação do teste

            Assert.Equal(EmailValidationMessages.InvalidDestinationEmail.Description(), excecao.Message);
            _mocker.Verify<IEmailService>(s => s.Send(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);

            #endregion
        }
    }
}

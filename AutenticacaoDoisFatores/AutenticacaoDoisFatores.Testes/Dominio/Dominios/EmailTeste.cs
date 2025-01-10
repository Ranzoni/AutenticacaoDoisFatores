﻿using AutenticacaoDoisFatores.Dominio.Compartilhados;
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

            var email = _mocker.CreateInstance<Email>();

            #endregion

            email.EnviarConfirmacaoDeCadastroDeCliente(emailDestinoParaTeste);

            #region Verificação do teste

            _mocker.Verify<IServicoDeEmail>(s =>
                s.Enviar
                    (
                        emailDestinoParaTeste,
                        MensagensEnvioEmail.TituloConfirmacaoCadastroCliente.Descricao() ?? "",
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
            var email = _mocker.CreateInstance<Email>();

            var excecao = Assert.Throws<ExcecoesEmail>(() => email.EnviarConfirmacaoDeCadastroDeCliente(emailInvalido));

            #region Verificação do teste

            Assert.Equal(MensagensValidacaoEmail.EmailDestinoNaoInvalido.Descricao(), excecao.Message);
            _mocker.Verify<IServicoDeEmail>(s => s.Enviar(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);

            #endregion
        }
    }
}

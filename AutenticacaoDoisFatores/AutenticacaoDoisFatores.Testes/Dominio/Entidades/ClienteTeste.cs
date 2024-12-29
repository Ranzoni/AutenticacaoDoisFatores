using AutenticacaoDoisFatores.Dominio.Entidades;
using AutenticacaoDoisFatores.Dominio.Validadores;
using AutenticacaoDoisFatores.Dominio.Excecoes;
using Bogus;
using AutenticacaoDoisFatores.Dominio.Compartilhados.Mensagens;
using AutenticacaoDoisFatores.Dominio.Compartilhados;

namespace AutenticacaoDoisFatores.Testes.Dominio.Entidades
{
    public class ClienteTeste
    {
        private readonly Faker _faker = new();

        [Fact]
        internal void DeveInstanciarNovoCliente()
        {
            #region Preparação do teste

            var nomeCliente = _faker.Company.CompanyName();
            var emailCliente = _faker.Internet.Email();

            #endregion Preparação do teste

            var cliente = new Cliente(nome: nomeCliente, email: emailCliente);

            #region Verificação do teste

            Assert.NotNull(cliente);

            Assert.Equal(nomeCliente, cliente.Nome);
            Assert.True(ValidadorDeCliente.NomeEhValido(cliente.Nome));

            Assert.Equal(emailCliente, cliente.Email);
            Assert.True(ValidadorDeCliente.EmailEhValido(cliente.Email));

            #endregion Verificação do teste
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("ab")]
        [InlineData("abcdefghijklmnopqrstuvwxyz0123456789abcdefghijklmno")]
        internal void NaoDeveInstanciarNovoClienteComNomeInvalido(string nomeInvalido)
        {
            var emailCliente = _faker.Internet.Email();

            var excecao = Assert.Throws<ExcecoesCliente>(() => new Cliente(nome: nomeInvalido, email: emailCliente));

            Assert.Equal(MensagensCliente.NomeNaoPreenchido.Descricao(), excecao.Message);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("abcd")]
        [InlineData("abcdefghijklmnopqrstuvwxyz0123456789abcdefghijklmnopqrstuvwxyz0123456789abcdefghijklmnopqrstuvwxyz0123456789abcdefghijklmnopqrstuvwxyz0123456789abcdefghijklmnopqrstuvwxyz0123456789abcdefghijklmnopqrstuvwxyz0123456789abcdefghijklmnopqrstuvwxyz0123456789abcde")]
        internal void NaoDeveInstanciarNovoClienteComEmailInvalido(string emailInvalido)
        {
            var nomeCliente = _faker.Company.CompanyName();

            var excecao = Assert.Throws<ExcecoesCliente>(() => new Cliente(nome: nomeCliente, email: emailInvalido));

            Assert.Equal(MensagensCliente.EmailNaoPreenchido.Descricao(), excecao.Message);
        }
    }
}

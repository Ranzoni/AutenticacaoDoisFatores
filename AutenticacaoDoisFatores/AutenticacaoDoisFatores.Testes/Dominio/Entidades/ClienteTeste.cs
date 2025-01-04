using AutenticacaoDoisFatores.Dominio.Validadores;
using AutenticacaoDoisFatores.Dominio.Excecoes;
using Bogus;
using AutenticacaoDoisFatores.Dominio.Compartilhados.Mensagens;
using AutenticacaoDoisFatores.Dominio.Compartilhados;
using AutenticacaoDoisFatores.Dominio.Construtores;

namespace AutenticacaoDoisFatores.Testes.Dominio.Entidades
{
    public class ClienteTeste
    {
        private readonly Faker _faker = new();

        [Fact]
        internal void DeveInstanciarNovoCliente()
        {
            #region Preparação do teste

            var nomeParaTeste = _faker.Company.CompanyName();
            var emailParaTeste = _faker.Internet.Email();
            var nomeDominioParaTeste = _faker.Internet.DomainWord();

            #endregion Preparação do teste

            var cliente = new ConstrutorDeCliente()
                .ComNome(nomeParaTeste)
                .ComEmail(emailParaTeste)
                .ComNomeDominio(nomeDominioParaTeste)
                .ConstruirNovoCliente();

            #region Verificação do teste

            Assert.NotNull(cliente);

            Assert.Equal(nomeParaTeste, cliente.Nome);
            Assert.True(ValidadorDeCliente.NomeEhValido(cliente.Nome));

            Assert.Equal(emailParaTeste, cliente.Email);
            Assert.True(ValidadorDeCliente.EmailEhValido(cliente.Email));

            Assert.Equal(nomeDominioParaTeste, cliente.NomeDominio);
            Assert.True(ValidadorDeCliente.NomeDominioEhValido(cliente.NomeDominio));

            #endregion Verificação do teste
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("ab")]
        [InlineData("abcdefghijklmnopqrstuvwxyz0123456789abcdefghijklmno")]
        internal void NaoDeveInstanciarNovoClienteComNomeInvalido(string nomeInvalido)
        {
            var emailParaTeste = _faker.Internet.Email();
            var nomeDominioParaTeste = _faker.Internet.DomainName();

            var excecao = Assert.Throws<ExcecoesCliente>
            (() =>
                new ConstrutorDeCliente()
                    .ComNome(nomeInvalido)
                    .ComEmail(emailParaTeste)
                    .ComNomeDominio(nomeDominioParaTeste)
                    .ConstruirNovoCliente()
            );

            Assert.Equal(MensagensCliente.NomeInvalido.Descricao(), excecao.Message);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("abcd")]
        [InlineData("abcdefghijklmnopqrstuvwxyz0123456789abcdefghijklmnopqrstuvwxyz0123456789abcdefghijklmnopqrstuvwxyz0123456789abcdefghijklmnopqrstuvwxyz0123456789abcdefghijklmnopqrstuvwxyz0123456789abcdefghijklmnopqrstuvwxyz0123456789abcdefghijklmnopqrstuvwxyz0123456789abcde")]
        internal void NaoDeveInstanciarNovoClienteComEmailInvalido(string emailInvalido)
        {
            var nomeParaTeste = _faker.Company.CompanyName();
            var nomeDominioParaTeste = _faker.Internet.DomainName();

            var excecao = Assert.Throws<ExcecoesCliente>
            (() =>
                new ConstrutorDeCliente()
                    .ComNome(nomeParaTeste)
                    .ComEmail(emailInvalido)
                    .ComNomeDominio(nomeDominioParaTeste)
                    .ConstruirNovoCliente()
            );

            Assert.Equal(MensagensCliente.EmailInvalido.Descricao(), excecao.Message);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("ab")]
        [InlineData("abcdefghijklmnop")]
        [InlineData("teste dominio")]
        [InlineData("domínio")]
        [InlineData("dominio.")]
        [InlineData("dominio@")]
        [InlineData("dominio!")]
        internal void NaoDeveInstanciarNovoClienteComNomeDominioInvalido(string nomeDominioInvalido)
        {
            var nomeParaTeste = _faker.Company.CompanyName();
            var emailParaTeste = _faker.Internet.Email();

            var excecao = Assert.Throws<ExcecoesCliente>
            (() =>
                new ConstrutorDeCliente()
                    .ComNome(nomeParaTeste)
                    .ComEmail(emailParaTeste)
                    .ComNomeDominio(nomeDominioInvalido)
                    .ConstruirNovoCliente()
            );

            Assert.Equal(MensagensCliente.NomeDominioInvalido.Descricao(), excecao.Message);
        }
    }
}

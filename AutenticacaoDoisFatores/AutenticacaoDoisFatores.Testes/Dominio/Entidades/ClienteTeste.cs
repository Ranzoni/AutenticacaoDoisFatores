using AutenticacaoDoisFatores.Dominio.Validadores;
using AutenticacaoDoisFatores.Dominio.Excecoes;
using Bogus;
using AutenticacaoDoisFatores.Dominio.Compartilhados.Mensagens;
using AutenticacaoDoisFatores.Dominio.Compartilhados;
using AutenticacaoDoisFatores.Testes.Compartilhados;

namespace AutenticacaoDoisFatores.Testes.Dominio.Entidades
{
    public class ClienteTeste
    {
        [Fact]
        internal void DeveInstanciarNovoCliente()
        {
            #region Preparação do Teste

            var faker = new Faker();
            var nomeParaTeste = faker.Company.CompanyName();
            var emailParaTeste = faker.Internet.Email();
            var nomeDominioParaTeste = faker.Internet.DomainWord();
            var chaveAcessoParaTeste = faker.Random.AlphaNumeric(20);

            var construtor = ConstrutorDeClientesTeste.RetornarConstrutorDeCliente
                (
                    nome: nomeParaTeste,
                    email: emailParaTeste,
                    nomeDominio: nomeDominioParaTeste,
                    chaveAcesso: chaveAcessoParaTeste
                );

            #endregion

            var cliente = construtor.ConstruirNovoCliente();

            #region Verificação do teste

            Assert.NotNull(cliente);

            Assert.Equal(nomeParaTeste, cliente.Nome);
            Assert.True(ValidadorDeCliente.NomeEhValido(cliente.Nome));

            Assert.Equal(emailParaTeste, cliente.Email);
            Assert.True(ValidadorDeCliente.EmailEhValido(cliente.Email));

            Assert.Equal(nomeDominioParaTeste, cliente.NomeDominio);
            Assert.True(ValidadorDeCliente.NomeDominioEhValido(cliente.NomeDominio));

            Assert.Equal(chaveAcessoParaTeste, cliente.ChaveAcesso);
            Assert.True(ValidadorDeCliente.ChaveAcessoEhValida(cliente.ChaveAcesso));

            #endregion
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("ab")]
        [InlineData("abcdefghijklmnopqrstuvwxyz0123456789abcdefghijklmno")]
        internal void NaoDeveInstanciarNovoClienteComNomeInvalido(string nomeInvalido)
        {
            var construtor = ConstrutorDeClientesTeste.RetornarConstrutorDeCliente(nome: nomeInvalido);

            var excecao = Assert.Throws<ExcecoesCliente>(construtor.ConstruirNovoCliente);

            Assert.Equal(MensagensValidacaoCliente.NomeInvalido.Descricao(), excecao.Message);
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
        internal void NaoDeveInstanciarNovoClienteComEmailInvalido(string emailInvalido)
        {
            var construtor = ConstrutorDeClientesTeste.RetornarConstrutorDeCliente(email: emailInvalido);

            var excecao = Assert.Throws<ExcecoesCliente>(construtor.ConstruirNovoCliente);

            Assert.Equal(MensagensValidacaoCliente.EmailInvalido.Descricao(), excecao.Message);
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
            var construtor = ConstrutorDeClientesTeste.RetornarConstrutorDeCliente(nomeDominio: nomeDominioInvalido);

            var excecao = Assert.Throws<ExcecoesCliente>(construtor.ConstruirNovoCliente);

            Assert.Equal(MensagensValidacaoCliente.NomeDominioInvalido.Descricao(), excecao.Message);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("ab")]
        [InlineData("abcdefghijklmnopqrstuvwxyz0123456789abcdefghijklmnopqrstuvwxyz0123456789abcdefghijklmnopqrstuvwxyz0123456789abcdefghijklmnopqrstuvwxyz0123456789abcdefghijklmnopqrstuvwxyz0123456789abcdefghijklmnopqrstuvwxyz0123456789abcdefghijklmnopqrstuvwxyz0123456789abcdefghijklmnopqrstuvwxyz0123456789abcdefghijklmnopqrstuvwxyz0123456789abcdefghijklmnopqrstuvwxyz0123456789abcdefghijklmnopqrstuvwxyz0123456789abcdefghijklmnopqrstuvwxyz0123456789abcdefghijklmnopqrstuvwxyz0123456789abcdefghijklmnopqrstuvwxyz0123456789abcdefghijklmnopqrstuvwxyz0123456789abcdefghijklmnopqrstuvwxyz0123456789abcdefghijklmnopqrstuvwxyz0123456789abcdefghijklmnopqrstuvwxyz0123456789abcdefghijklmnopqrstuvwxyz0123456789abcdefghijklmnopqrstuvwxyz0123456789abcdefghijklmnopqrstuvwxyz0123456789abcdefghijklmnopqrstuvwxyz0123456789abcdefghijklmnopqrstuvwxyz0123456789abcdefghijklmnopqrstuvwxyz0123456789abcdefghijklmnopqrstuvwxyz0123456789abcdefghijklmnopqrstuvwxyz0123456789abcdefghijklmnopqrstuvwxyz0123456789abcdefghijklmnopqrstuvwxyz0123456789abcdefghijklmnopqrstuvwxyz0123456789abcdefghijklmnopqrstuvwxyz0123456789abcdefghijklmnopqrstuvwxyz0123456789abcdefghijklmnopqrstuvwxyz0123456789abcdefghijklmnopqrstuvwxyz0123456789abcdefghijklmnopqrstuvwxyz0123456789abcdefghijklmnopqrstuvwxyz0123456789abcdefghijklmnopqrstuvwxyz0123456789abcdefghijklmnopqrstuvwxyz0123456789abcdefghijklmnopqrstuvwxyz0123456789abcdefghijklmnopqrstuvwxyz0123456789abcdefghijklmnopqrstuvwxyz0123456789abcdefghijklmnopqrstuvwxyz0123456789abcdefghijklmnopqrstuvwxyz0123456789abcdefghijklmnopqrstuvwxyz0123456789abcdefghijklmnopqrstuvwxyz0123456789abcdefghijklmnopqrstuvwxyz0123456789abcdefghijklmnopqrstuvwxyz0123456789abcdefghijklmnopqrstuvwxyz0123456789abcdefghijklmnopqrstuvwxyz0123456789abcdefghijklmnopqrstuvwxyz0123456789abcdefghijklmnopqrstuvwxyz0123456789abcdefghijklmnopqrstuvwxyz0123456789abcdefghijklmnopqrstuvwxyz0123456789abcdefghijklmnopqrstuvwxyz0123456789abcdefghijklmnopqrstuvwxyz0123456789abcdefghijklmnopqrstuvwxyz0123456789abcdefghijklmnopqrstu")]
        internal void NaoDeveInstanciarNovoClienteComChaveAcessoInvalida(string chaveAcessoInvalida)
        {
            var construtor = ConstrutorDeClientesTeste.RetornarConstrutorDeCliente(chaveAcesso: chaveAcessoInvalida);

            var excecao = Assert.Throws<ExcecoesCliente>(construtor.ConstruirNovoCliente);

            Assert.Equal(MensagensValidacaoCliente.ChaveAcessoInvalida.Descricao(), excecao.Message);
        }
    }
}

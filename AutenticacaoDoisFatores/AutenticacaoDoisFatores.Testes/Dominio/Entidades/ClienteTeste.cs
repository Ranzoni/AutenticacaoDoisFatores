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
        private readonly Faker _faker = new();

        [Fact]
        internal void DeveInstanciarNovoCliente()
        {
            #region Preparação do Teste

            var nomeParaTeste = _faker.Company.CompanyName();
            var emailParaTeste = _faker.Internet.Email();
            var nomeDominioParaTeste = _faker.Internet.DomainWord();
            var chaveAcessoParaTeste = _faker.Random.AlphaNumeric(20);

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

        [Fact]
        internal void DeveInstanciarClienteCadastrado()
        {
            #region Preparação do Teste

            var nomeParaTeste = _faker.Company.CompanyName();
            var emailParaTeste = _faker.Internet.Email();
            var nomeDominioParaTeste = _faker.Internet.DomainWord();
            var chaveAcessoParaTeste = _faker.Random.AlphaNumeric(20);
            var ativoParaTeste = _faker.Random.Bool();
            var dataCadastroParaTeste = _faker.Date.Past(1);
            var dataAlteracaoParaTeste = _faker.Date.Past(1, dataCadastroParaTeste);

            var construtor = ConstrutorDeClientesTeste.RetornarConstrutorDeCliente
                (
                    nome: nomeParaTeste,
                    email: emailParaTeste,
                    nomeDominio: nomeDominioParaTeste,
                    chaveAcesso: chaveAcessoParaTeste,
                    ativo: ativoParaTeste,
                    dataCadastro: dataCadastroParaTeste,
                    dataAlteracao: dataAlteracaoParaTeste
                );

            #endregion

            var cliente = construtor.ConstruirClienteCadastrado();

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

            Assert.Equal(ativoParaTeste, cliente.Ativo);

            Assert.Equal(dataCadastroParaTeste, cliente.DataCadastro);

            Assert.Equal(dataAlteracaoParaTeste, cliente.DataAlteracao);

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

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        internal void DeveAtivarOuDesativarNovaEntidade(bool valor)
        {
            #region Preparação do Teste

            var construtor = ConstrutorDeClientesTeste.RetornarConstrutorDeCliente();

            var cliente = construtor.ConstruirNovoCliente();

            #endregion

            cliente.Ativar(valor);

            #region Verificação do teste

            Assert.Equal(valor, cliente.Ativo);
            Assert.Null(cliente.DataAlteracao);

            #endregion
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        internal void DeveAtivarOuDesativarEntidadeCadastrada(bool valor)
        {
            #region Preparação do Teste

            var dataAlteracaoParaTeste = _faker.Date.Past();

            var construtor = ConstrutorDeClientesTeste.RetornarConstrutorDeCliente
                (
                    ativo: !valor,
                    dataAlteracao: dataAlteracaoParaTeste
                );

            var cliente = construtor.ConstruirClienteCadastrado();

            #endregion

            cliente.Ativar(valor);

            #region Verificação do teste

            Assert.Equal(valor, cliente.Ativo);
            Assert.NotEqual(dataAlteracaoParaTeste, cliente.DataAlteracao);
            Assert.True(dataAlteracaoParaTeste < cliente.DataAlteracao);

            #endregion
        }

        [Fact]
        internal void DeveAlterarChaveAcessoNovaEntidade()
        {
            #region Preparação do Teste

            var construtor = ConstrutorDeClientesTeste.RetornarConstrutorDeCliente();

            var novaChave = _faker.Random.AlphaNumeric(32);

            var cliente = construtor.ConstruirNovoCliente();

            #endregion

            cliente.AlterarChaveAcesso(novaChave);

            #region Verificação do teste

            Assert.Equal(novaChave, cliente.ChaveAcesso);
            Assert.Null(cliente.DataAlteracao);

            #endregion
        }

        [Fact]
        internal void DeveAlterarChaveAcessoEntidadeCadastrada()
        {
            #region Preparação do Teste

            var dataAlteracaoParaTeste = _faker.Date.Past();

            var construtor = ConstrutorDeClientesTeste.RetornarConstrutorDeCliente
                (
                    dataAlteracao: dataAlteracaoParaTeste
                );

            var novaChave = _faker.Random.AlphaNumeric(32);

            var cliente = construtor.ConstruirClienteCadastrado();

            #endregion

            cliente.AlterarChaveAcesso(novaChave);

            #region Verificação do teste

            Assert.Equal(novaChave, cliente.ChaveAcesso);
            Assert.NotEqual(dataAlteracaoParaTeste, cliente.DataAlteracao);
            Assert.True(dataAlteracaoParaTeste < cliente.DataAlteracao);

            #endregion
        }
    }
}

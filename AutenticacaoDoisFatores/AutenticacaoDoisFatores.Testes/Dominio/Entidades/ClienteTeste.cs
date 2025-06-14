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
            var nomeDominioParaTeste = "dominio_cliente";
            var chaveAcessoParaTeste = _faker.Random.AlphaNumeric(20);

            var construtor = ConstrutorDeClientesTeste.RetornarConstrutor
                (
                    nome: nomeParaTeste,
                    email: emailParaTeste,
                    nomeDominio: nomeDominioParaTeste,
                    chaveAcesso: chaveAcessoParaTeste
                );

            #endregion

            var cliente = construtor.BuildNew();

            #region Verificação do teste

            Assert.NotNull(cliente);

            Assert.Equal(nomeParaTeste, cliente.Nome);
            Assert.True(ClientValidator.NomeEhValido(cliente.Nome));

            Assert.Equal(emailParaTeste, cliente.Email);
            Assert.True(ClientValidator.EmailEhValido(cliente.Email));

            Assert.Equal(nomeDominioParaTeste, cliente.NomeDominio);
            Assert.True(ClientValidator.NomeDominioEhValido(cliente.NomeDominio));

            Assert.Equal(chaveAcessoParaTeste, cliente.ChaveAcesso);
            Assert.True(ClientValidator.ChaveAcessoEhValida(cliente.ChaveAcesso));

            #endregion
        }

        [Fact]
        internal void DeveInstanciarClienteCadastrado()
        {
            #region Preparação do Teste

            var nomeParaTeste = _faker.Company.CompanyName();
            var emailParaTeste = _faker.Internet.Email();
            var nomeDominioParaTeste = "dominio_cliente";
            var chaveAcessoParaTeste = _faker.Random.AlphaNumeric(20);
            var ativoParaTeste = _faker.Random.Bool();
            var dataCadastroParaTeste = _faker.Date.Past(1);
            var dataAlteracaoParaTeste = _faker.Date.Past(1, dataCadastroParaTeste);

            var construtor = ConstrutorDeClientesTeste.RetornarConstrutor
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

            var cliente = construtor.Build();

            #region Verificação do teste

            Assert.NotNull(cliente);

            Assert.Equal(nomeParaTeste, cliente.Nome);
            Assert.True(ClientValidator.NomeEhValido(cliente.Nome));

            Assert.Equal(emailParaTeste, cliente.Email);
            Assert.True(ClientValidator.EmailEhValido(cliente.Email));

            Assert.Equal(nomeDominioParaTeste, cliente.NomeDominio);
            Assert.True(ClientValidator.NomeDominioEhValido(cliente.NomeDominio));

            Assert.Equal(chaveAcessoParaTeste, cliente.ChaveAcesso);
            Assert.True(ClientValidator.ChaveAcessoEhValida(cliente.ChaveAcesso));

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
            var construtor = ConstrutorDeClientesTeste.RetornarConstrutor(nome: nomeInvalido);

            var excecao = Assert.Throws<ClientException>(construtor.BuildNew);

            Assert.Equal(ClientValidationMessages.InvalidName.Description(), excecao.Message);
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
            var construtor = ConstrutorDeClientesTeste.RetornarConstrutor(email: emailInvalido);

            var excecao = Assert.Throws<ClientException>(construtor.BuildNew);

            Assert.Equal(ClientValidationMessages.InvalidEmail.Description(), excecao.Message);
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
            var construtor = ConstrutorDeClientesTeste.RetornarConstrutor(nomeDominio: nomeDominioInvalido);

            var excecao = Assert.Throws<ClientException>(construtor.BuildNew);

            Assert.Equal(ClientValidationMessages.InvalidDomainName.Description(), excecao.Message);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("ab")]
        [InlineData("abcdefghijklmnopqrstuvwxyz0123456789abcdefghijklmnopqrstuvwxyz0123456789abcdefghijklmnopqrstuvwxyz0123456789abcdefghijklmnopqrstuvwxyz0123456789abcdefghijklmnopqrstuvwxyz0123456789abcdefghijklmnopqrstuvwxyz0123456789abcdefghijklmnopqrstuvwxyz0123456789abcdefghijklmnopqrstuvwxyz0123456789abcdefghijklmnopqrstuvwxyz0123456789abcdefghijklmnopqrstuvwxyz0123456789abcdefghijklmnopqrstuvwxyz0123456789abcdefghijklmnopqrstuvwxyz0123456789abcdefghijklmnopqrstuvwxyz0123456789abcdefghijklmnopqrstuvwxyz0123456789abcdefghijklmnopqrstuvwxyz0123456789abcdefghijklmnopqrstuvwxyz0123456789abcdefghijklmnopqrstuvwxyz0123456789abcdefghijklmnopqrstuvwxyz0123456789abcdefghijklmnopqrstuvwxyz0123456789abcdefghijklmnopqrstuvwxyz0123456789abcdefghijklmnopqrstuvwxyz0123456789abcdefghijklmnopqrstuvwxyz0123456789abcdefghijklmnopqrstuvwxyz0123456789abcdefghijklmnopqrstuvwxyz0123456789abcdefghijklmnopqrstuvwxyz0123456789abcdefghijklmnopqrstuvwxyz0123456789abcdefghijklmnopqrstuvwxyz0123456789abcdefghijklmnopqrstuvwxyz0123456789abcdefghijklmnopqrstuvwxyz0123456789abcdefghijklmnopqrstuvwxyz0123456789abcdefghijklmnopqrstuvwxyz0123456789abcdefghijklmnopqrstuvwxyz0123456789abcdefghijklmnopqrstuvwxyz0123456789abcdefghijklmnopqrstuvwxyz0123456789abcdefghijklmnopqrstuvwxyz0123456789abcdefghijklmnopqrstuvwxyz0123456789abcdefghijklmnopqrstuvwxyz0123456789abcdefghijklmnopqrstuvwxyz0123456789abcdefghijklmnopqrstuvwxyz0123456789abcdefghijklmnopqrstuvwxyz0123456789abcdefghijklmnopqrstuvwxyz0123456789abcdefghijklmnopqrstuvwxyz0123456789abcdefghijklmnopqrstuvwxyz0123456789abcdefghijklmnopqrstuvwxyz0123456789abcdefghijklmnopqrstuvwxyz0123456789abcdefghijklmnopqrstuvwxyz0123456789abcdefghijklmnopqrstuvwxyz0123456789abcdefghijklmnopqrstuvwxyz0123456789abcdefghijklmnopqrstuvwxyz0123456789abcdefghijklmnopqrstuvwxyz0123456789abcdefghijklmnopqrstuvwxyz0123456789abcdefghijklmnopqrstuvwxyz0123456789abcdefghijklmnopqrstuvwxyz0123456789abcdefghijklmnopqrstuvwxyz0123456789abcdefghijklmnopqrstuvwxyz0123456789abcdefghijklmnopqrstu")]
        internal void NaoDeveInstanciarNovoClienteComChaveAcessoInvalida(string chaveAcessoInvalida)
        {
            var construtor = ConstrutorDeClientesTeste.RetornarConstrutor(chaveAcesso: chaveAcessoInvalida);

            var excecao = Assert.Throws<ClientException>(construtor.BuildNew);

            Assert.Equal(ClientValidationMessages.InvalidAccessKey.Description(), excecao.Message);
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        internal void DeveAtivarOuDesativarNovaEntidade(bool valor)
        {
            #region Preparação do Teste

            var construtor = ConstrutorDeClientesTeste.RetornarConstrutor();

            var cliente = construtor.BuildNew();

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

            var construtor = ConstrutorDeClientesTeste.RetornarConstrutor
                (
                    ativo: !valor,
                    dataAlteracao: dataAlteracaoParaTeste
                );

            var cliente = construtor.Build();

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

            var construtor = ConstrutorDeClientesTeste.RetornarConstrutor();

            var novaChave = _faker.Random.AlphaNumeric(32);

            var cliente = construtor.BuildNew();

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

            var construtor = ConstrutorDeClientesTeste.RetornarConstrutor
                (
                    dataAlteracao: dataAlteracaoParaTeste
                );

            var novaChave = _faker.Random.AlphaNumeric(32);

            var cliente = construtor.Build();

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

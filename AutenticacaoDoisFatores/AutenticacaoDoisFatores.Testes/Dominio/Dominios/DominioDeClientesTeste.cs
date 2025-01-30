using AutenticacaoDoisFatores.Dominio.Compartilhados;
using AutenticacaoDoisFatores.Dominio.Compartilhados.Mensagens;
using AutenticacaoDoisFatores.Dominio.Dominios;
using AutenticacaoDoisFatores.Dominio.Entidades;
using AutenticacaoDoisFatores.Dominio.Excecoes;
using AutenticacaoDoisFatores.Dominio.Repositorios;
using AutenticacaoDoisFatores.Testes.Compartilhados;
using Bogus;
using Moq;
using Moq.AutoMock;

namespace AutenticacaoDoisFatores.Testes.Dominio.Dominios
{
    public class DominioDeClientesTeste
    {
        private readonly Faker _faker = new();
        private readonly AutoMocker _mocker = new();

        #region Teste de cadastro

        [Fact]
        internal async Task DeveCriarCliente()
        {
            #region Preparação do teste

            var construtor = ConstrutorDeClientesTeste.RetornarConstrutor();
            var cliente = construtor.ConstruirNovo();

            var dominio = _mocker.CreateInstance<DominioDeClientes>();

            #endregion Preparação do teste

            var retorno = await dominio.CriarClienteAsync(cliente);

            #region Verificação do teste

            Assert.NotNull(retorno);
            Assert.Equal(cliente, retorno);
            _mocker.Verify<IRepositorioDeClientes>(r => r.Adicionar(cliente), Times.Once);
            _mocker.Verify<IRepositorioDeClientes>(r => r.SalvarAlteracoesAsync(), Times.Once);

            #endregion Verificação do teste
        }

        [Fact]
        internal async Task DeveCriarDominioCliente()
        {
            #region Preparação do teste

            var construtor = ConstrutorDeClientesTeste.RetornarConstrutor();
            var cliente = construtor.ConstruirNovo();

            var dominio = _mocker.CreateInstance<DominioDeClientes>();

            _mocker.GetMock<IRepositorioDeClientes>().Setup(r => r.BuscarUnicoAsync(cliente.Id)).ReturnsAsync(cliente);

            #endregion Preparação do teste

            await dominio.CriarDominioDoClienteAsync(cliente.Id);

            #region Verificação do teste

            _mocker.Verify<IRepositorioDeClientes>(r => r.BuscarUnicoAsync(cliente.Id), Times.Once);
            _mocker.Verify<IRepositorioDeClientes>(r => r.CriarDominio(cliente.NomeDominio), Times.Once);

            #endregion Verificação do teste
        }

        [Fact]
        internal async Task DeveRetornarExcecaoAoCriarDominioDeClienteInexistente()
        {
            #region Preparação do teste

            var idParaTeste = Guid.NewGuid();

            var dominio = _mocker.CreateInstance<DominioDeClientes>();

            #endregion Preparação do teste

            var excecao = await Assert.ThrowsAsync<ExcecoesCliente>(() => dominio.CriarDominioDoClienteAsync(idParaTeste));

            #region Verificação do teste

            Assert.Equal(MensagensValidacaoCliente.ClienteNaoEncontrado.Descricao(), excecao.Message);
            _mocker.Verify<IRepositorioDeClientes>(r => r.BuscarUnicoAsync(idParaTeste), Times.Once);
            _mocker.Verify<IRepositorioDeClientes>(r => r.CriarDominio(It.IsAny<string>()), Times.Never);

            #endregion Verificação do teste
        }

        [Fact]
        internal async Task DeveRetornarExcecaoAoCriarClienteComEmailJaCadastrado()
        {
            #region Preparação do teste

            var emailJaCadastrado = _faker.Internet.Email();

            var construtor = ConstrutorDeClientesTeste.RetornarConstrutor(email: emailJaCadastrado);
            var cliente = construtor.ConstruirNovo();

            var dominio = _mocker.CreateInstance<DominioDeClientes>();
            _mocker.GetMock<IRepositorioDeClientes>().Setup(r => r.ExisteEmail(emailJaCadastrado)).ReturnsAsync(true);

            #endregion Preparação do teste

            var excecao = await Assert.ThrowsAsync<ExcecoesCliente>(() => dominio.CriarClienteAsync(cliente));

            #region Verificação do teste

            Assert.Equal(MensagensValidacaoCliente.EmailJaCadastrado.Descricao(), excecao.Message);
            _mocker.Verify<IRepositorioDeClientes>(r => r.Adicionar(It.IsAny<Cliente>()), Times.Never);
            _mocker.Verify<IRepositorioDeClientes>(r => r.SalvarAlteracoesAsync(), Times.Never);

            #endregion Verificação do teste
        }

        [Fact]
        internal async Task DeveRetornarExcecaoAoCriarClienteComNomeDominioJaCadastrado()
        {
            #region Preparação do teste

            var dominioJaCadastrado = _faker.Internet.DomainWord();

            var construtor = ConstrutorDeClientesTeste.RetornarConstrutor(nomeDominio: dominioJaCadastrado);
            var cliente = construtor.ConstruirNovo();

            var dominio = _mocker.CreateInstance<DominioDeClientes>();
            _mocker.GetMock<IRepositorioDeClientes>().Setup(r => r.ExisteDominio(dominioJaCadastrado)).ReturnsAsync(true);

            #endregion Preparação do teste

            var excecao = await Assert.ThrowsAsync<ExcecoesCliente>(() => dominio.CriarClienteAsync(cliente));

            #region Verificação do teste

            Assert.Equal(MensagensValidacaoCliente.NomeDominioJaCadastrado.Descricao(), excecao.Message);
            _mocker.Verify<IRepositorioDeClientes>(r => r.Adicionar(It.IsAny<Cliente>()), Times.Never);
            _mocker.Verify<IRepositorioDeClientes>(r => r.SalvarAlteracoesAsync(), Times.Never);

            #endregion Verificação do teste
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        internal async Task DeveRetornarVerdadeiroOuFalsoEmailCadastrado(bool emailCadastrado)
        {
            #region Preparação do teste

            var emailParaTeste = _faker.Internet.Email();

            var dominio = _mocker.CreateInstance<DominioDeClientes>();
            _mocker.GetMock<IRepositorioDeClientes>().Setup(r => r.ExisteEmail(emailParaTeste)).ReturnsAsync(emailCadastrado);

            #endregion Preparação do teste

            var retorno = await dominio.EmailEstaCadastradoAsync(emailParaTeste);

            #region Verificação do teste

            Assert.Equal(retorno, emailCadastrado);
            _mocker.Verify<IRepositorioDeClientes>(r => r.ExisteEmail(emailParaTeste), Times.Once);

            #endregion Verificação do teste
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        internal async Task DeveRetornarVerdadeiroOuFalsoDominioCadastrado(bool dominioJaCadastrado)
        {
            #region Preparação do teste

            var nomeDominioParaTeste = _faker.Internet.DomainName();

            var dominio = _mocker.CreateInstance<DominioDeClientes>();
            _mocker.GetMock<IRepositorioDeClientes>().Setup(r => r.ExisteDominio(nomeDominioParaTeste)).ReturnsAsync(dominioJaCadastrado);

            #endregion Preparação do teste

            var retorno = await dominio.NomeDominioEstaCadastradoAsync(nomeDominioParaTeste);

            #region Verificação do teste

            Assert.Equal(retorno, dominioJaCadastrado);
            _mocker.Verify<IRepositorioDeClientes>(r => r.ExisteDominio(nomeDominioParaTeste), Times.Once);

            #endregion Verificação do teste
        }

        #endregion

        #region Teste de alteração

        [Fact]
        internal async Task DeveAlterarCliente()
        {
            #region Preparação do teste

            var construtor = ConstrutorDeClientesTeste.RetornarConstrutor();
            var cliente = construtor.ConstruirCadastrado();

            var dominio = _mocker.CreateInstance<DominioDeClientes>();

            #endregion Preparação do teste

            var retorno = await dominio.AlterarClienteAsync(cliente);

            #region Verificação do teste

            Assert.NotNull(retorno);
            Assert.Equal(cliente, retorno);
            _mocker.Verify<IRepositorioDeClientes>(r => r.Editar(cliente), Times.Once);
            _mocker.Verify<IRepositorioDeClientes>(r => r.SalvarAlteracoesAsync(), Times.Once);

            #endregion Verificação do teste
        }

        #endregion
    }
}

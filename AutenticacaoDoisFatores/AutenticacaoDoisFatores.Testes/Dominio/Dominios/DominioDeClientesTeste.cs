using AutenticacaoDoisFatores.Dominio.Compartilhados;
using AutenticacaoDoisFatores.Dominio.Compartilhados.Mensagens;
using AutenticacaoDoisFatores.Dominio.Dominios;
using AutenticacaoDoisFatores.Dominio.Entidades;
using AutenticacaoDoisFatores.Dominio.Excecoes;
using AutenticacaoDoisFatores.Dominio.Filtros;
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
            _mocker.GetMock<IRepositorioDeClientes>().Setup(r => r.ExisteEmailAsync(emailJaCadastrado)).ReturnsAsync(true);

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
            _mocker.GetMock<IRepositorioDeClientes>().Setup(r => r.ExisteDominioAsync(dominioJaCadastrado)).ReturnsAsync(true);

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
            _mocker.GetMock<IRepositorioDeClientes>().Setup(r => r.ExisteEmailAsync(emailParaTeste)).ReturnsAsync(emailCadastrado);

            #endregion Preparação do teste

            var retorno = await dominio.EmailEstaCadastradoAsync(emailParaTeste);

            #region Verificação do teste

            Assert.Equal(retorno, emailCadastrado);
            _mocker.Verify<IRepositorioDeClientes>(r => r.ExisteEmailAsync(emailParaTeste), Times.Once);

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
            _mocker.GetMock<IRepositorioDeClientes>().Setup(r => r.ExisteDominioAsync(nomeDominioParaTeste)).ReturnsAsync(dominioJaCadastrado);

            #endregion Preparação do teste

            var retorno = await dominio.NomeDominioEstaCadastradoAsync(nomeDominioParaTeste);

            #region Verificação do teste

            Assert.Equal(retorno, dominioJaCadastrado);
            _mocker.Verify<IRepositorioDeClientes>(r => r.ExisteDominioAsync(nomeDominioParaTeste), Times.Once);

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

        #region Teste de busca

        [Fact]
        internal async Task DeveBuscarUnico()
        {
            #region Preparação do teste

            var mocker = new AutoMocker();

            var idCliente = Guid.NewGuid();
            var cliente = ConstrutorDeClientesTeste
                .RetornarConstrutor(id: idCliente)
                .ConstruirCadastrado();

            var dominio = mocker.CreateInstance<DominioDeClientes>();
            mocker.GetMock<IRepositorioDeClientes>().Setup(r => r.BuscarUnicoAsync(idCliente)).ReturnsAsync(cliente);

            #endregion

            var resposta = await dominio.BuscarClienteAsync(idCliente);

            #region Verificação do teste

            Assert.NotNull(resposta);
            Assert.Equal(cliente, resposta);
            mocker.Verify<IRepositorioDeClientes>(r => r.BuscarUnicoAsync(idCliente), Times.Once);

            #endregion
        }

        [Fact]
        internal async Task DeveRetornarNuloAoBuscarUnicoQueNaoExiste()
        {
            #region Preparação do teste

            var mocker = new AutoMocker();

            var idCliente = Guid.NewGuid();

            var dominio = mocker.CreateInstance<DominioDeClientes>();

            #endregion

            var resposta = await dominio.BuscarClienteAsync(idCliente);

            #region Verificação do teste

            Assert.Null(resposta);
            mocker.Verify<IRepositorioDeClientes>(r => r.BuscarUnicoAsync(idCliente), Times.Once);

            #endregion
        }

        [Fact]
        internal async Task DeveBuscarVariosSemFiltro()
        {
            #region Preparação do teste

            var mocker = new AutoMocker();

            var idCliente = Guid.NewGuid();
            var filtros = new FiltroDeClientes();

            var maximoRegistros = _faker.Random.Int(2, filtros.QtdPorPagina);
            var listaDeClientes = GerarVarios(maximoRegistros);

            var dominio = mocker.CreateInstance<DominioDeClientes>();
            mocker.GetMock<IRepositorioDeClientes>().Setup(r => r.BuscarVariosAsync(filtros)).ReturnsAsync(listaDeClientes);

            #endregion

            var resposta = await dominio.BuscarVariosAsync(filtros);

            #region Verificação do teste

            Assert.NotNull(resposta);
            Assert.Equal(listaDeClientes, resposta);
            mocker.Verify<IRepositorioDeClientes>(r => r.BuscarVariosAsync(filtros), Times.Once);

            #endregion
        }

        [Fact]
        internal async Task DeveBuscarVariosComFiltro()
        {
            #region Preparação do teste

            var mocker = new AutoMocker();

            var filtros = new FiltroDeClientes();
            var maximoRegistros = _faker.Random.Int(2, filtros.QtdPorPagina);

            var idCliente = Guid.NewGuid();
            var listaDeClientes = GerarVarios(maximoRegistros, ativo: true);

            var maximoPaginacao = _faker.Random.Int(1, maximoRegistros);

            var dominio = mocker.CreateInstance<DominioDeClientes>();
            mocker.GetMock<IRepositorioDeClientes>().Setup(r => r.BuscarVariosAsync(filtros)).ReturnsAsync(listaDeClientes);

            #endregion

            var resposta = await dominio.BuscarVariosAsync(filtros);

            #region Verificação do teste

            Assert.NotNull(resposta);
            Assert.Equal(listaDeClientes, resposta);
            mocker.Verify<IRepositorioDeClientes>(r => r.BuscarVariosAsync(filtros), Times.Once);

            #endregion
        }

        [Fact]
        internal async Task DeveRetornarListaVaziaAoBuscarVariosQueNaoEncontrou()
        {
            #region Preparação do teste

            var mocker = new AutoMocker();

            var idCliente = Guid.NewGuid();
            var filtros = new FiltroDeClientes();

            var dominio = mocker.CreateInstance<DominioDeClientes>();

            #endregion

            var resposta = await dominio.BuscarVariosAsync(filtros);

            #region Verificação do teste

            Assert.Empty(resposta);
            mocker.Verify<IRepositorioDeClientes>(r => r.BuscarVariosAsync(filtros), Times.Once);

            #endregion
        }

        [Fact]
        internal async Task DeveBuscarPorEmail()
        {
            #region Preparação do teste

            var mocker = new AutoMocker();

            var email = _faker.Person.Email;
            var cliente = ConstrutorDeClientesTeste
                .RetornarConstrutor(email: email)
                .ConstruirCadastrado();

            var dominio = mocker.CreateInstance<DominioDeClientes>();
            mocker.GetMock<IRepositorioDeClientes>().Setup(r => r.BuscarPorEmailAsync(email)).ReturnsAsync(cliente);

            #endregion

            var resposta = await dominio.BuscarPorEmailAsync(email);

            #region Verificação do teste

            Assert.NotNull(resposta);
            Assert.Equal(cliente, resposta);
            mocker.Verify<IRepositorioDeClientes>(r => r.BuscarPorEmailAsync(email), Times.Once);

            #endregion
        }

        [Fact]
        internal async Task DeveRetornarNuloBuscarPorEmailClienteInexistente()
        {
            #region Preparação do teste

            var mocker = new AutoMocker();

            var email = _faker.Person.Email;

            var dominio = mocker.CreateInstance<DominioDeClientes>();

            #endregion

            var resposta = await dominio.BuscarPorEmailAsync(email);

            #region Verificação do teste

            Assert.Null(resposta);
            mocker.Verify<IRepositorioDeClientes>(r => r.BuscarPorEmailAsync(email), Times.Once);

            #endregion
        }

        #endregion

        private static List<Cliente> GerarVarios(int qtd, bool? ativo = null)
        {
            var listaDeClientes = new List<Cliente>();

            for (var i = 1; i <= qtd; i++)
            {
                var cliente = ConstrutorDeClientesTeste
                    .RetornarConstrutor(ativo: ativo)
                    .ConstruirCadastrado();

                listaDeClientes.Add(cliente);
            }

            return listaDeClientes;
        }
    }
}

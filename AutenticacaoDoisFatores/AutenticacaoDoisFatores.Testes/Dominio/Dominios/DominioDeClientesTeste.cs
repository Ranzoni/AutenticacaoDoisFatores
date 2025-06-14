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
            var cliente = construtor.BuildNew();

            var dominio = _mocker.CreateInstance<ClientDomain>();

            #endregion Preparação do teste

            var retorno = await dominio.CriarClienteAsync(cliente);

            #region Verificação do teste

            Assert.NotNull(retorno);
            Assert.Equal(cliente, retorno);
            _mocker.Verify<IClientRepository>(r => r.Adicionar(cliente), Times.Once);
            _mocker.Verify<IClientRepository>(r => r.SaveChangesAsync(), Times.Once);

            #endregion Verificação do teste
        }

        [Fact]
        internal async Task DeveCriarDominioCliente()
        {
            #region Preparação do teste

            var construtor = ConstrutorDeClientesTeste.RetornarConstrutor();
            var cliente = construtor.BuildNew();

            var dominio = _mocker.CreateInstance<ClientDomain>();

            _mocker.GetMock<IClientRepository>().Setup(r => r.BuscarUnicoAsync(cliente.Id)).ReturnsAsync(cliente);

            #endregion Preparação do teste

            await dominio.CriarDominioDoClienteAsync(cliente.Id);

            #region Verificação do teste

            _mocker.Verify<IClientRepository>(r => r.BuscarUnicoAsync(cliente.Id), Times.Once);
            _mocker.Verify<IClientRepository>(r => r.CriarDominio(cliente.NomeDominio), Times.Once);

            #endregion Verificação do teste
        }

        [Fact]
        internal async Task DeveRetornarExcecaoAoCriarDominioDeClienteInexistente()
        {
            #region Preparação do teste

            var idParaTeste = Guid.NewGuid();

            var dominio = _mocker.CreateInstance<ClientDomain>();

            #endregion Preparação do teste

            var excecao = await Assert.ThrowsAsync<ClientException>(() => dominio.CreateDomainAsync(idParaTeste));

            #region Verificação do teste

            Assert.Equal(ClientValidationMessages.ClientNotFound.Description(), excecao.Message);
            _mocker.Verify<IClientRepository>(r => r.GetByIdAsync(idParaTeste), Times.Once);
            _mocker.Verify<IClientRepository>(r => r.NewDomain(It.IsAny<string>()), Times.Never);

            #endregion Verificação do teste
        }

        [Fact]
        internal async Task DeveRetornarExcecaoAoCriarClienteComEmailJaCadastrado()
        {
            #region Preparação do teste

            var emailJaCadastrado = _faker.Internet.Email();

            var construtor = ConstrutorDeClientesTeste.RetornarConstrutor(email: emailJaCadastrado);
            var cliente = construtor.BuildNew();

            var dominio = _mocker.CreateInstance<ClientDomain>();
            _mocker.GetMock<IClientRepository>().Setup(r => r.EmailExistsAsync(emailJaCadastrado)).ReturnsAsync(true);

            #endregion Preparação do teste

            var excecao = await Assert.ThrowsAsync<ClientException>(() => dominio.CriarClienteAsync(cliente));

            #region Verificação do teste

            Assert.Equal(ClientValidationMessages.EmailAlreadyRegistered.Description(), excecao.Message);
            _mocker.Verify<IClientRepository>(r => r.Add(It.IsAny<Client>()), Times.Never);
            _mocker.Verify<IClientRepository>(r => r.SaveChangesAsync(), Times.Never);

            #endregion Verificação do teste
        }

        [Fact]
        internal async Task DeveRetornarExcecaoAoCriarClienteComNomeDominioJaCadastrado()
        {
            #region Preparação do teste

            var dominioJaCadastrado = _faker.Internet.DomainWord();

            var construtor = ConstrutorDeClientesTeste.RetornarConstrutor(nomeDominio: dominioJaCadastrado);
            var cliente = construtor.BuildNew();

            var dominio = _mocker.CreateInstance<ClientDomain>();
            _mocker.GetMock<IClientRepository>().Setup(r => r.DomainExistsAsync(dominioJaCadastrado)).ReturnsAsync(true);

            #endregion Preparação do teste

            var excecao = await Assert.ThrowsAsync<ClientException>(() => dominio.CriarClienteAsync(cliente));

            #region Verificação do teste

            Assert.Equal(ClientValidationMessages.DomainNameAlreadyRegistered.Description(), excecao.Message);
            _mocker.Verify<IClientRepository>(r => r.Add(It.IsAny<Client>()), Times.Never);
            _mocker.Verify<IClientRepository>(r => r.SaveChangesAsync(), Times.Never);

            #endregion Verificação do teste
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        internal async Task DeveRetornarVerdadeiroOuFalsoEmailCadastrado(bool emailCadastrado)
        {
            #region Preparação do teste

            var emailParaTeste = _faker.Internet.Email();

            var dominio = _mocker.CreateInstance<ClientDomain>();
            _mocker.GetMock<IClientRepository>().Setup(r => r.EmailExistsAsync(emailParaTeste)).ReturnsAsync(emailCadastrado);

            #endregion Preparação do teste

            var retorno = await dominio.EmailExistsAsync(emailParaTeste);

            #region Verificação do teste

            Assert.Equal(retorno, emailCadastrado);
            _mocker.Verify<IClientRepository>(r => r.EmailExistsAsync(emailParaTeste), Times.Once);

            #endregion Verificação do teste
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        internal async Task DeveRetornarVerdadeiroOuFalsoDominioCadastrado(bool dominioJaCadastrado)
        {
            #region Preparação do teste

            var nomeDominioParaTeste = _faker.Internet.DomainName();

            var dominio = _mocker.CreateInstance<ClientDomain>();
            _mocker.GetMock<IClientRepository>().Setup(r => r.DomainExistsAsync(nomeDominioParaTeste)).ReturnsAsync(dominioJaCadastrado);

            #endregion Preparação do teste

            var retorno = await dominio.DomainExistsAsync(nomeDominioParaTeste);

            #region Verificação do teste

            Assert.Equal(retorno, dominioJaCadastrado);
            _mocker.Verify<IClientRepository>(r => r.DomainExistsAsync(nomeDominioParaTeste), Times.Once);

            #endregion Verificação do teste
        }

        #endregion

        #region Teste de alteração

        [Fact]
        internal async Task DeveAlterarCliente()
        {
            #region Preparação do teste

            var construtor = ConstrutorDeClientesTeste.RetornarConstrutor();
            var cliente = construtor.Build();

            var dominio = _mocker.CreateInstance<ClientDomain>();

            #endregion Preparação do teste

            var retorno = await dominio.AlterarClienteAsync(cliente);

            #region Verificação do teste

            Assert.NotNull(retorno);
            Assert.Equal(cliente, retorno);
            _mocker.Verify<IClientRepository>(r => r.Editar(cliente), Times.Once);
            _mocker.Verify<IClientRepository>(r => r.SaveChangesAsync(), Times.Once);

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
                .Build();

            var dominio = mocker.CreateInstance<ClientDomain>();
            mocker.GetMock<IClientRepository>().Setup(r => r.GetByIdAsync(idCliente)).ReturnsAsync(cliente);

            #endregion

            var resposta = await dominio.GetByIdAsync(idCliente);

            #region Verificação do teste

            Assert.NotNull(resposta);
            Assert.Equal(cliente, resposta);
            mocker.Verify<IClientRepository>(r => r.GetByIdAsync(idCliente), Times.Once);

            #endregion
        }

        [Fact]
        internal async Task DeveRetornarNuloAoBuscarUnicoQueNaoExiste()
        {
            #region Preparação do teste

            var mocker = new AutoMocker();

            var idCliente = Guid.NewGuid();

            var dominio = mocker.CreateInstance<ClientDomain>();

            #endregion

            var resposta = await dominio.GetByIdAsync(idCliente);

            #region Verificação do teste

            Assert.Null(resposta);
            mocker.Verify<IClientRepository>(r => r.GetByIdAsync(idCliente), Times.Once);

            #endregion
        }

        [Fact]
        internal async Task DeveBuscarVarios()
        {
            #region Preparação do teste

            var mocker = new AutoMocker();

            var filtros = new ClientFilter();
            var maximoRegistros = _faker.Random.Int(2, filtros.Quantity);

            var listaDeClientes = GerarVarios(maximoRegistros, ativo: true);

            var maximoPaginacao = _faker.Random.Int(1, maximoRegistros);

            var dominio = mocker.CreateInstance<ClientDomain>();
            mocker.GetMock<IClientRepository>().Setup(r => r.GetAllAsync(filtros)).ReturnsAsync(listaDeClientes);

            #endregion

            var resposta = await dominio.BuscarVariosAsync(filtros);

            #region Verificação do teste

            Assert.NotNull(resposta);
            Assert.Equal(listaDeClientes, resposta);
            mocker.Verify<IClientRepository>(r => r.GetAllAsync(filtros), Times.Once);

            #endregion
        }

        [Fact]
        internal async Task DeveRetornarListaVaziaAoBuscarVariosQueNaoEncontrou()
        {
            #region Preparação do teste

            var mocker = new AutoMocker();

            var filtros = new ClientFilter();

            var dominio = mocker.CreateInstance<ClientDomain>();

            #endregion

            var resposta = await dominio.BuscarVariosAsync(filtros);

            #region Verificação do teste

            Assert.Empty(resposta);
            mocker.Verify<IClientRepository>(r => r.GetAllAsync(filtros), Times.Once);

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
                .Build();

            var dominio = mocker.CreateInstance<ClientDomain>();
            mocker.GetMock<IClientRepository>().Setup(r => r.GetByEmailAsync(email)).ReturnsAsync(cliente);

            #endregion

            var resposta = await dominio.GetByEmailAsync(email);

            #region Verificação do teste

            Assert.NotNull(resposta);
            Assert.Equal(cliente, resposta);
            mocker.Verify<IClientRepository>(r => r.GetByEmailAsync(email), Times.Once);

            #endregion
        }

        [Fact]
        internal async Task DeveRetornarNuloBuscarPorEmailClienteInexistente()
        {
            #region Preparação do teste

            var mocker = new AutoMocker();

            var email = _faker.Person.Email;

            var dominio = mocker.CreateInstance<ClientDomain>();

            #endregion

            var resposta = await dominio.GetByEmailAsync(email);

            #region Verificação do teste

            Assert.Null(resposta);
            mocker.Verify<IClientRepository>(r => r.GetByEmailAsync(email), Times.Once);

            #endregion
        }

        #endregion

        private static List<Client> GerarVarios(int qtd, bool? ativo = null)
        {
            var listaDeClientes = new List<Client>();

            for (var i = 1; i <= qtd; i++)
            {
                var cliente = ConstrutorDeClientesTeste
                    .RetornarConstrutor(ativo: ativo)
                    .Build();

                listaDeClientes.Add(cliente);
            }

            return listaDeClientes;
        }
    }
}

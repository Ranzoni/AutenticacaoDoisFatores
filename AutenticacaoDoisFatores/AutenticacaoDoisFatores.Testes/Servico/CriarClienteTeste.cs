using AutenticacaoDoisFatores.Dominio.Compartilhados.Mensagens;
using AutenticacaoDoisFatores.Dominio.Dominios;
using AutenticacaoDoisFatores.Dominio.Entidades;
using AutenticacaoDoisFatores.Dominio.Repositorios;
using AutenticacaoDoisFatores.Servico.CasosDeUso;
using AutenticacaoDoisFatores.Servico.Mapeadores;
using AutenticacaoDoisFatores.Testes.Compartilhados;
using AutoMapper;
using Bogus;
using Mensageiro;
using Moq;
using Moq.AutoMock;

namespace AutenticacaoDoisFatores.Testes.Servico
{
    public class CriarClienteTeste
    {
        private readonly Faker _faker = new();
        private readonly AutoMocker _mocker = new();
        private readonly IMapper _mapeador;

        public CriarClienteTeste()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<MapeadorDeCliente>();
            });
            _mapeador = config.CreateMapper();
        }

        [Fact]
        internal async Task DeveExecutar()
        {
            #region Preparação do teste

            var construtorDeDto = ConstrutorDeClientesTeste.RetornarConstrutorDeNovoCliente();
            var novoCliente = construtorDeDto.Construir();
            var construtorDeCliente = ConstrutorDeClientesTeste
                .RetornarConstrutorDeCliente
                (
                    nome: novoCliente.Nome,
                    email: novoCliente.Email,
                    nomeDominio: novoCliente.NomeDominio,
                    chaveAcesso: novoCliente.ChaveDescriptografada()
                );
            var cliente = construtorDeCliente.ConstruirNovoCliente();

            _mocker.CreateInstance<DominioDeClientes>();
            _mocker.Use(_mapeador);
            _mocker.GetMock<IRepositorioDeClientes>().Setup(r => r.BuscarUnicoAsync(It.IsAny<Guid>())).ReturnsAsync(cliente);

            var servico = _mocker.CreateInstance<CriarCliente>();

            #endregion Preparação do teste

            var clienteCadastrado = await servico.ExecutarAsync(novoCliente);

            #region Verificação do teste

            Assert.NotNull(clienteCadastrado);
            Assert.Equal(novoCliente.Nome, clienteCadastrado.Nome);
            Assert.Equal(novoCliente.Email, clienteCadastrado.Email);
            Assert.Equal(novoCliente.NomeDominio, clienteCadastrado.NomeDominio);
            Assert.NotEqual(novoCliente.ChaveAcesso, novoCliente.ChaveDescriptografada());
            _mocker.Verify<IRepositorioDeClientes>(r => r.CriarDominio(cliente.NomeDominio), Times.Once);
            _mocker.Verify<INotificador>(n => n.AddMensagem(It.IsAny<MensagensValidacaoCliente>()), Times.Never);

            #endregion Preparação do teste
        }

        [Fact]
        internal void DeveMapearParaClienteAChaveAcessoCriptografada()
        {
            #region Preparação do teste

            var construtorDeDto = ConstrutorDeClientesTeste.RetornarConstrutorDeNovoCliente();
            var novoCliente = construtorDeDto.Construir();

            #endregion Preparação do teste

            var cliente = _mapeador.Map<Cliente>(novoCliente);

            #region Verificação do teste

            Assert.NotNull(cliente);
            Assert.NotEqual(novoCliente.ChaveDescriptografada(), cliente.ChaveAcesso);
            Assert.Equal(novoCliente.ChaveAcesso, cliente.ChaveAcesso);

            #endregion Preparação do teste
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("ab")]
        [InlineData("abcdefghijklmnopqrstuvwxyz0123456789abcdefghijklmno")]
        internal async Task DeveRetornarNuloEMensagemQuandoNomeForInvalido(string nomeInvalido)
        {
            #region Preparação do teste

            var construtor = ConstrutorDeClientesTeste.RetornarConstrutorDeNovoCliente(nome: nomeInvalido);
            var novoCliente = construtor.Construir();

            _mocker.CreateInstance<DominioDeClientes>();
            _mocker.Use(_mapeador);
            _mocker.GetMock<INotificador>().Setup(n => n.ExisteMensagem()).Returns(true);
            var servico = _mocker.CreateInstance<CriarCliente>();

            #endregion Preparação do teste

            var clienteCadastrado = await servico.ExecutarAsync(novoCliente);

            #region Verificação do teste

            Assert.Null(clienteCadastrado);
            _mocker.Verify<INotificador>(n => n.AddMensagem(MensagensValidacaoCliente.NomeInvalido), Times.Once);

            #endregion Preparação do teste
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("ab")]
        [InlineData("abcdefghijklmnopqrstuvwxyz0123456789abcdefghijklmnopqrstuvwxyz0123456789abcdefghijklmnopqrstuvwxyz0123456789abcdefghijklmnopqrstuvwxyz0123456789abcdefghijklmnopqrstuvwxyz0123456789abcdefghijklmnopqrstuvwxyz0123456789abcdefghijklmnopqrstuvwxyz0123456789abcde")]
        internal async Task DeveRetornarNuloEMensagemQuandoEmailForInvalido(string emailInvalido)
        {
            #region Preparação do teste

            var construtor = ConstrutorDeClientesTeste.RetornarConstrutorDeNovoCliente(email: emailInvalido);
            var novoCliente = construtor.Construir();

            _mocker.CreateInstance<DominioDeClientes>();
            _mocker.Use(_mapeador);
            _mocker.GetMock<INotificador>().Setup(n => n.ExisteMensagem()).Returns(true);
            var servico = _mocker.CreateInstance<CriarCliente>();

            #endregion Preparação do teste

            var clienteCadastrado = await servico.ExecutarAsync(novoCliente);

            #region Verificação do teste

            Assert.Null(clienteCadastrado);
            _mocker.Verify<INotificador>(n => n.AddMensagem(MensagensValidacaoCliente.EmailInvalido), Times.Once);

            #endregion Preparação do teste
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
        internal async Task DeveRetornarNuloEMensagemQuandoNomeDominioForInvalido(string nomeDominioInvalido)
        {
            #region Preparação do teste

            var construtor = ConstrutorDeClientesTeste.RetornarConstrutorDeNovoCliente(nomeDominio: nomeDominioInvalido);
            var novoCliente = construtor.Construir();

            _mocker.CreateInstance<DominioDeClientes>();
            _mocker.Use(_mapeador);
            _mocker.GetMock<INotificador>().Setup(n => n.ExisteMensagem()).Returns(true);
            var servico = _mocker.CreateInstance<CriarCliente>();

            #endregion Preparação do teste

            var clienteCadastrado = await servico.ExecutarAsync(novoCliente);

            #region Verificação do teste

            Assert.Null(clienteCadastrado);
            _mocker.Verify<INotificador>(n => n.AddMensagem(MensagensValidacaoCliente.NomeDominioInvalido), Times.Once);

            #endregion Preparação do teste
        }

        [Fact]
        internal async Task DeveRetornarNuloEMensagemQuandoEmailJaEstiverCadastrado()
        {
            #region Preparação do teste

            var emailJaCadastrado = _faker.Internet.Email();

            var construtor = ConstrutorDeClientesTeste.RetornarConstrutorDeNovoCliente(email: emailJaCadastrado);
            var novoCliente = construtor.Construir();

            _mocker.CreateInstance<DominioDeClientes>();
            _mocker.Use(_mapeador);
            _mocker.GetMock<IRepositorioDeClientes>().Setup(r => r.ExisteEmail(emailJaCadastrado)).ReturnsAsync(true);
            _mocker.GetMock<INotificador>().Setup(n => n.ExisteMensagem()).Returns(true);
            var servico = _mocker.CreateInstance<CriarCliente>();

            #endregion Preparação do teste

            var clienteCadastrado = await servico.ExecutarAsync(novoCliente);

            #region Verificação do teste

            Assert.Null(clienteCadastrado);
            _mocker.Verify<INotificador>(n => n.AddMensagem(MensagensValidacaoCliente.EmailJaCadastrado), Times.Once);

            #endregion Preparação do teste
        }

        [Fact]
        internal async Task DeveRetornarNuloEMensagemQuandoNomeDominioJaEstiverCadastrado()
        {
            #region Preparação do teste

            var nomeDominioJaCadastrado = _faker.Internet.DomainWord();

            var construtor = ConstrutorDeClientesTeste.RetornarConstrutorDeNovoCliente(nomeDominio: nomeDominioJaCadastrado);
            var novoCliente = construtor.Construir();

            _mocker.CreateInstance<DominioDeClientes>();
            _mocker.Use(_mapeador);
            _mocker.GetMock<IRepositorioDeClientes>().Setup(r => r.ExisteDominio(nomeDominioJaCadastrado)).ReturnsAsync(true);
            _mocker.GetMock<INotificador>().Setup(n => n.ExisteMensagem()).Returns(true);
            var servico = _mocker.CreateInstance<CriarCliente>();

            #endregion Preparação do teste

            var clienteCadastrado = await servico.ExecutarAsync(novoCliente);

            #region Verificação do teste

            Assert.Null(clienteCadastrado);
            _mocker.Verify<INotificador>(n => n.AddMensagem(MensagensValidacaoCliente.NomeDominioJaCadastrado), Times.Once);

            #endregion Preparação do teste
        }
    }
}

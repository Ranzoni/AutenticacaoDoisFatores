using AutenticacaoDoisFatores.Dominio.Compartilhados;
using AutenticacaoDoisFatores.Dominio.Compartilhados.Mensagens;
using AutenticacaoDoisFatores.Dominio.Dominios;
using AutenticacaoDoisFatores.Dominio.Entidades;
using AutenticacaoDoisFatores.Dominio.Repositorios;
using AutenticacaoDoisFatores.Dominio.Servicos;
using AutenticacaoDoisFatores.Servico.CasosDeUso.Clientes;
using AutenticacaoDoisFatores.Servico.Excecoes;
using AutenticacaoDoisFatores.Testes.Compartilhados;
using Bogus;
using Mensageiro;
using Moq;
using Moq.AutoMock;

namespace AutenticacaoDoisFatores.Testes.Servico.Clientes
{
    public class CriarClienteTeste
    {
        private readonly Faker _faker = new();
        private readonly AutoMocker _mocker = new();

        [Fact]
        internal async Task DeveExecutar()
        {
            #region Preparação do teste

            var construtorDeDto = ConstrutorDeClientesTeste.RetornarConstrutorDeNovo();
            var novoCliente = construtorDeDto.Construir();
            var construtorDeCliente = ConstrutorDeClientesTeste
                .RetornarConstrutor
                (
                    nome: novoCliente.Nome,
                    email: novoCliente.Email,
                    nomeDominio: novoCliente.NomeDominio,
                    chaveAcesso: novoCliente.ChaveDescriptografada()
                );
            var cliente = construtorDeCliente.ConstruirNovo();

            _mocker.CreateInstance<DominioDeClientes>();
            _mocker.GetMock<IRepositorioDeClientes>().Setup(r => r.BuscarUnicoAsync(It.IsAny<Guid>())).ReturnsAsync(cliente);

            var linkConfirmacaoCadastroParaTeste = _faker.Internet.UrlWithPath();

            var chaveAutenticacaoParaTeste = _faker.Random.AlphaNumeric(40);
            Environment.SetEnvironmentVariable("ADF_CHAVE_AUTENTICACAO", chaveAutenticacaoParaTeste);

            var servico = _mocker.CreateInstance<CriarCliente>();

            #endregion Preparação do teste

            var clienteCadastrado = await servico.ExecutarAsync(novoCliente, linkConfirmacaoCadastroParaTeste);

            #region Verificação do teste

            Assert.NotNull(clienteCadastrado);
            Assert.Equal(novoCliente.Nome, clienteCadastrado.Nome);
            Assert.Equal(novoCliente.Email, clienteCadastrado.Email);
            Assert.Equal(novoCliente.NomeDominio, clienteCadastrado.NomeDominio);
            Assert.NotEqual(novoCliente.ChaveAcesso, novoCliente.ChaveDescriptografada());
            _mocker.Verify<IRepositorioDeClientes>(r => r.CriarDominio(cliente.NomeDominio), Times.Once);
            _mocker.Verify<IRepositorioDeClientes>(r => r.Adicionar(It.IsAny<Cliente>()), Times.Once);
            _mocker.Verify<IRepositorioDeUsuarios>(r => r.Adicionar(It.IsAny<Usuario>(), cliente.NomeDominio), Times.Once);
            _mocker.Verify<IServicoDeEmail>(s =>
                s.Enviar
                    (
                        cliente.Email,
                        MensagensEnvioEmail.TituloConfirmacaoCadastroCliente.Descricao() ?? "",
                        It.IsAny<string>()
                    ),
                Times.Once);
            _mocker.Verify<INotificador>(n => n.AddMensagem(It.IsAny<MensagensValidacaoCliente>()), Times.Never);

            #endregion Preparação do teste
        }

        [Fact]
        internal void DeveMapearParaClienteAChaveAcessoCriptografada()
        {
            #region Preparação do teste

            var construtorDeDto = ConstrutorDeClientesTeste.RetornarConstrutorDeNovo();
            var novoCliente = construtorDeDto.Construir();

            #endregion Preparação do teste

            var cliente = (Cliente)(novoCliente);

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

            var construtor = ConstrutorDeClientesTeste.RetornarConstrutorDeNovo(nome: nomeInvalido);
            var novoCliente = construtor.Construir();

            _mocker.CreateInstance<DominioDeClientes>();
            _mocker.GetMock<INotificador>().Setup(n => n.ExisteMensagem()).Returns(true);

            var linkConfirmacaoCadastroParaTeste = _faker.Internet.UrlWithPath();

            var servico = _mocker.CreateInstance<CriarCliente>();

            #endregion Preparação do teste

            var clienteCadastrado = await servico.ExecutarAsync(novoCliente, linkConfirmacaoCadastroParaTeste);

            #region Verificação do teste

            Assert.Null(clienteCadastrado);
            _mocker.Verify<IRepositorioDeClientes>(r => r.Adicionar(It.IsAny<Cliente>()), Times.Never);
            _mocker.Verify<IServicoDeEmail>(s => s.Enviar(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
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

            var construtor = ConstrutorDeClientesTeste.RetornarConstrutorDeNovo(email: emailInvalido);
            var novoCliente = construtor.Construir();

            _mocker.CreateInstance<DominioDeClientes>();
            _mocker.GetMock<INotificador>().Setup(n => n.ExisteMensagem()).Returns(true);

            var linkConfirmacaoCadastroParaTeste = _faker.Internet.UrlWithPath();

            var servico = _mocker.CreateInstance<CriarCliente>();

            #endregion Preparação do teste

            var clienteCadastrado = await servico.ExecutarAsync(novoCliente, linkConfirmacaoCadastroParaTeste);

            #region Verificação do teste

            Assert.Null(clienteCadastrado);
            _mocker.Verify<IRepositorioDeClientes>(r => r.Adicionar(It.IsAny<Cliente>()), Times.Never);
            _mocker.Verify<IServicoDeEmail>(s => s.Enviar(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
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

            var construtor = ConstrutorDeClientesTeste.RetornarConstrutorDeNovo(nomeDominio: nomeDominioInvalido);
            var novoCliente = construtor.Construir();

            _mocker.CreateInstance<DominioDeClientes>();
            _mocker.GetMock<INotificador>().Setup(n => n.ExisteMensagem()).Returns(true);

            var linkConfirmacaoCadastroParaTeste = _faker.Internet.UrlWithPath();

            var servico = _mocker.CreateInstance<CriarCliente>();

            #endregion Preparação do teste

            var clienteCadastrado = await servico.ExecutarAsync(novoCliente, linkConfirmacaoCadastroParaTeste);

            #region Verificação do teste

            Assert.Null(clienteCadastrado);
            _mocker.Verify<IRepositorioDeClientes>(r => r.Adicionar(It.IsAny<Cliente>()), Times.Never);
            _mocker.Verify<IServicoDeEmail>(s => s.Enviar(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
            _mocker.Verify<INotificador>(n => n.AddMensagem(MensagensValidacaoCliente.NomeDominioInvalido), Times.Once);

            #endregion Preparação do teste
        }

        [Theory]
        [InlineData("")]
        [InlineData("       ")]
        [InlineData("T&st.1")]
        [InlineData("Teste.de.senh@.que.é.muito.grand3.Teste.de.senh@.qu")]
        [InlineData("testedesenha.123")]
        [InlineData("@Senha.para_teste")]
        [InlineData("TESTEDESENHA.123")]
        [InlineData("2senhaInvalida")]
        internal async Task DeveRetornarNuloEMensagemQuandoSenhaForInvalida(string senha)
        {
            #region Preparação do teste

            var construtor = ConstrutorDeClientesTeste.RetornarConstrutorDeNovo(senhaAdm: senha);
            var novoCliente = construtor.Construir();

            _mocker.CreateInstance<DominioDeClientes>();
            _mocker.GetMock<INotificador>().Setup(n => n.ExisteMensagem()).Returns(true);

            var linkConfirmacaoCadastroParaTeste = _faker.Internet.UrlWithPath();

            var servico = _mocker.CreateInstance<CriarCliente>();

            #endregion Preparação do teste

            var clienteCadastrado = await servico.ExecutarAsync(novoCliente, linkConfirmacaoCadastroParaTeste);

            #region Verificação do teste

            Assert.Null(clienteCadastrado);
            _mocker.Verify<IRepositorioDeClientes>(r => r.Adicionar(It.IsAny<Cliente>()), Times.Never);
            _mocker.Verify<IServicoDeEmail>(s => s.Enviar(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
            _mocker.Verify<INotificador>(n => n.AddMensagem(MensagensValidacaoUsuario.SenhaInvalida), Times.Once);

            #endregion Preparação do teste
        }

        [Fact]
        internal async Task DeveRetornarNuloEMensagemQuandoEmailJaEstiverCadastrado()
        {
            #region Preparação do teste

            var emailJaCadastrado = _faker.Internet.Email();

            var construtor = ConstrutorDeClientesTeste.RetornarConstrutorDeNovo(email: emailJaCadastrado);
            var novoCliente = construtor.Construir();

            _mocker.CreateInstance<DominioDeClientes>();
            _mocker.GetMock<IRepositorioDeClientes>().Setup(r => r.ExisteEmailAsync(emailJaCadastrado)).ReturnsAsync(true);
            _mocker.GetMock<INotificador>().Setup(n => n.ExisteMensagem()).Returns(true);

            var linkConfirmacaoCadastroParaTeste = _faker.Internet.UrlWithPath();

            var servico = _mocker.CreateInstance<CriarCliente>();

            #endregion Preparação do teste

            var clienteCadastrado = await servico.ExecutarAsync(novoCliente, linkConfirmacaoCadastroParaTeste);

            #region Verificação do teste

            Assert.Null(clienteCadastrado);
            _mocker.Verify<IRepositorioDeClientes>(r => r.Adicionar(It.IsAny<Cliente>()), Times.Never);
            _mocker.Verify<IServicoDeEmail>(s => s.Enviar(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
            _mocker.Verify<INotificador>(n => n.AddMensagem(MensagensValidacaoCliente.EmailJaCadastrado), Times.Once);

            #endregion Preparação do teste
        }

        [Fact]
        internal async Task DeveRetornarNuloEMensagemQuandoNomeDominioJaEstiverCadastrado()
        {
            #region Preparação do teste

            var nomeDominioJaCadastrado = _faker.Internet.DomainWord();

            var construtor = ConstrutorDeClientesTeste.RetornarConstrutorDeNovo(nomeDominio: nomeDominioJaCadastrado);
            var novoCliente = construtor.Construir();

            _mocker.CreateInstance<DominioDeClientes>();
            _mocker.GetMock<IRepositorioDeClientes>().Setup(r => r.ExisteDominioAsync(nomeDominioJaCadastrado)).ReturnsAsync(true);
            _mocker.GetMock<INotificador>().Setup(n => n.ExisteMensagem()).Returns(true);

            var linkConfirmacaoCadastroParaTeste = _faker.Internet.UrlWithPath();

            var servico = _mocker.CreateInstance<CriarCliente>();

            #endregion Preparação do teste

            var clienteCadastrado = await servico.ExecutarAsync(novoCliente, linkConfirmacaoCadastroParaTeste);

            #region Verificação do teste

            Assert.Null(clienteCadastrado);
            _mocker.Verify<IRepositorioDeClientes>(r => r.Adicionar(It.IsAny<Cliente>()), Times.Never);
            _mocker.Verify<IServicoDeEmail>(s => s.Enviar(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
            _mocker.Verify<INotificador>(n => n.AddMensagem(MensagensValidacaoCliente.NomeDominioJaCadastrado), Times.Once);

            #endregion Preparação do teste
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        internal async Task DeveRetornarExcecaoQuandoLinkDeConfirmacaoDeCadastroNaoForInformado(string linkConfirmacaoCadastroVazio)
        {
            #region Preparação do teste

            var construtor = ConstrutorDeClientesTeste.RetornarConstrutorDeNovo();
            var novoCliente = construtor.Construir();

            _mocker.CreateInstance<DominioDeClientes>();

            var servico = _mocker.CreateInstance<CriarCliente>();

            #endregion Preparação do teste

            var excecao = await Assert.ThrowsAsync<ExcecoesCriacaoCliente>(() => servico.ExecutarAsync(novoCliente, linkConfirmacaoCadastroVazio));

            #region Verificação do teste

            Assert.Equal(MensagensValidacaoCliente.LinkConfirmacaoCadastroNaoInformado.Descricao(), excecao.Message);
            _mocker.Verify<IRepositorioDeClientes>(r => r.Adicionar(It.IsAny<Cliente>()), Times.Never);
            _mocker.Verify<IServicoDeEmail>(s => s.Enviar(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);

            #endregion Preparação do teste
        }
    }
}

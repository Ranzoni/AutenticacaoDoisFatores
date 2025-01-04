using AutenticacaoDoisFatores.Dominio.Compartilhados.Mensagens;
using AutenticacaoDoisFatores.Dominio.Construtores;
using AutenticacaoDoisFatores.Dominio.Dominios;
using AutenticacaoDoisFatores.Dominio.Repositorios;
using AutenticacaoDoisFatores.Servico.CasosDeUso;
using AutenticacaoDoisFatores.Servico.DTO;
using AutenticacaoDoisFatores.Servico.Mapeadores;
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

            var nomeParaTeste = _faker.Company.CompanyName();
            var emailParaTeste = _faker.Internet.Email();
            var dominioParaTeste = _faker.Internet.DomainWord();

            var novoCliente = new NovoCliente(nome: nomeParaTeste, email: emailParaTeste, nomeDominio: dominioParaTeste);

            var cliente = new ConstrutorDeCliente()
                .ComId(Guid.NewGuid())
                .ComNome(nomeParaTeste)
                .ComEmail(emailParaTeste)
                .ComNomeDominio(dominioParaTeste)
                .ComDataCadastro(_faker.Date.Past())
                .ConstruirNovoCliente();

            _mocker.CreateInstance<DominioDeClientes>();
            _mocker.Use(_mapeador);
            _mocker.GetMock<IRepositorioDeClientes>().Setup(r => r.BuscarUnicoAsync(It.IsAny<Guid>())).ReturnsAsync(cliente);

            var servico = _mocker.CreateInstance<CriarCliente>();

            #endregion Preparação do teste

            var clienteCadastrado = await servico.ExecutarAsync(novoCliente);

            #region Verificação do teste

            Assert.NotNull(clienteCadastrado);
            Assert.Equal(nomeParaTeste, clienteCadastrado.Nome);
            Assert.Equal(emailParaTeste, clienteCadastrado.Email);
            _mocker.Verify<IRepositorioDeClientes>(r => r.CriarDominio(cliente.NomeDominio), Times.Once);
            _mocker.Verify<INotificador>(n => n.AddMensagem(It.IsAny<MensagensCliente>()), Times.Never);

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

            var emailParaTeste = _faker.Internet.Email();
            var dominioParaTeste = _faker.Internet.DomainName();

            var novoCliente = new NovoCliente(nome: nomeInvalido, email: emailParaTeste, nomeDominio: dominioParaTeste);

            _mocker.CreateInstance<DominioDeClientes>();
            _mocker.Use(_mapeador);
            _mocker.GetMock<INotificador>().Setup(n => n.ExisteMensagem()).Returns(true);
            var servico = _mocker.CreateInstance<CriarCliente>();

            #endregion Preparação do teste

            var clienteCadastrado = await servico.ExecutarAsync(novoCliente);

            #region Verificação do teste

            Assert.Null(clienteCadastrado);
            _mocker.Verify<INotificador>(n => n.AddMensagem(MensagensCliente.NomeInvalido), Times.Once);

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

            var nomeParaTeste = _faker.Company.CompanyName();
            var dominioParaTeste = _faker.Internet.DomainName();

            var novoCliente = new NovoCliente(nome: nomeParaTeste, email: emailInvalido, nomeDominio: dominioParaTeste);

            _mocker.CreateInstance<DominioDeClientes>();
            _mocker.Use(_mapeador);
            _mocker.GetMock<INotificador>().Setup(n => n.ExisteMensagem()).Returns(true);
            var servico = _mocker.CreateInstance<CriarCliente>();

            #endregion Preparação do teste

            var clienteCadastrado = await servico.ExecutarAsync(novoCliente);

            #region Verificação do teste

            Assert.Null(clienteCadastrado);
            _mocker.Verify<INotificador>(n => n.AddMensagem(MensagensCliente.EmailInvalido), Times.Once);

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

            var nomeParaTeste = _faker.Company.CompanyName();
            var emailParaTeste = _faker.Internet.Email();

            var novoCliente = new NovoCliente(nome: nomeParaTeste, email: emailParaTeste, nomeDominio: nomeDominioInvalido);

            _mocker.CreateInstance<DominioDeClientes>();
            _mocker.Use(_mapeador);
            _mocker.GetMock<INotificador>().Setup(n => n.ExisteMensagem()).Returns(true);
            var servico = _mocker.CreateInstance<CriarCliente>();

            #endregion Preparação do teste

            var clienteCadastrado = await servico.ExecutarAsync(novoCliente);

            #region Verificação do teste

            Assert.Null(clienteCadastrado);
            _mocker.Verify<INotificador>(n => n.AddMensagem(MensagensCliente.NomeDominioInvalido), Times.Once);

            #endregion Preparação do teste
        }
    }
}

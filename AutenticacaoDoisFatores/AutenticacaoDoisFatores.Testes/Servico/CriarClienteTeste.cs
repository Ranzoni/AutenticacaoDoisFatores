using AutenticacaoDoisFatores.Dominio.Compartilhados.Mensagens;
using AutenticacaoDoisFatores.Dominio.Dominios;
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

            var novoCliente = new NovoCliente(nome: nomeParaTeste, email: emailParaTeste);

            _mocker.CreateInstance<DominioDeClientes>();
            _mocker.Use(_mapeador);
            var servico = _mocker.CreateInstance<CriarCliente>();

            #endregion Preparação do teste

            var clienteCadastrado = await servico.ExecutarAsync(novoCliente);

            #region Verificação do teste

            Assert.NotNull(clienteCadastrado);
            Assert.Equal(nomeParaTeste, clienteCadastrado.Nome);
            Assert.Equal(emailParaTeste, clienteCadastrado.Email);
            _mocker.Verify<INotificador>(n => n.AddMensagem(It.IsAny<MensagensCliente>()), Times.Never);

            #endregion Preparação do teste
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("ab")]
        [InlineData("abcdefghijklmnopqrstuvwxyz0123456789abcdefghijklmno")]
        internal async Task DeveRetornarNuloQuandoNomeForInvalido(string nomeInvalido)
        {
            #region Preparação do teste

            var emailParaTeste = _faker.Internet.Email();

            var novoCliente = new NovoCliente(nome: nomeInvalido, email: emailParaTeste);

            _mocker.CreateInstance<DominioDeClientes>();
            _mocker.Use(_mapeador);
            var servico = _mocker.CreateInstance<CriarCliente>();

            #endregion Preparação do teste

            var clienteCadastrado = await servico.ExecutarAsync(novoCliente);

            #region Verificação do teste

            Assert.Null(clienteCadastrado);
            _mocker.Verify<INotificador>(n => n.AddMensagem(MensagensCliente.NomeInvalido), Times.Once);

            #endregion Preparação do teste
        }
    }
}

using AutenticacaoDoisFatores.Dominio.Dominios;
using AutenticacaoDoisFatores.Servico.CasosDeUso;
using AutenticacaoDoisFatores.Servico.DTO;
using AutenticacaoDoisFatores.Servico.Mapeadores;
using AutoMapper;
using Bogus;
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

            #endregion Preparação do teste
        }
    }
}

using AutenticacaoDoisFatores.Dominio.Compartilhados;
using AutenticacaoDoisFatores.Dominio.Compartilhados.Mensagens;
using AutenticacaoDoisFatores.Dominio.Dominios;
using AutenticacaoDoisFatores.Dominio.Excecoes;
using AutenticacaoDoisFatores.Dominio.Repositorios;
using AutenticacaoDoisFatores.Servico.Compartilhados;
using Moq;
using Moq.AutoMock;

namespace AutenticacaoDoisFatores.Testes.Dominio.Dominios
{
    public class DominioDeCodDeAutenticacaoTeste
    {
        [Fact]
        internal async Task DeveSalvar()
        {
            #region Preparação do teste

            var mocker = new AutoMocker();

            var dominio = mocker.CreateInstance<DominioDeCodDeAutenticacao>();

            var idUsuario = Guid.NewGuid();
            var codigoAutenticacao = Seguranca.GerarCodigoAutenticacao();

            #endregion

            await dominio.SalvarAsync(idUsuario, codigoAutenticacao);

            #region Verificação do teste

            mocker.Verify<IRepositorioDeCodigoDeAutenticacao>(r => r.SalvarAsync(idUsuario, codigoAutenticacao), Times.Once);

            #endregion
        }

        [Theory]
        [InlineData("")]
        [InlineData("  ")]
        internal async Task DeveRetornarExcecaoAoSalvarComCodigoVazio(string codigoAutenticacaoVazio)
        {
            #region Preparação do teste

            var mocker = new AutoMocker();

            var dominio = mocker.CreateInstance<DominioDeCodDeAutenticacao>();

            var idUsuario = Guid.NewGuid();

            #endregion

            var excecao = await Assert.ThrowsAsync<ExcecoesCodDeAutenticacao>(() => dominio.SalvarAsync(idUsuario, codigoAutenticacaoVazio));

            #region Verificação do teste

            Assert.NotNull(excecao);
            Assert.Equal(MensagensValidacaoCodDeAutenticacao.CodAutenticacaoVazio.Descricao(), excecao.Message);
            mocker.Verify<IRepositorioDeCodigoDeAutenticacao>(r => r.SalvarAsync(It.IsAny<Guid>(), It.IsAny<string>()), Times.Never);

            #endregion
        }

        [Fact]
        internal async Task DeveBuscar()
        {
            #region Preparação do teste

            var mocker = new AutoMocker();

            var dominio = mocker.CreateInstance<DominioDeCodDeAutenticacao>();

            var idUsuario = Guid.NewGuid();
            var codigoAutenticacao = Seguranca.GerarCodigoAutenticacao();

            mocker.GetMock<IRepositorioDeCodigoDeAutenticacao>().Setup(r => r.BuscarCodigoAsync(idUsuario)).ReturnsAsync(codigoAutenticacao);

            #endregion

            var retorno = await dominio.BuscarCodigoAsync(idUsuario);

            #region Verificação do teste

            Assert.NotNull(retorno);
            Assert.Equal(codigoAutenticacao, retorno);
            mocker.Verify<IRepositorioDeCodigoDeAutenticacao>(r => r.BuscarCodigoAsync(idUsuario), Times.Once);

            #endregion
        }

        [Fact]
        internal async Task DeveRetornarNuloAoBuscarCodigoQueNaoExiste()
        {
            #region Preparação do teste

            var mocker = new AutoMocker();

            var dominio = mocker.CreateInstance<DominioDeCodDeAutenticacao>();

            var idUsuario = Guid.NewGuid();

            #endregion

            var retorno = await dominio.BuscarCodigoAsync(idUsuario);

            #region Verificação do teste

            Assert.Null(retorno);
            mocker.Verify<IRepositorioDeCodigoDeAutenticacao>(r => r.BuscarCodigoAsync(idUsuario), Times.Once);

            #endregion
        }
    }
}

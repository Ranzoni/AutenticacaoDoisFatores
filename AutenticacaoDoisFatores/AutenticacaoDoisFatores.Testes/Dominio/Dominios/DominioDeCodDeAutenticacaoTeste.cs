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

            var dominio = mocker.CreateInstance<AuthCodeManager>();

            var idUsuario = Guid.NewGuid();
            var codigoAutenticacao = Security.GenerateAuthCode();

            #endregion

            await dominio.SaveAsync(idUsuario, codigoAutenticacao);

            #region Verificação do teste

            mocker.Verify<IAuthCodeRepository>(r => r.SaveAsync(idUsuario, codigoAutenticacao), Times.Once);

            #endregion
        }

        [Theory]
        [InlineData("")]
        [InlineData("  ")]
        internal async Task DeveRetornarExcecaoAoSalvarComCodigoVazio(string codigoAutenticacaoVazio)
        {
            #region Preparação do teste

            var mocker = new AutoMocker();

            var dominio = mocker.CreateInstance<AuthCodeManager>();

            var idUsuario = Guid.NewGuid();

            #endregion

            var excecao = await Assert.ThrowsAsync<AuthCodeException>(() => dominio.SaveAsync(idUsuario, codigoAutenticacaoVazio));

            #region Verificação do teste

            Assert.NotNull(excecao);
            Assert.Equal(AuthCodeValidationMessages.EmptyAuthCode.Description(), excecao.Message);
            mocker.Verify<IAuthCodeRepository>(r => r.SaveAsync(It.IsAny<Guid>(), It.IsAny<string>()), Times.Never);

            #endregion
        }

        [Fact]
        internal async Task DeveBuscar()
        {
            #region Preparação do teste

            var mocker = new AutoMocker();

            var dominio = mocker.CreateInstance<AuthCodeManager>();

            var idUsuario = Guid.NewGuid();
            var codigoAutenticacao = Security.GenerateAuthCode();

            mocker.GetMock<IAuthCodeRepository>().Setup(r => r.GetByCodeAsync(idUsuario)).ReturnsAsync(codigoAutenticacao);

            #endregion

            var retorno = await dominio.GetCodeAsync(idUsuario);

            #region Verificação do teste

            Assert.NotNull(retorno);
            Assert.Equal(codigoAutenticacao, retorno);
            mocker.Verify<IAuthCodeRepository>(r => r.GetByCodeAsync(idUsuario), Times.Once);

            #endregion
        }

        [Fact]
        internal async Task DeveRetornarNuloAoBuscarCodigoQueNaoExiste()
        {
            #region Preparação do teste

            var mocker = new AutoMocker();

            var dominio = mocker.CreateInstance<AuthCodeManager>();

            var idUsuario = Guid.NewGuid();

            #endregion

            var retorno = await dominio.GetCodeAsync(idUsuario);

            #region Verificação do teste

            Assert.Null(retorno);
            mocker.Verify<IAuthCodeRepository>(r => r.GetByCodeAsync(idUsuario), Times.Once);

            #endregion
        }
    }
}

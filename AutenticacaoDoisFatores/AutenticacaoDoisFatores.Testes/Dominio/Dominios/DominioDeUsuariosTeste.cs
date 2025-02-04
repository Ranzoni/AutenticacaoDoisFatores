using AutenticacaoDoisFatores.Dominio.Dominios;
using AutenticacaoDoisFatores.Dominio.Repositorios;
using AutenticacaoDoisFatores.Testes.Compartilhados;
using Moq;
using Moq.AutoMock;

namespace AutenticacaoDoisFatores.Testes.Dominio.Dominios
{
    public class DominioDeUsuariosTeste
    {
        [Fact]
        internal async Task DeveCriarUsuario()
        {
            #region Preparação do teste

            var mocker = new AutoMocker();

            var dominio = mocker.CreateInstance<DominioDeUsuario>();

            var usuarioParaCriar = ConstrutorDeUsuariosTeste
                .RetornarConstrutorDeNovo()
                .ConstruirNovo();

            #endregion

            await dominio.CriarUsuarioAsync(usuarioParaCriar);

            #region Verificação do teste

            mocker.Verify<IRepositorioDeUsuarios>(r => r.Adicionar(usuarioParaCriar), Times.Once);
            mocker.Verify<IRepositorioDeUsuarios>(r => r.SalvarAlteracoesAsync(), Times.Once);

            #endregion
        }
    }
}

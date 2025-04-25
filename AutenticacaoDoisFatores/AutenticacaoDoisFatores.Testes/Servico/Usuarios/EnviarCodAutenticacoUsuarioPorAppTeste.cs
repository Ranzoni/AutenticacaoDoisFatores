using AutenticacaoDoisFatores.Servico.CasosDeUso.Usuarios.Autenticadores.AutenticacoesDoisFatores;
using AutenticacaoDoisFatores.Testes.Compartilhados;
using Mensageiro;
using Moq.AutoMock;

namespace AutenticacaoDoisFatores.Testes.Servico.Usuarios
{
    public class EnviarCodAutenticacoUsuarioPorAppTeste
    {
        [Fact]
        internal async Task DeveAutenticarPorApp()
        {
            #region Preparação do teste

            var mocker = new AutoMocker();

            var notificador = mocker.GetMock<INotificador>().Object;
            
            var servico = new AutenticadorUsuarioEmDoisFatoresPorApp(notificador);

            var usuario = ConstrutorDeUsuariosTeste
                .RetornarConstrutor(ativo: true)
                .ConstruirCadastrado();

            #endregion

            var retorno = await servico.EnviarAsync(usuario);

            #region Verificação do teste

            Assert.NotNull(retorno);
            Assert.NotEmpty(retorno.Token);

            #endregion
        }

        [Fact]
        internal async Task NaoDeveAutenticarPorAppParaUsuarioInativo()
        {
            #region Preparação do teste

            var mocker = new AutoMocker();

            var notificador = mocker.GetMock<INotificador>().Object;

            var servico = new AutenticadorUsuarioEmDoisFatoresPorApp(notificador);

            var usuario = ConstrutorDeUsuariosTeste
                .RetornarConstrutor(ativo: false)
                .ConstruirCadastrado();

            mocker.GetMock<INotificador>().Setup(n => n.ExisteMensagem()).Returns(true);

            #endregion

            var retorno = await servico.EnviarAsync(usuario);

            #region Verificação do teste

            Assert.Null(retorno);

            #endregion
        }
    }
}

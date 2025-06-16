using AutenticacaoDoisFatores.Dominio.Compartilhados;
using AutenticacaoDoisFatores.Dominio.Compartilhados.Mensagens;
using AutenticacaoDoisFatores.Dominio.Repositorios;
using AutenticacaoDoisFatores.Dominio.Servicos;
using AutenticacaoDoisFatores.Servico.CasosDeUso.Usuarios.Autenticadores.AutenticacoesDoisFatores;
using AutenticacaoDoisFatores.Testes.Compartilhados;
using Mensageiro;
using Moq;
using Moq.AutoMock;

namespace AutenticacaoDoisFatores.Testes.Servico.Usuarios
{
    public class EnviarCodAutenticacaoPorEmailTeste
    {
        [Fact]
        internal async Task DeveEnviarEmail()
        {
            #region Preparação do teste

            var mocker = new AutoMocker();

            var servico = mocker.CreateInstance<UserTwoFactorAuthByEmail>();

            var usuario = ConstrutorDeUsuariosTeste
                .RetornarConstrutor(ativo: true)
                .Build();

            #endregion

            var resposta = await servico.EnviarAsync(usuario);

            #region Verificação do teste

            Assert.NotNull(resposta);
            Assert.False(resposta.Token.EstaVazio());
            mocker.Verify<IAuthCodeRepository>(r => r.SalvarAsync(usuario.Id, It.IsAny<string>()), Times.Once);
            mocker.Verify<IEmailService>(s => s.Enviar(usuario.Email, It.IsAny<string>(), It.IsAny<string>()), Times.Once);

            #endregion
        }

        [Fact]
        internal async Task NaoDeveEnviarEmailParaUsuarioInativo()
        {
            #region Preparação do teste

            var mocker = new AutoMocker();

            var servico = mocker.CreateInstance<UserTwoFactorAuthByEmail>();

            var usuario = ConstrutorDeUsuariosTeste
                .RetornarConstrutor(ativo: false)
                .Build();

            #endregion

            var resposta = await servico.EnviarAsync(usuario);

            #region Verificação do teste

            Assert.Null(resposta);
            mocker.Verify<IAuthCodeRepository>(r => r.SaveAsync(It.IsAny<Guid>(), It.IsAny<string>()), Times.Never);
            mocker.Verify<IEmailService>(s => s.Send(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
            mocker.Verify<INotificador>(n => n.AddMensagemNaoEncontrado(UserValidationMessages.UserNotFound), Times.Once);

            #endregion
        }
    }
}

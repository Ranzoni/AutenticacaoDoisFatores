using AutenticacaoDoisFatores.Dominio.Compartilhados.Mensagens;
using AutenticacaoDoisFatores.Dominio.Repositorios;
using AutenticacaoDoisFatores.Dominio.Servicos;
using AutenticacaoDoisFatores.Servico.CasosDeUso.Usuarios;
using AutenticacaoDoisFatores.Testes.Compartilhados;
using Bogus;
using Mensageiro;
using Moq;
using Moq.AutoMock;

namespace AutenticacaoDoisFatores.Testes.Servico.Usuarios
{
    public class EnviarEmailParaUsuarioTeste
    {
        [Fact]
        internal async Task DeveEnviarEmail()
        {
            #region Preparação do teste

            var mocker = new AutoMocker();

            var servico = mocker.CreateInstance<EnviarEmailParaUsuario>();

            var faker = new Faker();

            var idUsuario = Guid.NewGuid();

            var usuario = ConstrutorDeUsuariosTeste
                .RetornarConstrutor(id: idUsuario, ativo: false)
                .Build();

            var envioEmailParaUsuario = ConstrutorDeEnvioEmailParaUsuarioTeste
                .RetornarConstrutor()
                .Construir();

            mocker.GetMock<IUserRepository>().Setup(r => r.GetByIdAsync(idUsuario)).ReturnsAsync(usuario);

            #endregion

            await servico.ExecutarAsync(idUsuario, envioEmailParaUsuario);

            #region Verificação do teste

            mocker.Verify<IEmailService>(s => s.Enviar(usuario.Email, It.IsAny<string>(), It.IsAny<string>()), Times.Once);

            #endregion
        }

        [Fact]
        internal async Task NaoDeveEnviarEmailParaUsuarioInexistente()
        {
            #region Preparação do teste

            var mocker = new AutoMocker();

            var servico = mocker.CreateInstance<EnviarEmailParaUsuario>();

            var faker = new Faker();

            var idUsuario = Guid.NewGuid();

            var envioEmailParaUsuario = ConstrutorDeEnvioEmailParaUsuarioTeste
                .RetornarConstrutor()
                .Construir();

            #endregion

            await servico.ExecutarAsync(idUsuario, envioEmailParaUsuario);

            #region Verificação do teste

            mocker.Verify<IEmailService>(s => s.Send(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
            mocker.Verify<INotificador>(n => n.AddMensagemNaoEncontrado(UserValidationMessages.UserNotFound), Times.Once);

            #endregion
        }

        [Fact]
        internal async Task NaoDeveEnviarEmailParaUsuarioAdmin()
        {
            #region Preparação do teste

            var mocker = new AutoMocker();

            var servico = mocker.CreateInstance<EnviarEmailParaUsuario>();

            var faker = new Faker();

            var idUsuario = Guid.NewGuid();

            var envioEmailParaUsuario = ConstrutorDeEnvioEmailParaUsuarioTeste
                .RetornarConstrutor()
                .Construir();

            var usuario = ConstrutorDeUsuariosTeste
                .RetornarConstrutor(id: idUsuario, ehAdm: true)
                .Build();

            mocker.GetMock<IUserRepository>().Setup(r => r.GetByIdAsync(idUsuario)).ReturnsAsync(usuario);

            #endregion

            await servico.ExecutarAsync(idUsuario, envioEmailParaUsuario);

            #region Verificação do teste

            mocker.Verify<IEmailService>(s => s.Send(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
            mocker.Verify<INotificador>(n => n.AddMensagemNaoEncontrado(UserValidationMessages.UserNotFound), Times.Once);

            #endregion
        }
    }
}

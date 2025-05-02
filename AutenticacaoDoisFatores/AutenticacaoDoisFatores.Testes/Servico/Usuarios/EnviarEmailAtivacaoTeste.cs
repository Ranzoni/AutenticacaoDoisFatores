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
    public class EnviarEmailAtivacaoTeste
    {
        [Fact]
        internal async Task DeveEnviarEmailAtivacao()
        {
            #region Preparação do teste

            var mocker = new AutoMocker();

            var servico = mocker.CreateInstance<EnviarEmailAtivacao>();

            var faker = new Faker();

            var idUsuario = Guid.NewGuid();
            var linkAtivacao = faker.Internet.Url();

            var usuario = ConstrutorDeUsuariosTeste
                .RetornarConstrutor(id: idUsuario, ativo: false)
                .ConstruirCadastrado();

            mocker.GetMock<IRepositorioDeUsuarios>().Setup(r => r.BuscarUnicoAsync(idUsuario)).ReturnsAsync(usuario);

            #endregion

            await servico.ExecutarAsync(idUsuario, linkAtivacao);

            #region Verificação do teste

            mocker.Verify<IServicoDeEmail>(s => s.Enviar(usuario.Email, It.IsAny<string>(), It.IsAny<string>()), Times.Once);

            #endregion
        }

        [Fact]
        internal async Task NaoDeveEnviarEmailAtivacaoParaUsuarioInexistente()
        {
            #region Preparação do teste

            var mocker = new AutoMocker();

            var servico = mocker.CreateInstance<EnviarEmailAtivacao>();

            var faker = new Faker();

            var idUsuario = Guid.NewGuid();
            var linkAtivacao = faker.Internet.Url();

            #endregion

            await servico.ExecutarAsync(idUsuario, linkAtivacao);

            #region Verificação do teste

            mocker.Verify<IServicoDeEmail>(s => s.Enviar(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
            mocker.Verify<INotificador>(n => n.AddMensagemNaoEncontrado(MensagensValidacaoUsuario.UsuarioNaoEncontrado), Times.Once);

            #endregion
        }

        [Fact]
        internal async Task NaoDeveEnviarEmailAtivacaoParaUsuarioAdmin()
        {
            #region Preparação do teste

            var mocker = new AutoMocker();

            var servico = mocker.CreateInstance<EnviarEmailAtivacao>();

            var faker = new Faker();

            var idUsuario = Guid.NewGuid();
            var linkAtivacao = faker.Internet.Url();

            var usuario = ConstrutorDeUsuariosTeste
                .RetornarConstrutor(id: idUsuario, ehAdm: true)
                .ConstruirCadastrado();

            mocker.GetMock<IRepositorioDeUsuarios>().Setup(r => r.BuscarUnicoAsync(idUsuario)).ReturnsAsync(usuario);

            #endregion

            await servico.ExecutarAsync(idUsuario, linkAtivacao);

            #region Verificação do teste

            mocker.Verify<IServicoDeEmail>(s => s.Enviar(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
            mocker.Verify<INotificador>(n => n.AddMensagemNaoEncontrado(MensagensValidacaoUsuario.UsuarioNaoEncontrado), Times.Once);

            #endregion
        }

        [Fact]
        internal async Task NaoDeveEnviarEmailAtivacaoParaUsuarioAtivo()
        {
            #region Preparação do teste

            var mocker = new AutoMocker();

            var servico = mocker.CreateInstance<EnviarEmailAtivacao>();

            var faker = new Faker();

            var idUsuario = Guid.NewGuid();
            var linkAtivacao = faker.Internet.Url();

            var usuario = ConstrutorDeUsuariosTeste
                .RetornarConstrutor(id: idUsuario, ativo: true)
                .ConstruirCadastrado();

            mocker.GetMock<IRepositorioDeUsuarios>().Setup(r => r.BuscarUnicoAsync(idUsuario)).ReturnsAsync(usuario);

            #endregion

            await servico.ExecutarAsync(idUsuario, linkAtivacao);

            #region Verificação do teste

            mocker.Verify<IServicoDeEmail>(s => s.Enviar(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
            mocker.Verify<INotificador>(n => n.AddMensagem(MensagensValidacaoUsuario.UsuarioJaAtivo), Times.Once);

            #endregion
        }
    }
}

using AutenticacaoDoisFatores.Dominio.Compartilhados.Mensagens;
using AutenticacaoDoisFatores.Dominio.Entidades;
using AutenticacaoDoisFatores.Dominio.Repositorios;
using AutenticacaoDoisFatores.Servico.CasosDeUso.Usuarios;
using AutenticacaoDoisFatores.Servico.DTO.Usuarios;
using AutenticacaoDoisFatores.Testes.Compartilhados;
using Mensageiro;
using Moq;
using Moq.AutoMock;

namespace AutenticacaoDoisFatores.Testes.Servico.Usuarios
{
    public class AlterarEmailUsuarioTeste
    {
        [Fact]
        internal async Task DeveAlterarEmailUsuario()
        {
            #region Preparação do teste

            var mocker = new AutoMocker();

            var servico = mocker.CreateInstance<AlterarEmailUsuario>();

            var idUsuario = Guid.NewGuid();
            var novoEmail = "teste@novoemail.com";
            var trocarEmailUsuario = new TrocarEmailUsuario(novoEmail);

            var usuarioCadastrado = ConstrutorDeUsuariosTeste
                .RetornarConstrutor(id: idUsuario, ativo: true, ehAdm: false)
                .ConstruirCadastrado();

            mocker.GetMock<IRepositorioDeUsuarios>().Setup(r => r.BuscarUnicoAsync(idUsuario)).ReturnsAsync(usuarioCadastrado);

            #endregion

            await servico.ExecutarAsync(idUsuario, trocarEmailUsuario);

            #region Verificação do teste

            Assert.Equal(novoEmail, usuarioCadastrado.Email);
            mocker.Verify<IRepositorioDeUsuarios>(r => r.Editar(usuarioCadastrado), Times.Once);
            mocker.Verify<IRepositorioDeUsuarios>(r => r.SalvarAlteracoesAsync(), Times.Once);

            #endregion
        }

        [Fact]
        internal async Task NaoDeveAlterarEmailUsuarioInexistente()
        {
            #region Preparação do teste

            var mocker = new AutoMocker();

            var servico = mocker.CreateInstance<AlterarEmailUsuario>();

            var idUsuario = Guid.NewGuid();
            var novoEmail = "teste@novoemail.com";

            var trocarEmailUsuario = new TrocarEmailUsuario(novoEmail);

            mocker.GetMock<INotificador>().Setup(n => n.ExisteMensagem()).Returns(true);

            #endregion

            await servico.ExecutarAsync(idUsuario, trocarEmailUsuario);

            #region Verificação do teste

            mocker.Verify<IRepositorioDeUsuarios>(r => r.Editar(It.IsAny<Usuario>()), Times.Never);
            mocker.Verify<IRepositorioDeUsuarios>(r => r.SalvarAlteracoesAsync(), Times.Never);
            mocker.Verify<INotificador>(n => n.AddMensagemNaoEncontrado(MensagensValidacaoUsuario.UsuarioNaoEncontrado), Times.Once);

            #endregion
        }

        [Fact]
        internal async Task NaoDeveAlterarEmailUsuarioInativo()
        {
            #region Preparação do teste

            var mocker = new AutoMocker();

            var servico = mocker.CreateInstance<AlterarEmailUsuario>();

            var idUsuario = Guid.NewGuid();
            var novoEmail = "teste@novoemail.com";
            var trocarEmailUsuario = new TrocarEmailUsuario(novoEmail);

            var usuarioCadastrado = ConstrutorDeUsuariosTeste
                .RetornarConstrutor(id: idUsuario, ativo: false, ehAdm: false)
                .ConstruirCadastrado();

            mocker.GetMock<IRepositorioDeUsuarios>().Setup(r => r.BuscarUnicoAsync(idUsuario)).ReturnsAsync(usuarioCadastrado);
            mocker.GetMock<INotificador>().Setup(n => n.ExisteMensagem()).Returns(true);

            #endregion

            await servico.ExecutarAsync(idUsuario, trocarEmailUsuario);

            #region Verificação do teste

            Assert.NotEqual(novoEmail, usuarioCadastrado.Email);
            mocker.Verify<IRepositorioDeUsuarios>(r => r.Editar(It.IsAny<Usuario>()), Times.Never);
            mocker.Verify<IRepositorioDeUsuarios>(r => r.SalvarAlteracoesAsync(), Times.Never);
            mocker.Verify<INotificador>(n => n.AddMensagemNaoEncontrado(MensagensValidacaoUsuario.UsuarioNaoEncontrado), Times.Once);

            #endregion
        }

        [Fact]
        internal async Task NaoDeveAlterarEmailUsuarioAdmin()
        {
            #region Preparação do teste

            var mocker = new AutoMocker();

            var servico = mocker.CreateInstance<AlterarEmailUsuario>();

            var idUsuario = Guid.NewGuid();
            var novoEmail = "teste@novoemail.com";
            var trocarEmailUsuario = new TrocarEmailUsuario(novoEmail);

            var usuarioCadastrado = ConstrutorDeUsuariosTeste
                .RetornarConstrutor(id: idUsuario, ativo: true, ehAdm: true)
                .ConstruirCadastrado();

            mocker.GetMock<IRepositorioDeUsuarios>().Setup(r => r.BuscarUnicoAsync(idUsuario)).ReturnsAsync(usuarioCadastrado);
            mocker.GetMock<INotificador>().Setup(n => n.ExisteMensagem()).Returns(true);

            #endregion

            await servico.ExecutarAsync(idUsuario, trocarEmailUsuario);

            #region Verificação do teste

            Assert.NotEqual(novoEmail, usuarioCadastrado.Email);
            mocker.Verify<IRepositorioDeUsuarios>(r => r.Editar(It.IsAny<Usuario>()), Times.Never);
            mocker.Verify<IRepositorioDeUsuarios>(r => r.SalvarAlteracoesAsync(), Times.Never);
            mocker.Verify<INotificador>(n => n.AddMensagemNaoEncontrado(MensagensValidacaoUsuario.UsuarioNaoEncontrado), Times.Once);

            #endregion
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("@")]
        [InlineData("a@")]
        [InlineData("a@.")]
        [InlineData("a@.com")]
        [InlineData("@.")]
        [InlineData("@.com")]
        [InlineData("@dominio.com")]
        [InlineData("abcdefghijklmnopqrstuvwxyz0123456789abcdefghijklmnopqrstuvwxyz0123456789abcdefghijklmnopqrstuvwxyz0123456789abcdefghijklmnopqrstuvwxyz0123456789abcdefghijklmnopqrstuvwxyz0123456789abcdefghijklmnopqrstuvwxyz0123456789abcdefghijklmnopqrstuvwxyz0123456789abcde")]
        internal async Task NaoDeveAlterarEmailUsuarioQuandoEhInvaliido(string emailInvalido)
        {
            #region Preparação do teste

            var mocker = new AutoMocker();

            var servico = mocker.CreateInstance<AlterarEmailUsuario>();

            var idUsuario = Guid.NewGuid();
            var trocarEmailUsuario = new TrocarEmailUsuario(emailInvalido);

            var usuarioCadastrado = ConstrutorDeUsuariosTeste
                .RetornarConstrutor(id: idUsuario, ativo: true, ehAdm: false)
                .ConstruirCadastrado();

            mocker.GetMock<IRepositorioDeUsuarios>().Setup(r => r.BuscarUnicoAsync(idUsuario)).ReturnsAsync(usuarioCadastrado);
            mocker.GetMock<INotificador>().Setup(n => n.ExisteMensagem()).Returns(true);

            #endregion

            await servico.ExecutarAsync(idUsuario, trocarEmailUsuario);

            #region Verificação do teste

            Assert.NotEqual(emailInvalido, usuarioCadastrado.Email);
            mocker.Verify<IRepositorioDeUsuarios>(r => r.Editar(It.IsAny<Usuario>()), Times.Never);
            mocker.Verify<IRepositorioDeUsuarios>(r => r.SalvarAlteracoesAsync(), Times.Never);
            mocker.Verify<INotificador>(n => n.AddMensagem(MensagensValidacaoUsuario.EmailInvalido), Times.Once);

            #endregion
        }

        [Fact]
        internal async Task NaoDeveAlterarEmailParaUmJaCadastrado()
        {
            #region Preparação do teste

            var mocker = new AutoMocker();

            var servico = mocker.CreateInstance<AlterarEmailUsuario>();

            var idUsuario = Guid.NewGuid();
            string novoEmail = "teste@novoemail.com";
            var trocarEmailUsuario = new TrocarEmailUsuario(novoEmail);

            var usuarioCadastrado = ConstrutorDeUsuariosTeste
                .RetornarConstrutor(id: idUsuario, ativo: true, ehAdm: false)
                .ConstruirCadastrado();

            mocker.GetMock<IRepositorioDeUsuarios>().Setup(r => r.BuscarUnicoAsync(idUsuario)).ReturnsAsync(usuarioCadastrado);
            mocker.GetMock<IRepositorioDeUsuarios>().Setup(r => r.ExisteEmailAsync(novoEmail, It.IsAny<Guid?>())).ReturnsAsync(true);
            mocker.GetMock<INotificador>().Setup(n => n.ExisteMensagem()).Returns(true);

            #endregion

            await servico.ExecutarAsync(idUsuario, trocarEmailUsuario);

            #region Verificação do teste

            Assert.NotEqual(novoEmail, usuarioCadastrado.Email);
            mocker.Verify<IRepositorioDeUsuarios>(r => r.Editar(It.IsAny<Usuario>()), Times.Never);
            mocker.Verify<IRepositorioDeUsuarios>(r => r.SalvarAlteracoesAsync(), Times.Never);
            mocker.Verify<INotificador>(n => n.AddMensagem(MensagensValidacaoUsuario.EmailJaCadastrado), Times.Once);

            #endregion
        }
    }
}

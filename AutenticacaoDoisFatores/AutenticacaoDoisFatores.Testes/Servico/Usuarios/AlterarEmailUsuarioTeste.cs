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

            var servico = mocker.CreateInstance<ChangeUserEmail>();

            var idUsuario = Guid.NewGuid();
            var novoEmail = "teste@novoemail.com";
            var trocarEmailUsuario = new UserEmailChange(novoEmail);

            var usuarioCadastrado = ConstrutorDeUsuariosTeste
                .RetornarConstrutor(id: idUsuario, ativo: true, ehAdm: false)
                .Build();

            mocker.GetMock<IUserRepository>().Setup(r => r.GetByIdAsync(idUsuario)).ReturnsAsync(usuarioCadastrado);

            #endregion

            await servico.ExecutarAsync(idUsuario, trocarEmailUsuario);

            #region Verificação do teste

            Assert.Equal(novoEmail, usuarioCadastrado.Email);
            mocker.Verify<IUserRepository>(r => r.Editar(usuarioCadastrado), Times.Once);
            mocker.Verify<IUserRepository>(r => r.SaveChangesAsync(), Times.Once);

            #endregion
        }

        [Fact]
        internal async Task NaoDeveAlterarEmailUsuarioInexistente()
        {
            #region Preparação do teste

            var mocker = new AutoMocker();

            var servico = mocker.CreateInstance<ChangeUserEmail>();

            var idUsuario = Guid.NewGuid();
            var novoEmail = "teste@novoemail.com";

            var trocarEmailUsuario = new UserEmailChange(novoEmail);

            mocker.GetMock<INotificador>().Setup(n => n.ExisteMensagem()).Returns(true);

            #endregion

            await servico.ExecutarAsync(idUsuario, trocarEmailUsuario);

            #region Verificação do teste

            mocker.Verify<IUserRepository>(r => r.Update(It.IsAny<User>()), Times.Never);
            mocker.Verify<IUserRepository>(r => r.SaveChangesAsync(), Times.Never);
            mocker.Verify<INotificador>(n => n.AddMensagemNaoEncontrado(UserValidationMessages.UserNotFound), Times.Once);

            #endregion
        }

        [Fact]
        internal async Task NaoDeveAlterarEmailUsuarioInativo()
        {
            #region Preparação do teste

            var mocker = new AutoMocker();

            var servico = mocker.CreateInstance<ChangeUserEmail>();

            var idUsuario = Guid.NewGuid();
            var novoEmail = "teste@novoemail.com";
            var trocarEmailUsuario = new UserEmailChange(novoEmail);

            var usuarioCadastrado = ConstrutorDeUsuariosTeste
                .RetornarConstrutor(id: idUsuario, ativo: false, ehAdm: false)
                .Build();

            mocker.GetMock<IUserRepository>().Setup(r => r.GetByIdAsync(idUsuario)).ReturnsAsync(usuarioCadastrado);
            mocker.GetMock<INotificador>().Setup(n => n.ExisteMensagem()).Returns(true);

            #endregion

            await servico.ExecutarAsync(idUsuario, trocarEmailUsuario);

            #region Verificação do teste

            Assert.NotEqual(novoEmail, usuarioCadastrado.Email);
            mocker.Verify<IUserRepository>(r => r.Update(It.IsAny<User>()), Times.Never);
            mocker.Verify<IUserRepository>(r => r.SaveChangesAsync(), Times.Never);
            mocker.Verify<INotificador>(n => n.AddMensagemNaoEncontrado(UserValidationMessages.UserNotFound), Times.Once);

            #endregion
        }

        [Fact]
        internal async Task NaoDeveAlterarEmailUsuarioAdmin()
        {
            #region Preparação do teste

            var mocker = new AutoMocker();

            var servico = mocker.CreateInstance<ChangeUserEmail>();

            var idUsuario = Guid.NewGuid();
            var novoEmail = "teste@novoemail.com";
            var trocarEmailUsuario = new UserEmailChange(novoEmail);

            var usuarioCadastrado = ConstrutorDeUsuariosTeste
                .RetornarConstrutor(id: idUsuario, ativo: true, ehAdm: true)
                .Build();

            mocker.GetMock<IUserRepository>().Setup(r => r.GetByIdAsync(idUsuario)).ReturnsAsync(usuarioCadastrado);
            mocker.GetMock<INotificador>().Setup(n => n.ExisteMensagem()).Returns(true);

            #endregion

            await servico.ExecutarAsync(idUsuario, trocarEmailUsuario);

            #region Verificação do teste

            Assert.NotEqual(novoEmail, usuarioCadastrado.Email);
            mocker.Verify<IUserRepository>(r => r.Update(It.IsAny<User>()), Times.Never);
            mocker.Verify<IUserRepository>(r => r.SaveChangesAsync(), Times.Never);
            mocker.Verify<INotificador>(n => n.AddMensagemNaoEncontrado(UserValidationMessages.UserNotFound), Times.Once);

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

            var servico = mocker.CreateInstance<ChangeUserEmail>();

            var idUsuario = Guid.NewGuid();
            var trocarEmailUsuario = new UserEmailChange(emailInvalido);

            var usuarioCadastrado = ConstrutorDeUsuariosTeste
                .RetornarConstrutor(id: idUsuario, ativo: true, ehAdm: false)
                .Build();

            mocker.GetMock<IUserRepository>().Setup(r => r.GetByIdAsync(idUsuario)).ReturnsAsync(usuarioCadastrado);
            mocker.GetMock<INotificador>().Setup(n => n.ExisteMensagem()).Returns(true);

            #endregion

            await servico.ExecutarAsync(idUsuario, trocarEmailUsuario);

            #region Verificação do teste

            Assert.NotEqual(emailInvalido, usuarioCadastrado.Email);
            mocker.Verify<IUserRepository>(r => r.Update(It.IsAny<User>()), Times.Never);
            mocker.Verify<IUserRepository>(r => r.SaveChangesAsync(), Times.Never);
            mocker.Verify<INotificador>(n => n.AddMensagem(UserValidationMessages.InvalidEmail), Times.Once);

            #endregion
        }

        [Fact]
        internal async Task NaoDeveAlterarEmailParaUmJaCadastrado()
        {
            #region Preparação do teste

            var mocker = new AutoMocker();

            var servico = mocker.CreateInstance<ChangeUserEmail>();

            var idUsuario = Guid.NewGuid();
            string novoEmail = "teste@novoemail.com";
            var trocarEmailUsuario = new UserEmailChange(novoEmail);

            var usuarioCadastrado = ConstrutorDeUsuariosTeste
                .RetornarConstrutor(id: idUsuario, ativo: true, ehAdm: false)
                .Build();

            mocker.GetMock<IUserRepository>().Setup(r => r.GetByIdAsync(idUsuario)).ReturnsAsync(usuarioCadastrado);
            mocker.GetMock<IUserRepository>().Setup(r => r.EmailExistsAsync(novoEmail, It.IsAny<Guid?>())).ReturnsAsync(true);
            mocker.GetMock<INotificador>().Setup(n => n.ExisteMensagem()).Returns(true);

            #endregion

            await servico.ExecutarAsync(idUsuario, trocarEmailUsuario);

            #region Verificação do teste

            Assert.NotEqual(novoEmail, usuarioCadastrado.Email);
            mocker.Verify<IUserRepository>(r => r.Update(It.IsAny<User>()), Times.Never);
            mocker.Verify<IUserRepository>(r => r.SaveChangesAsync(), Times.Never);
            mocker.Verify<INotificador>(n => n.AddMensagem(UserValidationMessages.EmailAlreadyRegistered), Times.Once);

            #endregion
        }
    }
}

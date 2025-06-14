using AutenticacaoDoisFatores.Dominio.Compartilhados.Mensagens;
using AutenticacaoDoisFatores.Dominio.Entidades;
using AutenticacaoDoisFatores.Dominio.Repositorios;
using AutenticacaoDoisFatores.Servico.CasosDeUso.Usuarios;
using AutenticacaoDoisFatores.Testes.Compartilhados;
using Mensageiro;
using Moq;
using Moq.AutoMock;

namespace AutenticacaoDoisFatores.Testes.Servico.Usuarios
{
    public class ExcluirUsuarioTeste
    {
        [Fact]
        internal async Task DeveExcluirUsuario()
        {
            #region Preparação do teste

            var mocker = new AutoMocker();

            var idUsuario = Guid.NewGuid();
            var usuario = ConstrutorDeUsuariosTeste
                .RetornarConstrutor(id: idUsuario)
                .Build();

            var servico = mocker.CreateInstance<ExcluirUsuario>();
            mocker.GetMock<IUserRepository>().Setup(r => r.GetByIdAsync(idUsuario)).ReturnsAsync(usuario);

            #endregion

            await servico.ExecutarAsync(idUsuario);

            #region Verificação do teste

            mocker.Verify<IUserRepository>(r => r.Excluir(usuario), Times.Once);
            mocker.Verify<IUserRepository>(r => r.SaveChangesAsync(), Times.Once);

            #endregion
        }

        [Fact]
        internal async Task NaoDeveExcluirUsuarioInexistente()
        {
            #region Preparação do teste

            var mocker = new AutoMocker();

            var idUsuario = Guid.NewGuid();

            var servico = mocker.CreateInstance<ExcluirUsuario>();

            #endregion

            await servico.ExecutarAsync(idUsuario);

            #region Verificação do teste

            mocker.Verify<IUserRepository>(r => r.Remove(It.IsAny<User>()), Times.Never);
            mocker.Verify<IUserRepository>(r => r.SaveChangesAsync(), Times.Never);
            mocker.Verify<INotificador>(n => n.AddMensagemNaoEncontrado(UserValidationMessages.UserNotFound), Times.Once);

            #endregion
        }

        [Fact]
        internal async Task NaoDeveExcluirUsuarioAdmin()
        {
            #region Preparação do teste

            var mocker = new AutoMocker();

            var idUsuario = Guid.NewGuid();
            var usuario = ConstrutorDeUsuariosTeste
                .RetornarConstrutor(id: idUsuario, ehAdm: true)
                .Build();

            var servico = mocker.CreateInstance<ExcluirUsuario>();
            mocker.GetMock<IUserRepository>().Setup(r => r.GetByIdAsync(idUsuario)).ReturnsAsync(usuario);

            #endregion

            await servico.ExecutarAsync(idUsuario);

            #region Verificação do teste

            mocker.Verify<IUserRepository>(r => r.Remove(It.IsAny<User>()), Times.Never);
            mocker.Verify<IUserRepository>(r => r.SaveChangesAsync(), Times.Never);
            mocker.Verify<INotificador>(n => n.AddMensagemNaoEncontrado(UserValidationMessages.UserNotFound), Times.Once);

            #endregion
        }
    }
}

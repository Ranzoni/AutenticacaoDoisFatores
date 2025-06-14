using AutenticacaoDoisFatores.Dominio.Compartilhados.Mensagens;
using AutenticacaoDoisFatores.Dominio.Compartilhados.Permissoes;
using AutenticacaoDoisFatores.Dominio.Repositorios;
using AutenticacaoDoisFatores.Servico.CasosDeUso.Permissoes;
using AutenticacaoDoisFatores.Testes.Compartilhados;
using Mensageiro;
using Moq;
using Moq.AutoMock;

namespace AutenticacaoDoisFatores.Testes.Servico.Permissoes
{
    public class RemoverPermissoesParaUsuarioTeste
    {
        private readonly AutoMocker _mocker = new();

        [Fact]
        internal async Task DeveRemoverPermissoesParaUsuarioAtivo()
        {
            #region Preparação do teste

            var servico = _mocker.CreateInstance<RemoverPermissoesParaUsuario>();

            var idUsuario = Guid.NewGuid();
            var usuario = ConstrutorDeUsuariosTeste
                .RetornarConstrutor(ativo: true, id: idUsuario)
                .Build();
            var permissoesParaExcluir = new List<PermissionType>()
            {
                PermissionType.ActivateUser,
                PermissionType.InactivateUser
            };
            var permissoesInclusas = new List<PermissionType>
            {
                PermissionType.CreateUser,
                PermissionType.ActivateUser,
                PermissionType.InactivateUser
            };
            var permissoesEsperadas = permissoesInclusas.Except(permissoesParaExcluir);

            _mocker.GetMock<IUserRepository>().Setup(r => r.GetByIdAsync(idUsuario)).ReturnsAsync(usuario);
            _mocker.GetMock<IPermissionsRepository>().Setup(r => r.GetByUserIdAsync(idUsuario)).ReturnsAsync(permissoesInclusas);

            #endregion

            await servico.ExecutarAsync(idUsuario, permissoesParaExcluir);

            #region Verificação do teste

            _mocker.Verify<IPermissionsRepository>(r => r.UpdateAsync(idUsuario, permissoesEsperadas), Times.Once);

            #endregion
        }

        [Fact]
        internal async Task NaoDeveRemoverPermissoesParaUsuarioAdmin()
        {
            #region Preparação do teste

            var servico = _mocker.CreateInstance<RemoverPermissoesParaUsuario>();

            var idUsuario = Guid.NewGuid();
            var usuario = ConstrutorDeUsuariosTeste
                .RetornarConstrutor(ativo: true, ehAdm: true, id: idUsuario)
                .Build();
            var permissoesParaExcluir = new List<PermissionType>()
            {
                PermissionType.ActivateUser,
                PermissionType.InactivateUser
            };

            _mocker.GetMock<IUserRepository>().Setup(r => r.GetByIdAsync(idUsuario)).ReturnsAsync(usuario);

            #endregion

            await servico.ExecutarAsync(idUsuario, permissoesParaExcluir);

            #region Verificação do teste

            _mocker.Verify<IPermissionsRepository>(r => r.UpdateAsync(It.IsAny<Guid>(), It.IsAny<IEnumerable<PermissionType>>()), Times.Never);
            _mocker.Verify<INotificador>(n => n.AddMensagemNaoEncontrado(UserValidationMessages.UserNotFound), Times.Once);

            #endregion
        }

        [Fact]
        internal async Task NaoDeveRemoverPermissoesParaUsuarioInativo()
        {
            #region Preparação do teste

            var servico = _mocker.CreateInstance<RemoverPermissoesParaUsuario>();

            var idUsuario = Guid.NewGuid();
            var usuario = ConstrutorDeUsuariosTeste
                .RetornarConstrutor(ativo: false, id: idUsuario)
                .Build();
            var permissoesParaExcluir = new List<PermissionType>()
            {
                PermissionType.ActivateUser,
                PermissionType.InactivateUser
            };

            _mocker.GetMock<IUserRepository>().Setup(r => r.GetByIdAsync(idUsuario)).ReturnsAsync(usuario);

            #endregion

            await servico.ExecutarAsync(idUsuario, permissoesParaExcluir);

            #region Verificação do teste

            _mocker.Verify<IPermissionsRepository>(r => r.UpdateAsync(It.IsAny<Guid>(), It.IsAny<IEnumerable<PermissionType>>()), Times.Never);
            _mocker.Verify<INotificador>(n => n.AddMensagemNaoEncontrado(UserValidationMessages.UserNotFound), Times.Once);

            #endregion
        }

        [Fact]
        internal async Task NaoDeveRemoverPermissoesParaUsuarioNaoExistente()
        {
            #region Preparação do teste

            var servico = _mocker.CreateInstance<RemoverPermissoesParaUsuario>();

            var idUsuario = Guid.NewGuid();
            var permissoesParaExcluir = new List<PermissionType>()
            {
                PermissionType.ActivateUser,
                PermissionType.InactivateUser
            };

            #endregion

            await servico.ExecutarAsync(idUsuario, permissoesParaExcluir);

            #region Verificação do teste

            _mocker.Verify<IPermissionsRepository>(r => r.UpdateAsync(It.IsAny<Guid>(), It.IsAny<IEnumerable<PermissionType>>()), Times.Never);
            _mocker.Verify<INotificador>(n => n.AddMensagemNaoEncontrado(UserValidationMessages.UserNotFound), Times.Once);

            #endregion
        }
    }
}

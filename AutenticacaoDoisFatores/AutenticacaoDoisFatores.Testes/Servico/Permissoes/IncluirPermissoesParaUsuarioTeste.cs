using AutenticacaoDoisFatores.Dominio.Compartilhados.Mensagens;
using AutenticacaoDoisFatores.Dominio.Compartilhados.Permissoes;
using AutenticacaoDoisFatores.Dominio.Repositorios;
using AutenticacaoDoisFatores.Servico.CasosDeUso.Permissoes;
using AutenticacaoDoisFatores.Testes.Compartilhados;
using Bogus;
using Mensageiro;
using Moq;
using Moq.AutoMock;

namespace AutenticacaoDoisFatores.Testes.Servico.Permissoes
{
    public class IncluirPermissoesParaUsuarioTeste
    {
        private readonly AutoMocker _mocker = new();
        private readonly Faker _faker = new();

        [Fact]
        internal async Task DeveIncluirPermissoes()
        {
            #region Preparação do teste

            var idUsuario = Guid.NewGuid();
            var permissoesParaIncluir = _faker.Random.EnumValues<PermissionType>();

            var usuario = ConstrutorDeUsuariosTeste
                .RetornarConstrutor(id: idUsuario, ativo: true)
                .Build();

            var servico = _mocker.CreateInstance<AddUserPermission>();

            _mocker.GetMock<IUserRepository>().Setup(r => r.GetByIdAsync(idUsuario)).ReturnsAsync(usuario);

            #endregion

            await servico.ExecutarAsync(idUsuario, permissoesParaIncluir);

            _mocker.Verify<IPermissionsRepository>(r => r.AddAsync(idUsuario, permissoesParaIncluir), Times.Once);
        }

        [Fact]
        internal async Task NaoDeveIncluirPermissoesQuandoUsuarioNaoExiste()
        {
            #region Preparação do teste

            var idUsuario = Guid.NewGuid();
            var permissoesParaIncluir = _faker.Random.EnumValues<PermissionType>();

            var servico = _mocker.CreateInstance<AddUserPermission>();

            #endregion

            await servico.ExecutarAsync(idUsuario, permissoesParaIncluir);

            #region Verificação do teste

            _mocker.Verify<IPermissionsRepository>(r => r.AddAsync(It.IsAny<Guid>(), It.IsAny<IEnumerable<PermissionType>>()), Times.Never);
            _mocker.Verify<IPermissionsRepository>(r => r.UpdateAsync(It.IsAny<Guid>(), It.IsAny<IEnumerable<PermissionType>>()), Times.Never);
            _mocker.Verify<INotificador>(n => n.AddMensagemNaoEncontrado(UserValidationMessages.UserNotFound), Times.Once);

            #endregion
        }

        [Fact]
        internal async Task NaoDeveIncluirPermissoesQuandoUsuarioInativo()
        {
            #region Preparação do teste

            var idUsuario = Guid.NewGuid();
            var permissoesParaIncluir = _faker.Random.EnumValues<PermissionType>();

            var usuario = ConstrutorDeUsuariosTeste
                .RetornarConstrutor(id: idUsuario, ativo: false)
                .Build();

            var servico = _mocker.CreateInstance<AddUserPermission>();

            _mocker.GetMock<IUserRepository>().Setup(r => r.GetByIdAsync(idUsuario)).ReturnsAsync(usuario);

            #endregion

            await servico.ExecutarAsync(idUsuario, permissoesParaIncluir);

            #region Verificação do teste

            _mocker.Verify<IPermissionsRepository>(r => r.AddAsync(It.IsAny<Guid>(), It.IsAny<IEnumerable<PermissionType>>()), Times.Never);
            _mocker.Verify<IPermissionsRepository>(r => r.UpdateAsync(It.IsAny<Guid>(), It.IsAny<IEnumerable<PermissionType>>()), Times.Never);
            _mocker.Verify<INotificador>(n => n.AddMensagemNaoEncontrado(UserValidationMessages.UserNotFound), Times.Once);

            #endregion
        }

        [Fact]
        internal async Task NaoDeveIncluirPermissoesQuandoUsuarioEhAdm()
        {
            #region Preparação do teste

            var idUsuario = Guid.NewGuid();
            var permissoesParaIncluir = _faker.Random.EnumValues<PermissionType>();

            var usuario = ConstrutorDeUsuariosTeste
                .RetornarConstrutor(id: idUsuario, ativo: true, ehAdm: true)
                .Build();

            var servico = _mocker.CreateInstance<AddUserPermission>();

            _mocker.GetMock<IUserRepository>().Setup(r => r.GetByIdAsync(idUsuario)).ReturnsAsync(usuario);

            #endregion

            await servico.ExecutarAsync(idUsuario, permissoesParaIncluir);

            #region Verificação do teste

            _mocker.Verify<IPermissionsRepository>(r => r.AddAsync(It.IsAny<Guid>(), It.IsAny<IEnumerable<PermissionType>>()), Times.Never);
            _mocker.Verify<INotificador>(n => n.AddMensagemNaoEncontrado(UserValidationMessages.UserNotFound), Times.Once);

            #endregion
        }
    }
}

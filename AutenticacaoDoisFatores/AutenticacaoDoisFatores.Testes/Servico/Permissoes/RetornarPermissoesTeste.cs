using AutenticacaoDoisFatores.Dominio.Compartilhados;
using AutenticacaoDoisFatores.Dominio.Compartilhados.Permissoes;
using AutenticacaoDoisFatores.Dominio.Repositorios;
using AutenticacaoDoisFatores.Servico.CasosDeUso.Permissoes;
using AutenticacaoDoisFatores.Servico.DTO.Permissoes;
using Bogus;
using Moq;
using Moq.AutoMock;

namespace AutenticacaoDoisFatores.Testes.Servico.Permissoes
{
    public class RetornarPermissoesTeste
    {
        [Fact]
        internal void DeveRetornarTodasPermissoes()
        {
            #region Preparação do teste

            var permissoesEsperadas = Enum.GetValues<PermissionType>()
                .Select(tipo => new PermissaoDisponivel(tipo.Description() ?? "", tipo));

            #endregion

            var retorno = GetPermissions.GetAll();

            #region Verificação do teste

            Assert.Equal(permissoesEsperadas.Count(), retorno.Count());
            for (var i = 0; i <= permissoesEsperadas.Count() - 1; i++)
            {
                var permissaoEsperada = permissoesEsperadas.ToArray()[i];
                var itemDoRetorno = retorno.ToArray()[i];

                Assert.Equal(permissaoEsperada.Nome, itemDoRetorno.Name);
                Assert.Equal(permissaoEsperada.Valor, itemDoRetorno.Value);
            }

            #endregion
        }

        [Fact]
        internal async Task DeveRetornarPermissoesPorUsuario()
        {
            #region Preparação do teste

            var mocker = new AutoMocker();
            var faker = new Faker();

            var idUsuario = Guid.NewGuid();
            var permissoesUsuario = faker.Random.EnumValues<PermissionType>(3);
            var permissoesEsperadas = permissoesUsuario
                .Select(tipo => new PermissaoDisponivel(tipo.Description() ?? "", tipo));

            var servico = mocker.CreateInstance<GetPermissions>();

            mocker.GetMock<IPermissionsRepository>().Setup(r => r.GetByUserIdAsync(idUsuario)).ReturnsAsync(permissoesUsuario);

            #endregion

            var retorno = await servico.GetByUserIdAsync(idUsuario);

            #region Verificação do teste

            Assert.NotEmpty(retorno);
            Assert.Equal(permissoesEsperadas.Count(), retorno.Count());
            for (var i = 0; i <= permissoesEsperadas.Count() - 1; i++)
            {
                var permissaoEsperada = permissoesEsperadas.ToArray()[i];
                var itemDoRetorno = retorno.ToArray()[i];

                Assert.Equal(permissaoEsperada.Nome, itemDoRetorno.Name);
                Assert.Equal(permissaoEsperada.Valor, itemDoRetorno.Value);
            }

            #endregion
        }

        [Fact]
        internal async Task DeveRetornarTodasPermissoesParaUsuarioAdm()
        {
            #region Preparação do teste

            var mocker = new AutoMocker();

            var idUsuario = Guid.NewGuid();
            var permissoesEsperadas = Enum.GetValues<PermissionType>()
                .Select(tipo => new PermissaoDisponivel(tipo.Description() ?? "", tipo));

            var servico = mocker.CreateInstance<GetPermissions>();

            mocker.GetMock<IUserRepository>().Setup(r => r.IsAdminAsync(idUsuario)).ReturnsAsync(true);

            #endregion

            var retorno = await servico.GetByUserIdAsync(idUsuario);

            #region Verificação do teste

            Assert.NotEmpty(retorno);
            Assert.Equal(permissoesEsperadas.Count(), retorno.Count());
            for (var i = 0; i <= permissoesEsperadas.Count() - 1; i++)
            {
                var permissaoEsperada = permissoesEsperadas.ToArray()[i];
                var itemDoRetorno = retorno.ToArray()[i];

                Assert.Equal(permissaoEsperada.Nome, itemDoRetorno.Name);
                Assert.Equal(permissaoEsperada.Valor, itemDoRetorno.Value);
            }

            #endregion
        }
    }
}

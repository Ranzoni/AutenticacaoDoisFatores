using AutenticacaoDoisFatores.Dominio.Compartilhados.Usuarios;
using AutenticacaoDoisFatores.Dominio.Repositorios;
using AutenticacaoDoisFatores.Dominio.Servicos;
using AutenticacaoDoisFatores.Servico.CasosDeUso.Usuarios.Autenticadores;
using AutenticacaoDoisFatores.Servico.CasosDeUso.Usuarios.Autenticadores.AutenticacoesDoisFatores;
using AutenticacaoDoisFatores.Servico.DTO.Usuarios;
using AutenticacaoDoisFatores.Testes.Compartilhados;
using Moq;
using Moq.AutoMock;

namespace AutenticacaoDoisFatores.Testes.Servico.Usuarios
{
    public class EnviarCodigoAutenticacaoUsuarioTeste
    {
        [Fact]
        internal async Task DeveEnviarCodPorEmail()
        {
            #region Preparação do teste

            var mocker = new AutoMocker();

            var servico = mocker.CreateInstance<UserTwoFactorAuthentication>();

            var enviarCodAutenticacaoUsuarioPorEmail= mocker.CreateInstance<UserTwoFactorAuthByEmail>();
            mocker.GetMock<IServiceProvider>().Setup(s => s.GetService(typeof(UserTwoFactorAuthByEmail))).Returns(enviarCodAutenticacaoUsuarioPorEmail);

            var usuario = ConstrutorDeUsuariosTeste
                .RetornarConstrutor(ativo: true, tipoDeAutenticacao: AuthType.Email)
                .Build();

            #endregion

            var retorno =  await servico.ExecutarAsync(usuario);

            #region Verificação do teste

            Assert.NotNull(retorno);
            Assert.NotEmpty((retorno as TwoFactorAuthResponse)!.Token);
            mocker.Verify<IAuthCodeRepository>(r => r.SalvarAsync(usuario.Id, It.IsAny<string>()), Times.Once);

            #endregion
        }
    }
}

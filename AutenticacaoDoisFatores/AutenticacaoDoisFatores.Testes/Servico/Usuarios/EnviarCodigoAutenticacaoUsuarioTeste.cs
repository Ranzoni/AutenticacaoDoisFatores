using AutenticacaoDoisFatores.Dominio.Compartilhados.Usuarios;
using AutenticacaoDoisFatores.Dominio.Repositorios;
using AutenticacaoDoisFatores.Servico.CasosDeUso.Usuarios.AutenticacoesDoisFatores;
using AutenticacaoDoisFatores.Servico.CasosDeUso.Usuarios.Autenticadores;
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

            var servico = mocker.CreateInstance<EnviarCodigoAutenticacaoUsuario>();

            var enviarCodAutenticacaoUsuarioPorEmail= mocker.CreateInstance<EnviarCodAutenticacaoUsuarioPorEmail>();
            mocker.GetMock<IServiceProvider>().Setup(s => s.GetService(typeof(EnviarCodAutenticacaoUsuarioPorEmail))).Returns(enviarCodAutenticacaoUsuarioPorEmail);

            var usuario = ConstrutorDeUsuariosTeste
                .RetornarConstrutor(ativo: true, tipoDeAutenticacao: TipoDeAutenticacao.Email)
                .ConstruirCadastrado();

            #endregion

            var retorno =  await servico.ExecutarAsync(usuario);

            #region Verificação do teste

            Assert.NotNull(retorno);
            Assert.NotEmpty((retorno as RespostaAutenticacaoDoisFatores)!.Token);
            mocker.Verify<IRepositorioDeCodigoDeAutenticacao>(r => r.SalvarAsync(usuario.Id, It.IsAny<string>()), Times.Once);

            #endregion
        }

        [Fact]
        internal async Task DeveEnviarCodPorSms()
        {
            #region Preparação do teste

            var mocker = new AutoMocker();

            var servico = mocker.CreateInstance<EnviarCodigoAutenticacaoUsuario>();

            var enviarCodAutenticacaoUsuarioPorSms = mocker.CreateInstance<EnviarCodAutenticacaoUsuarioPorSms>();
            mocker.GetMock<IServiceProvider>().Setup(s => s.GetService(typeof(EnviarCodAutenticacaoUsuarioPorSms))).Returns(enviarCodAutenticacaoUsuarioPorSms);

            var usuario = ConstrutorDeUsuariosTeste
                .RetornarConstrutor(ativo: true, tipoDeAutenticacao: TipoDeAutenticacao.SMS)
                .ConstruirCadastrado();

            #endregion

            var excecao = await Assert.ThrowsAsync<NotImplementedException>(() => servico.ExecutarAsync(usuario));

            #region Verificação do teste


            #endregion
        }
    }
}

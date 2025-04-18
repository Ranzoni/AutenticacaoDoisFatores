using AutenticacaoDoisFatores.Dominio.Compartilhados.Usuarios;
using AutenticacaoDoisFatores.Servico.CasosDeUso.Usuarios.AutenticacoesDoisFatores;
using AutenticacaoDoisFatores.Testes.Compartilhados;
using Moq.AutoMock;

namespace AutenticacaoDoisFatores.Testes.Servico.Usuarios
{
    public class EnviarCodAutenticacaoUsuarioPorSmsTeste
    {
        [Fact]
        internal async Task DeveEnviarCodAutenticacao()
        {
            #region Preparação do teste

            var mocker = new AutoMocker();

            var servico = mocker.CreateInstance<EnviarCodAutenticacaoUsuarioPorSms>();

            var usuario = ConstrutorDeUsuariosTeste
                .RetornarConstrutor(ativo: true, tipoDeAutenticacao: TipoDeAutenticacao.SMS)
                .ConstruirCadastrado();

            #endregion

            var excecao = await Assert.ThrowsAsync<NotImplementedException>(() => servico.EnviarAsync(usuario));

            #region Verificação do teste

            #endregion
        }
    }
}

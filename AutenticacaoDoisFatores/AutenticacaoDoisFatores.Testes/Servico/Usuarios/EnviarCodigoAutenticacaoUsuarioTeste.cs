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

            var servico = mocker.CreateInstance<AutenticadorUsuarioEmDoisFatores>();

            var enviarCodAutenticacaoUsuarioPorEmail= mocker.CreateInstance<AutenticadorUsuarioEmDoisFatoresPorEmail>();
            mocker.GetMock<IServiceProvider>().Setup(s => s.GetService(typeof(AutenticadorUsuarioEmDoisFatoresPorEmail))).Returns(enviarCodAutenticacaoUsuarioPorEmail);

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
    }
}

using AutenticacaoDoisFatores.Dominio.Compartilhados.Mensagens;
using AutenticacaoDoisFatores.Dominio.Entidades;
using AutenticacaoDoisFatores.Dominio.Repositorios;
using AutenticacaoDoisFatores.Servico.CasosDeUso.Usuarios.Autenticadores;
using AutenticacaoDoisFatores.Servico.DTO.Usuarios;
using AutenticacaoDoisFatores.Testes.Compartilhados;
using Mensageiro;
using Moq;
using Moq.AutoMock;

namespace AutenticacaoDoisFatores.Testes.Servico.Usuarios
{
    public class RetornarUsuarioAutenticadoTeste
    {
        [Fact]
        internal async Task DeveRetornarUsuarioAutenticado()
        {
            #region Preparação do teste

            var mocker = new AutoMocker();

            var servico = mocker.CreateInstance<RetornarUsuarioAutenticado>();

            var usuario = ConstrutorDeUsuariosTeste
                .RetornarConstrutor(ativo: true)
                .ConstruirCadastrado();

            #endregion

            var resposta = await servico.ExecutarAsync(usuario);

            #region Verificação do teste

            Assert.NotNull(resposta);
            Assert.IsType<UsuarioAutenticado>(resposta);
            Assert.NotEmpty((resposta as UsuarioAutenticado)!.Token);
            mocker.Verify<IRepositorioDeUsuarios>(r => r.Editar(usuario), Times.Once);
            mocker.Verify<IRepositorioDeUsuarios>(r => r.SalvarAlteracoesAsync(), Times.Once);

            #endregion
        }

        [Fact]
        internal async Task NaoDeveRetornarUsuarioAutenticadoQuandoInativo()
        {
            #region Preparação do teste

            var mocker = new AutoMocker();

            var servico = mocker.CreateInstance<RetornarUsuarioAutenticado>();

            var usuario = ConstrutorDeUsuariosTeste
                .RetornarConstrutor(ativo: false)
                .ConstruirCadastrado();

            #endregion

            var resposta = await servico.ExecutarAsync(usuario);

            #region Verificação do teste

            Assert.Null(resposta);
            mocker.Verify<IRepositorioDeUsuarios>(r => r.Editar(It.IsAny<Usuario>()), Times.Never);
            mocker.Verify<IRepositorioDeUsuarios>(r => r.SalvarAlteracoesAsync(), Times.Never);
            mocker.Verify<INotificador>(n => n.AddMensagemNaoEncontrado(MensagensValidacaoUsuario.UsuarioNaoEncontrado), Times.Once);

            #endregion
        }
    }
}

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
    public class GerarNovaSenhaUsuarioTeste
    {
        private readonly AutoMocker _mocker = new();

        [Fact]
        internal async Task DeveAlterarSenha()
        {
            #region Preparação do teste

            var idUsuario = Guid.NewGuid();
            var senhaAtual = "Senh4#Atual!!";
            var novaSenha = "Teste.De_N0v@!!Senha";

            var usuario = ConstrutorDeUsuariosTeste
                .RetornarConstrutor(id: idUsuario, senha: senhaAtual, ativo: true)
                .ConstruirCadastrado();

            var servico = _mocker.CreateInstance<GerarNovaSenhaUsuario>();

            _mocker.GetMock<IRepositorioDeUsuarios>().Setup(r => r.BuscarUnicoAsync(idUsuario)).ReturnsAsync(usuario);

            #endregion

            await servico.ExecutarAsync(idUsuario, novaSenha);

            #region Verificação de senha

            Assert.NotEqual(senhaAtual, usuario.Senha);
            Assert.NotNull(usuario.DataAlteracao);
            _mocker.Verify<IRepositorioDeUsuarios>(r => r.Editar(usuario), Times.Once);
            _mocker.Verify<IRepositorioDeUsuarios>(r => r.SalvarAlteracoesAsync(), Times.Once);

            #endregion
        }

        [Theory]
        [InlineData("")]
        [InlineData("       ")]
        [InlineData("T&st.1")]
        [InlineData("Teste.de.senh@.que.é.muito.grand3.Teste.de.senh@.qu")]
        [InlineData("testedesenha.123")]
        [InlineData("@Senha.para_teste")]
        [InlineData("TESTEDESENHA.123")]
        [InlineData("2senhaInvalida")]
        internal async Task NaoDeveAlterarSenhaQuandoEhInvalida(string senhaInvalida)
        {
            #region Preparação do teste

            var idUsuario = Guid.NewGuid();

            var servico = _mocker.CreateInstance<GerarNovaSenhaUsuario>();

            #endregion

            await servico.ExecutarAsync(idUsuario, senhaInvalida);

            #region Verificação de senha

            _mocker.Verify<IRepositorioDeUsuarios>(r => r.Editar(It.IsAny<Usuario>()), Times.Never);
            _mocker.Verify<IRepositorioDeUsuarios>(r => r.SalvarAlteracoesAsync(), Times.Never);
            _mocker.Verify<INotificador>(n => n.AddMensagem(MensagensValidacaoUsuario.SenhaInvalida), Times.Once);

            #endregion
        }

        [Fact]
        internal async Task NaoDeveAlterarSenhaQuandoUsuarioNaoExiste()
        {
            #region Preparação do teste

            var idUsuario = Guid.NewGuid();
            var novaSenha = "Teste.De_N0v@!!Senha";

            var servico = _mocker.CreateInstance<GerarNovaSenhaUsuario>();

            #endregion

            await servico.ExecutarAsync(idUsuario, novaSenha);

            #region Verificação de senha

            _mocker.Verify<IRepositorioDeUsuarios>(r => r.Editar(It.IsAny<Usuario>()), Times.Never);
            _mocker.Verify<IRepositorioDeUsuarios>(r => r.SalvarAlteracoesAsync(), Times.Never);
            _mocker.Verify<INotificador>(n => n.AddMensagemNaoEncontrado(MensagensValidacaoUsuario.UsuarioNaoEncontrado), Times.Once);

            #endregion
        }

        [Fact]
        internal async Task NaoDeveAlterarSenhaQuandoUsuarioInativo()
        {
            #region Preparação do teste

            var idUsuario = Guid.NewGuid();
            var senhaAtual = "Senh4#Atual!!";
            var novaSenha = "Teste.De_N0v@!!Senha";

            var usuario = ConstrutorDeUsuariosTeste
                .RetornarConstrutor(id: idUsuario, senha: senhaAtual, ativo: false)
                .ConstruirCadastrado();

            var servico = _mocker.CreateInstance<GerarNovaSenhaUsuario>();

            _mocker.GetMock<IRepositorioDeUsuarios>().Setup(r => r.BuscarUnicoAsync(idUsuario)).ReturnsAsync(usuario);

            #endregion

            await servico.ExecutarAsync(idUsuario, novaSenha);

            #region Verificação de senha

            _mocker.Verify<IRepositorioDeUsuarios>(r => r.Editar(It.IsAny<Usuario>()), Times.Never);
            _mocker.Verify<IRepositorioDeUsuarios>(r => r.SalvarAlteracoesAsync(), Times.Never);
            _mocker.Verify<INotificador>(n => n.AddMensagemNaoEncontrado(MensagensValidacaoUsuario.UsuarioNaoEncontrado), Times.Once);

            #endregion
        }

        [Fact]
        internal async Task NaoDeveAlterarSenhaQuandoUsuarioEhAdm()
        {
            #region Preparação do teste

            var idUsuario = Guid.NewGuid();
            var senhaAtual = "Senh4#Atual!!";
            var novaSenha = "Teste.De_N0v@!!Senha";

            var usuario = ConstrutorDeUsuariosTeste
                .RetornarConstrutor(id: idUsuario, senha: senhaAtual, ativo: true, ehAdm: true)
                .ConstruirCadastrado();

            var servico = _mocker.CreateInstance<GerarNovaSenhaUsuario>();

            _mocker.GetMock<IRepositorioDeUsuarios>().Setup(r => r.BuscarUnicoAsync(idUsuario)).ReturnsAsync(usuario);

            #endregion

            await servico.ExecutarAsync(idUsuario, novaSenha);

            #region Verificação de senha

            _mocker.Verify<IRepositorioDeUsuarios>(r => r.Editar(It.IsAny<Usuario>()), Times.Never);
            _mocker.Verify<IRepositorioDeUsuarios>(r => r.SalvarAlteracoesAsync(), Times.Never);
            _mocker.Verify<INotificador>(n => n.AddMensagemNaoEncontrado(MensagensValidacaoUsuario.UsuarioNaoEncontrado), Times.Once);

            #endregion
        }
    }
}

using AutenticacaoDoisFatores.Dominio.Compartilhados.Mensagens;
using AutenticacaoDoisFatores.Dominio.Compartilhados.Usuarios;
using AutenticacaoDoisFatores.Dominio.Dominios;
using AutenticacaoDoisFatores.Dominio.Repositorios;
using AutenticacaoDoisFatores.Dominio.Servicos;
using AutenticacaoDoisFatores.Servico.CasosDeUso.Usuarios;
using AutenticacaoDoisFatores.Servico.Compartilhados;
using AutenticacaoDoisFatores.Testes.Compartilhados;
using Mensageiro;
using Moq;
using Moq.AutoMock;

namespace AutenticacaoDoisFatores.Testes.Servico.Usuarios
{
    public class AutenticarUsuarioEmDoisFatoresTeste
    {
        [Fact]
        internal async Task DeveAutenticar()
        {
            #region Preparação do teste

            var mocker = new AutoMocker();

            var servico = mocker.CreateInstance<AutenticarUsuarioEmDoisFatores>();

            var idUsuario = Guid.NewGuid();
            var codigo = Seguranca.GerarCodigoAutenticacao();
            var codigoCriptografado = Criptografia.CriptografarComSha512(codigo);

            var usuario = ConstrutorDeUsuariosTeste
                .RetornarConstrutor(ativo: true, id: idUsuario)
                .ConstruirCadastrado();

            mocker.GetMock<IRepositorioDeUsuarios>().Setup(r => r.BuscarUnicoAsync(idUsuario)).ReturnsAsync(usuario);
            mocker.GetMock<IRepositorioDeCodigoDeAutenticacao>().Setup(r => r.BuscarCodigoAsync(idUsuario)).ReturnsAsync(codigoCriptografado);

            #endregion

            var resposta = await servico.ExecutarAsync(idUsuario, codigo);

            #region Verificação do teste

            Assert.NotNull(resposta);
            Assert.NotEmpty(resposta.Token);
            Assert.NotNull(usuario.DataUltimoAcesso);

            #endregion
        }

        [Fact]
        internal async Task DeveAutenticarComApp()
        {
            #region Preparação do teste

            var mocker = new AutoMocker();

            var servico = mocker.CreateInstance<AutenticarUsuarioEmDoisFatores>();

            var idUsuario = Guid.NewGuid();
            var codigo = Seguranca.GerarCodigoAutenticacao();
            var codigoCriptografado = Criptografia.CriptografarComSha512(codigo);

            var usuario = ConstrutorDeUsuariosTeste
                .RetornarConstrutor(ativo: true, id: idUsuario, tipoDeAutenticacao: TipoDeAutenticacao.AppAutenticador)
                .ConstruirCadastrado();

            mocker.GetMock<IRepositorioDeUsuarios>().Setup(r => r.BuscarUnicoAsync(idUsuario)).ReturnsAsync(usuario);
            mocker.GetMock<IServicoDeAutenticador>().Setup(s => s.CodigoEhValido(codigo)).Returns(true);

            #endregion

            var resposta = await servico.ExecutarAsync(idUsuario, codigo);

            #region Verificação do teste

            Assert.NotNull(resposta);
            Assert.NotEmpty(resposta.Token);
            Assert.NotNull(usuario.DataUltimoAcesso);

            #endregion
        }

        [Fact]
        internal async Task NaoDeveAutenticarQuandoUsuarioNaoExiste()
        {
            #region Preparação do teste

            var mocker = new AutoMocker();

            var servico = mocker.CreateInstance<AutenticarUsuarioEmDoisFatores>();

            var idUsuario = Guid.NewGuid();
            var codigo = Seguranca.GerarCodigoAutenticacao();

            #endregion

            var resposta = await servico.ExecutarAsync(idUsuario, codigo);

            #region Verificação do teste

            Assert.Null(resposta);
            mocker.Verify<INotificador>(n => n.AddMensagemNaoAutorizado(MensagensValidacaoUsuario.UsuarioNaoEncontrado));

            #endregion
        }

        [Fact]
        internal async Task NaoDeveAutenticarQuandoUsuarioEstaInativo()
        {
            #region Preparação do teste

            var mocker = new AutoMocker();

            var servico = mocker.CreateInstance<AutenticarUsuarioEmDoisFatores>();

            var idUsuario = Guid.NewGuid();
            var codigo = Seguranca.GerarCodigoAutenticacao();
            var codigoCriptografado = Criptografia.CriptografarComSha512(codigo);

            var usuario = ConstrutorDeUsuariosTeste
                .RetornarConstrutor(ativo: false, id: idUsuario)
                .ConstruirCadastrado();

            mocker.GetMock<IRepositorioDeUsuarios>().Setup(r => r.BuscarUnicoAsync(idUsuario)).ReturnsAsync(usuario);
            mocker.GetMock<IRepositorioDeCodigoDeAutenticacao>().Setup(r => r.BuscarCodigoAsync(idUsuario)).ReturnsAsync(codigoCriptografado);

            #endregion

            var resposta = await servico.ExecutarAsync(idUsuario, codigo);

            #region Verificação do teste

            Assert.Null(resposta);
            mocker.Verify<INotificador>(n => n.AddMensagemNaoAutorizado(MensagensValidacaoUsuario.UsuarioNaoEncontrado));

            #endregion
        }

        [Fact]
        internal async Task NaoDeveAutenticarQuandoCodigoNaoExistir()
        {
            #region Preparação do teste

            var mocker = new AutoMocker();

            var servico = mocker.CreateInstance<AutenticarUsuarioEmDoisFatores>();

            var idUsuario = Guid.NewGuid();
            var codigo = Seguranca.GerarCodigoAutenticacao();

            var usuario = ConstrutorDeUsuariosTeste
                .RetornarConstrutor(ativo: true, id: idUsuario)
                .ConstruirCadastrado();

            mocker.GetMock<IRepositorioDeUsuarios>().Setup(r => r.BuscarUnicoAsync(idUsuario)).ReturnsAsync(usuario);

            #endregion

            var resposta = await servico.ExecutarAsync(idUsuario, codigo);

            #region Verificação do teste

            Assert.Null(resposta);
            mocker.Verify<INotificador>(n => n.AddMensagemNaoAutorizado(MensagensValidacaoUsuario.NaoAutenticado));

            #endregion
        }

        [Fact]
        internal async Task NaoDeveAutenticarQuandoCodigoIncorreto()
        {
            #region Preparação do teste

            var mocker = new AutoMocker();

            var servico = mocker.CreateInstance<AutenticarUsuarioEmDoisFatores>();

            var idUsuario = Guid.NewGuid();
            var codigo = Seguranca.GerarCodigoAutenticacao();
            var codigoCriptografado = Criptografia.CriptografarComSha512(codigo);

            var usuario = ConstrutorDeUsuariosTeste
                .RetornarConstrutor(ativo: true, id: idUsuario)
                .ConstruirCadastrado();

            mocker.GetMock<IRepositorioDeUsuarios>().Setup(r => r.BuscarUnicoAsync(idUsuario)).ReturnsAsync(usuario);
            mocker.GetMock<IRepositorioDeCodigoDeAutenticacao>().Setup(r => r.BuscarCodigoAsync(idUsuario)).ReturnsAsync(codigoCriptografado);

            #endregion

            var resposta = await servico.ExecutarAsync(idUsuario, codigo + "1");

            #region Verificação do teste

            Assert.Null(resposta);
            mocker.Verify<INotificador>(n => n.AddMensagemNaoAutorizado(MensagensValidacaoUsuario.NaoAutenticado));

            #endregion
        }

        [Fact]
        internal async Task NaoDeveAutenticarPorAppQuandoCodigoIncorreto()
        {
            #region Preparação do teste

            var mocker = new AutoMocker();

            var servico = mocker.CreateInstance<AutenticarUsuarioEmDoisFatores>();

            var idUsuario = Guid.NewGuid();
            var codigo = Seguranca.GerarCodigoAutenticacao();
            var codigoCriptografado = Criptografia.CriptografarComSha512(codigo);

            var usuario = ConstrutorDeUsuariosTeste
                .RetornarConstrutor(ativo: true, id: idUsuario, tipoDeAutenticacao: TipoDeAutenticacao.AppAutenticador)
                .ConstruirCadastrado();

            mocker.GetMock<IRepositorioDeUsuarios>().Setup(r => r.BuscarUnicoAsync(idUsuario)).ReturnsAsync(usuario);
            mocker.GetMock<IServicoDeAutenticador>().Setup(s => s.CodigoEhValido(codigo)).Returns(true);

            #endregion

            var resposta = await servico.ExecutarAsync(idUsuario, codigo + "1");

            #region Verificação do teste

            Assert.Null(resposta);
            mocker.Verify<INotificador>(n => n.AddMensagemNaoAutorizado(MensagensValidacaoUsuario.NaoAutenticado));

            #endregion
        }
    }
}

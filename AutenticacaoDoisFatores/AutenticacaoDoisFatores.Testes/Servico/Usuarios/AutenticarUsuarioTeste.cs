using AutenticacaoDoisFatores.Dominio.Compartilhados;
using AutenticacaoDoisFatores.Dominio.Compartilhados.Mensagens;
using AutenticacaoDoisFatores.Dominio.Compartilhados.Usuarios;
using AutenticacaoDoisFatores.Dominio.Dominios;
using AutenticacaoDoisFatores.Dominio.Repositorios;
using AutenticacaoDoisFatores.Servico.CasosDeUso.Usuarios;
using AutenticacaoDoisFatores.Servico.CasosDeUso.Usuarios.AutenticacoesDoisFatores;
using AutenticacaoDoisFatores.Servico.CasosDeUso.Usuarios.Autenticadores;
using AutenticacaoDoisFatores.Servico.DTO.Usuarios;
using AutenticacaoDoisFatores.Testes.Compartilhados;
using Bogus;
using Mensageiro;
using Moq;
using Moq.AutoMock;

namespace AutenticacaoDoisFatores.Testes.Servico.Usuarios
{
    public class AutenticarUsuarioTeste
    {
        private readonly AutoMocker _mocker = new();
        private readonly Faker _faker = new();

        [Fact]
        internal async Task DeveAutenticarComNomeUsuario()
        {
            #region Preparação do teste

            var senha = _faker.Random.AlphaNumeric(20);
            var senhaCriptografada = Criptografia.CriptografarComSha512(senha);

            var usuarioParaTeste = ConstrutorDeUsuariosTeste
                .RetornarConstrutor(ativo: true, senha: senhaCriptografada)
                .ConstruirCadastrado();

            var servico = _mocker.CreateInstance<AutenticarUsuario>();
            var retornarUsuarioAutenticado = _mocker.CreateInstance<RetornarUsuarioAutenticado>();
            _mocker.GetMock<IRepositorioDeUsuarios>().Setup(r => r.BuscarPorNomeUsuarioAsync(usuarioParaTeste.NomeUsuario)).ReturnsAsync(usuarioParaTeste);
            _mocker.GetMock<IServiceProvider>().Setup(s => s.GetService(typeof(RetornarUsuarioAutenticado))).Returns(retornarUsuarioAutenticado);

            var dadosAutenticacao = new DadosAutenticacao(
                nomeUsuario: usuarioParaTeste.NomeUsuario,
                email: null,
                senha: senha);

            #endregion

            var resposta = await servico.ExecutarAsync(dadosAutenticacao);

            #region Verificação do teste

            var usuarioResposta = (UsuarioAutenticado?)resposta;
            Assert.NotNull(usuarioResposta);
            Assert.Equal(usuarioParaTeste.Nome, usuarioResposta.Usuario.Nome);
            Assert.Equal(usuarioParaTeste.NomeUsuario, usuarioResposta.Usuario.NomeUsuario);
            Assert.Equal(usuarioParaTeste.Email, usuarioResposta.Usuario.Email);
            Assert.False(usuarioResposta.Token.EstaVazio());

            #endregion
        }

        [Fact]
        internal async Task DeveAutenticarComEmail()
        {
            #region Preparação do teste

            var senha = _faker.Random.AlphaNumeric(20);
            var senhaCriptografada = Criptografia.CriptografarComSha512(senha);

            var usuarioParaTeste = ConstrutorDeUsuariosTeste
                .RetornarConstrutor(ativo: true, senha: senhaCriptografada)
                .ConstruirCadastrado();

            var servico = _mocker.CreateInstance<AutenticarUsuario>();
            var retornarUsuarioAutenticado = _mocker.CreateInstance<RetornarUsuarioAutenticado>();
            _mocker.GetMock<IRepositorioDeUsuarios>().Setup(r => r.BuscarPorEmailAsync(usuarioParaTeste.Email)).ReturnsAsync(usuarioParaTeste);
            _mocker.GetMock<IServiceProvider>().Setup(s => s.GetService(typeof(RetornarUsuarioAutenticado))).Returns(retornarUsuarioAutenticado);

            var dadosAutenticacao = new DadosAutenticacao(
                nomeUsuario: null,
                email: usuarioParaTeste.Email,
                senha: senha);

            #endregion

            var resposta = await servico.ExecutarAsync(dadosAutenticacao);

            #region Verificação do teste

            var usuarioResposta = (UsuarioAutenticado?)resposta;
            Assert.NotNull(usuarioResposta);
            Assert.Equal(usuarioParaTeste.Nome, usuarioResposta.Usuario.Nome);
            Assert.Equal(usuarioParaTeste.NomeUsuario, usuarioResposta.Usuario.NomeUsuario);
            Assert.Equal(usuarioParaTeste.Email, usuarioResposta.Usuario.Email);
            Assert.False(usuarioResposta.Token.EstaVazio());

            #endregion
        }

        [Fact]
        internal async Task NaoDeveAutenticarQuandoNomeUsuarioEEmailNaoPreenchidos()
        {
            #region Preparação do teste

            var servico = _mocker.CreateInstance<AutenticarUsuario>();

            var dadosAutenticacao = new DadosAutenticacao(
                nomeUsuario: null,
                email: null,
                senha: "teste.de_senh4@@");

            #endregion

            var resposta = await servico.ExecutarAsync(dadosAutenticacao);

            #region Verificação do teste

            Assert.Null(resposta);
            _mocker.Verify<INotificador>(n => n.AddMensagem(MensagensValidacaoUsuario.NomeUsuarioOuEmailObrigatorio), Times.Once);

            #endregion
        }

        [Fact]
        internal async Task NaoDeveAutenticarPorNomeUsuarioQuandoUsuarioNaoExiste()
        {
            #region Preparação do teste

            var servico = _mocker.CreateInstance<AutenticarUsuario>();

            var dadosAutenticacao = new DadosAutenticacao(
                nomeUsuario: _faker.Person.UserName,
                email: null,
                senha: "teste.de_senh4@@");

            #endregion

            var resposta = await servico.ExecutarAsync(dadosAutenticacao);

            #region Verificação do teste

            Assert.Null(resposta);
            _mocker.Verify<INotificador>(n => n.AddMensagemNaoAutorizado(MensagensValidacaoUsuario.NaoAutenticado), Times.Once);

            #endregion
        }

        [Fact]
        internal async Task NaoDeveAutenticarPorEmailQuandoUsuarioNaoExiste()
        {
            #region Preparação do teste

            var servico = _mocker.CreateInstance<AutenticarUsuario>();

            var dadosAutenticacao = new DadosAutenticacao(
                nomeUsuario: null,
                email: _faker.Person.Email,
                senha: "teste.de_senh4@@");

            #endregion

            var resposta = await servico.ExecutarAsync(dadosAutenticacao);

            #region Verificação do teste

            Assert.Null(resposta);
            _mocker.Verify<INotificador>(n => n.AddMensagemNaoAutorizado(MensagensValidacaoUsuario.NaoAutenticado), Times.Once);

            #endregion
        }

        [Fact]
        internal async Task NaoDeveAutenticarPorNomeUsuarioQuandoUsuarioNaoAtivo()
        {
            #region Preparação do teste

            var idUsuario = Guid.NewGuid();

            var usuarioParaTeste = ConstrutorDeUsuariosTeste
                .RetornarConstrutor(id: idUsuario, ativo: false)
                .ConstruirCadastrado();

            var servico = _mocker.CreateInstance<AutenticarUsuario>();

            _mocker.GetMock<IRepositorioDeUsuarios>().Setup(r => r.BuscarPorNomeUsuarioAsync(usuarioParaTeste.NomeUsuario)).ReturnsAsync(usuarioParaTeste);

            var dadosAutenticacao = new DadosAutenticacao(
                nomeUsuario: usuarioParaTeste.NomeUsuario,
                email: null,
                senha: "teste.de_senh4@@");

            #endregion

            var resposta = await servico.ExecutarAsync(dadosAutenticacao);

            #region Verificação do teste

            Assert.Null(resposta);
            _mocker.Verify<INotificador>(n => n.AddMensagemNaoAutorizado(MensagensValidacaoUsuario.NaoAutenticado), Times.Once);

            #endregion
        }

        [Fact]
        internal async Task NaoDeveAutenticarPorEmailQuandoUsuarioNaoAtivo()
        {
            #region Preparação do teste

            var usuarioParaTeste = ConstrutorDeUsuariosTeste
                .RetornarConstrutor(ativo: false)
                .ConstruirCadastrado();

            var servico = _mocker.CreateInstance<AutenticarUsuario>();

            _mocker.GetMock<IRepositorioDeUsuarios>().Setup(r => r.BuscarPorEmailAsync(usuarioParaTeste.Email)).ReturnsAsync(usuarioParaTeste);

            var dadosAutenticacao = new DadosAutenticacao(
                nomeUsuario: null,
                email: usuarioParaTeste.Email,
                senha: "teste.de_senh4@@");

            #endregion

            var resposta = await servico.ExecutarAsync(dadosAutenticacao);

            #region Verificação do teste

            Assert.Null(resposta);
            _mocker.Verify<INotificador>(n => n.AddMensagemNaoAutorizado(MensagensValidacaoUsuario.NaoAutenticado), Times.Once);

            #endregion
        }

        [Fact]
        internal async Task NaoDeveAutenticarPorNomeUsuarioQuandoSenhaIncorreta()
        {
            #region Preparação do teste

            var idUsuario = Guid.NewGuid();

            var usuarioParaTeste = ConstrutorDeUsuariosTeste
                .RetornarConstrutor(id: idUsuario, ativo: true, senha: _faker.Random.AlphaNumeric(30))
                .ConstruirCadastrado();

            var servico = _mocker.CreateInstance<AutenticarUsuario>();

            _mocker.GetMock<IRepositorioDeUsuarios>().Setup(r => r.BuscarPorNomeUsuarioAsync(usuarioParaTeste.NomeUsuario)).ReturnsAsync(usuarioParaTeste);

            var dadosAutenticacao = new DadosAutenticacao(
                nomeUsuario: usuarioParaTeste.NomeUsuario,
                email: null,
                senha: "teste.de_senh4@@");

            #endregion

            var resposta = await servico.ExecutarAsync(dadosAutenticacao);

            #region Verificação do teste

            Assert.Null(resposta);
            _mocker.Verify<INotificador>(n => n.AddMensagemNaoAutorizado(MensagensValidacaoUsuario.NaoAutenticado), Times.Once);

            #endregion
        }

        [Fact]
        internal async Task NaoDeveAutenticarPorEmailQuandoSenhaIncorreta()
        {
            #region Preparação do teste

            var idUsuario = Guid.NewGuid();

            var usuarioParaTeste = ConstrutorDeUsuariosTeste
                .RetornarConstrutor(id: idUsuario, ativo: true, senha: _faker.Random.AlphaNumeric(30))
                .ConstruirCadastrado();

            var servico = _mocker.CreateInstance<AutenticarUsuario>();

            _mocker.GetMock<IRepositorioDeUsuarios>().Setup(r => r.BuscarPorEmailAsync(usuarioParaTeste.Email)).ReturnsAsync(usuarioParaTeste);

            var dadosAutenticacao = new DadosAutenticacao(
                nomeUsuario: null,
                email: usuarioParaTeste.Email,
                senha: "teste.de_senh4@@");

            #endregion

            var resposta = await servico.ExecutarAsync(dadosAutenticacao);

            #region Verificação do teste

            Assert.Null(resposta);
            _mocker.Verify<INotificador>(n => n.AddMensagemNaoAutorizado(MensagensValidacaoUsuario.NaoAutenticado), Times.Once);

            #endregion
        }


        [Fact]
        internal async Task DeveAutenticarEnviandoCodDoisFatores()
        {
            #region Preparação do teste

            var senha = _faker.Random.AlphaNumeric(20);
            var senhaCriptografada = Criptografia.CriptografarComSha512(senha);

            var usuarioParaTeste = ConstrutorDeUsuariosTeste
                .RetornarConstrutor(ativo: true, senha: senhaCriptografada, tipoDeAutenticacao: TipoDeAutenticacao.Email)
                .ConstruirCadastrado();

            var servico = _mocker.CreateInstance<AutenticarUsuario>();
            var enviarCodigoAutenticacaoUsuario = _mocker.CreateInstance<EnviarCodigoAutenticacaoUsuario>();
            var enviarCodAutenticacaoUsuarioPorEmail = _mocker.CreateInstance<EnviarCodAutenticacaoUsuarioPorEmail>();
            _mocker.GetMock<IRepositorioDeUsuarios>().Setup(r => r.BuscarPorEmailAsync(usuarioParaTeste.Email)).ReturnsAsync(usuarioParaTeste);
            _mocker.GetMock<IServiceProvider>().Setup(s => s.GetService(typeof(EnviarCodigoAutenticacaoUsuario))).Returns(enviarCodigoAutenticacaoUsuario);
            _mocker.GetMock<IServiceProvider>().Setup(s => s.GetService(typeof(EnviarCodAutenticacaoUsuarioPorEmail))).Returns(enviarCodAutenticacaoUsuarioPorEmail);

            var dadosAutenticacao = new DadosAutenticacao(
                nomeUsuario: null,
                email: usuarioParaTeste.Email,
                senha: senha);

            #endregion

            var resposta = await servico.ExecutarAsync(dadosAutenticacao);

            #region Verificação do teste

            var usuarioResposta = (RespostaAutenticacaoDoisFatores?)resposta;
            Assert.NotNull(usuarioResposta);
            Assert.NotEmpty(usuarioResposta.Token);

            #endregion
        }
    }
}

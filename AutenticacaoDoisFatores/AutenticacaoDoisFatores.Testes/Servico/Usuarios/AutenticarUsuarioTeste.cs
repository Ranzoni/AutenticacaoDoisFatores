using AutenticacaoDoisFatores.Dominio.Compartilhados;
using AutenticacaoDoisFatores.Dominio.Compartilhados.Mensagens;
using AutenticacaoDoisFatores.Dominio.Compartilhados.Usuarios;
using AutenticacaoDoisFatores.Dominio.Dominios;
using AutenticacaoDoisFatores.Dominio.Repositorios;
using AutenticacaoDoisFatores.Servico.CasosDeUso.Usuarios;
using AutenticacaoDoisFatores.Servico.CasosDeUso.Usuarios.Autenticadores;
using AutenticacaoDoisFatores.Servico.CasosDeUso.Usuarios.Autenticadores.AutenticacoesDoisFatores;
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
            var senhaCriptografada = Encrypt.EncryptWithSha512(senha);

            var usuarioParaTeste = ConstrutorDeUsuariosTeste
                .RetornarConstrutor(ativo: true, senha: senhaCriptografada)
                .Build();

            var servico = _mocker.CreateInstance<AuthenticateUser>();
            var retornarUsuarioAutenticado = _mocker.CreateInstance<BaseUserAuthenticator>();
            _mocker.GetMock<IUserRepository>().Setup(r => r.BuscarPorNomeUsuarioAsync(usuarioParaTeste.NomeUsuario)).ReturnsAsync(usuarioParaTeste);
            _mocker.GetMock<IServiceProvider>().Setup(s => s.GetService(typeof(BaseUserAuthenticator))).Returns(retornarUsuarioAutenticado);

            var dadosAutenticacao = new DadosAutenticacao(
                nomeUsuarioOuEmail: usuarioParaTeste.NomeUsuario,
                senha: senha);

            #endregion

            var resposta = await servico.ExecutarAsync(dadosAutenticacao);

            #region Verificação do teste

            var usuarioResposta = (AuthenticatedUser?)resposta;
            Assert.NotNull(usuarioResposta);
            Assert.Equal(usuarioParaTeste.Nome, usuarioResposta.User.Name);
            Assert.Equal(usuarioParaTeste.NomeUsuario, usuarioResposta.User.Username);
            Assert.Equal(usuarioParaTeste.Email, usuarioResposta.User.Email);
            Assert.False(usuarioResposta.Token.IsNullOrEmptyOrWhiteSpaces());

            #endregion
        }

        [Fact]
        internal async Task DeveAutenticarComEmail()
        {
            #region Preparação do teste

            var senha = _faker.Random.AlphaNumeric(20);
            var senhaCriptografada = Encrypt.EncryptWithSha512(senha);

            var usuarioParaTeste = ConstrutorDeUsuariosTeste
                .RetornarConstrutor(ativo: true, senha: senhaCriptografada)
                .Build();

            var servico = _mocker.CreateInstance<AuthenticateUser>();
            var retornarUsuarioAutenticado = _mocker.CreateInstance<BaseUserAuthenticator>();
            _mocker.GetMock<IUserRepository>().Setup(r => r.BuscarPorEmailAsync(usuarioParaTeste.Email)).ReturnsAsync(usuarioParaTeste);
            _mocker.GetMock<IServiceProvider>().Setup(s => s.GetService(typeof(BaseUserAuthenticator))).Returns(retornarUsuarioAutenticado);

            var dadosAutenticacao = new DadosAutenticacao(
                nomeUsuarioOuEmail: usuarioParaTeste.Email,
                senha: senha);

            #endregion

            var resposta = await servico.ExecutarAsync(dadosAutenticacao);

            #region Verificação do teste

            var usuarioResposta = (AuthenticatedUser?)resposta;
            Assert.NotNull(usuarioResposta);
            Assert.Equal(usuarioParaTeste.Nome, usuarioResposta.User.Name);
            Assert.Equal(usuarioParaTeste.NomeUsuario, usuarioResposta.User.Username);
            Assert.Equal(usuarioParaTeste.Email, usuarioResposta.User.Email);
            Assert.False(usuarioResposta.Token.IsNullOrEmptyOrWhiteSpaces());

            #endregion
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("  ")]
        internal async Task NaoDeveAutenticarQuandoNomeUsuarioEEmailNaoPreenchidos(string? nomeUsuarioOuEmailNaoPreenchido)
        {
            #region Preparação do teste

            var servico = _mocker.CreateInstance<AuthenticateUser>();

            var dadosAutenticacao = new AuthData(
                usernameOrEmail: nomeUsuarioOuEmailNaoPreenchido,
                password: "teste.de_senh4@@");

            #endregion

            var resposta = await servico.ExecutarAsync(dadosAutenticacao);

            #region Verificação do teste

            Assert.Null(resposta);
            _mocker.Verify<INotificador>(n => n.AddMensagem(UserValidationMessages.UsernameOrEmailRequired), Times.Once);

            #endregion
        }

        [Fact]
        internal async Task NaoDeveAutenticarPorNomeUsuarioQuandoUsuarioNaoExiste()
        {
            #region Preparação do teste

            var servico = _mocker.CreateInstance<AuthenticateUser>();

            var dadosAutenticacao = new AuthData(
                usernameOrEmail: _faker.Person.UserName,
                password: "teste.de_senh4@@");

            #endregion

            var resposta = await servico.ExecutarAsync(dadosAutenticacao);

            #region Verificação do teste

            Assert.Null(resposta);
            _mocker.Verify<INotificador>(n => n.AddMensagemNaoAutorizado(UserValidationMessages.Unauthorized), Times.Once);

            #endregion
        }

        [Fact]
        internal async Task NaoDeveAutenticarPorEmailQuandoUsuarioNaoExiste()
        {
            #region Preparação do teste

            var servico = _mocker.CreateInstance<AuthenticateUser>();

            var dadosAutenticacao = new AuthData(
                usernameOrEmail: _faker.Person.Email,
                password: "teste.de_senh4@@");

            #endregion

            var resposta = await servico.ExecutarAsync(dadosAutenticacao);

            #region Verificação do teste

            Assert.Null(resposta);
            _mocker.Verify<INotificador>(n => n.AddMensagemNaoAutorizado(UserValidationMessages.Unauthorized), Times.Once);

            #endregion
        }

        [Fact]
        internal async Task NaoDeveAutenticarPorNomeUsuarioQuandoUsuarioNaoAtivo()
        {
            #region Preparação do teste

            var idUsuario = Guid.NewGuid();

            var usuarioParaTeste = ConstrutorDeUsuariosTeste
                .RetornarConstrutor(id: idUsuario, ativo: false)
                .Build();

            var servico = _mocker.CreateInstance<AuthenticateUser>();

            _mocker.GetMock<IUserRepository>().Setup(r => r.BuscarPorNomeUsuarioAsync(usuarioParaTeste.NomeUsuario)).ReturnsAsync(usuarioParaTeste);

            var dadosAutenticacao = new DadosAutenticacao(
                nomeUsuarioOuEmail: usuarioParaTeste.NomeUsuario,
                senha: "teste.de_senh4@@");

            #endregion

            var resposta = await servico.ExecutarAsync(dadosAutenticacao);

            #region Verificação do teste

            Assert.Null(resposta);
            _mocker.Verify<INotificador>(n => n.AddMensagemNaoAutorizado(UserValidationMessages.Unauthorized), Times.Once);

            #endregion
        }

        [Fact]
        internal async Task NaoDeveAutenticarPorEmailQuandoUsuarioNaoAtivo()
        {
            #region Preparação do teste

            var usuarioParaTeste = ConstrutorDeUsuariosTeste
                .RetornarConstrutor(ativo: false)
                .Build();

            var servico = _mocker.CreateInstance<AuthenticateUser>();

            _mocker.GetMock<IUserRepository>().Setup(r => r.BuscarPorEmailAsync(usuarioParaTeste.Email)).ReturnsAsync(usuarioParaTeste);

            var dadosAutenticacao = new DadosAutenticacao(
                nomeUsuarioOuEmail: usuarioParaTeste.Email,
                senha: "teste.de_senh4@@");

            #endregion

            var resposta = await servico.ExecutarAsync(dadosAutenticacao);

            #region Verificação do teste

            Assert.Null(resposta);
            _mocker.Verify<INotificador>(n => n.AddMensagemNaoAutorizado(UserValidationMessages.Unauthorized), Times.Once);

            #endregion
        }

        [Fact]
        internal async Task NaoDeveAutenticarPorNomeUsuarioQuandoSenhaIncorreta()
        {
            #region Preparação do teste

            var idUsuario = Guid.NewGuid();

            var usuarioParaTeste = ConstrutorDeUsuariosTeste
                .RetornarConstrutor(id: idUsuario, ativo: true, senha: _faker.Random.AlphaNumeric(30))
                .Build();

            var servico = _mocker.CreateInstance<AuthenticateUser>();

            _mocker.GetMock<IUserRepository>().Setup(r => r.BuscarPorNomeUsuarioAsync(usuarioParaTeste.NomeUsuario)).ReturnsAsync(usuarioParaTeste);

            var dadosAutenticacao = new DadosAutenticacao(
                nomeUsuarioOuEmail: usuarioParaTeste.NomeUsuario,
                senha: "teste.de_senh4@@");

            #endregion

            var resposta = await servico.ExecutarAsync(dadosAutenticacao);

            #region Verificação do teste

            Assert.Null(resposta);
            _mocker.Verify<INotificador>(n => n.AddMensagemNaoAutorizado(UserValidationMessages.Unauthorized), Times.Once);

            #endregion
        }

        [Fact]
        internal async Task NaoDeveAutenticarPorEmailQuandoSenhaIncorreta()
        {
            #region Preparação do teste

            var idUsuario = Guid.NewGuid();

            var usuarioParaTeste = ConstrutorDeUsuariosTeste
                .RetornarConstrutor(id: idUsuario, ativo: true, senha: _faker.Random.AlphaNumeric(30))
                .Build();

            var servico = _mocker.CreateInstance<AuthenticateUser>();

            _mocker.GetMock<IUserRepository>().Setup(r => r.BuscarPorEmailAsync(usuarioParaTeste.Email)).ReturnsAsync(usuarioParaTeste);

            var dadosAutenticacao = new DadosAutenticacao(
                nomeUsuarioOuEmail: usuarioParaTeste.Email,
                senha: "teste.de_senh4@@");

            #endregion

            var resposta = await servico.ExecutarAsync(dadosAutenticacao);

            #region Verificação do teste

            Assert.Null(resposta);
            _mocker.Verify<INotificador>(n => n.AddMensagemNaoAutorizado(UserValidationMessages.Unauthorized), Times.Once);

            #endregion
        }


        [Fact]
        internal async Task DeveAutenticarEnviandoCodDoisFatores()
        {
            #region Preparação do teste

            var senha = _faker.Random.AlphaNumeric(20);
            var senhaCriptografada = Encrypt.EncryptWithSha512(senha);

            var usuarioParaTeste = ConstrutorDeUsuariosTeste
                .RetornarConstrutor(ativo: true, senha: senhaCriptografada, tipoDeAutenticacao: AuthType.Email)
                .Build();

            var servico = _mocker.CreateInstance<AuthenticateUser>();
            var enviarCodigoAutenticacaoUsuario = _mocker.CreateInstance<UserTwoFactorAuthentication>();
            var enviarCodAutenticacaoUsuarioPorEmail = _mocker.CreateInstance<UserTwoFactorAuthByEmail>();
            _mocker.GetMock<IUserRepository>().Setup(r => r.BuscarPorEmailAsync(usuarioParaTeste.Email)).ReturnsAsync(usuarioParaTeste);
            _mocker.GetMock<IServiceProvider>().Setup(s => s.GetService(typeof(UserTwoFactorAuthentication))).Returns(enviarCodigoAutenticacaoUsuario);
            _mocker.GetMock<IServiceProvider>().Setup(s => s.GetService(typeof(UserTwoFactorAuthByEmail))).Returns(enviarCodAutenticacaoUsuarioPorEmail);

            var dadosAutenticacao = new DadosAutenticacao(
                nomeUsuarioOuEmail: usuarioParaTeste.Email,
                senha: senha);

            #endregion

            var resposta = await servico.ExecutarAsync(dadosAutenticacao);

            #region Verificação do teste

            var usuarioResposta = (TwoFactorAuthResponse?)resposta;
            Assert.NotNull(usuarioResposta);
            Assert.NotEmpty(usuarioResposta.Token);

            #endregion
        }
    }
}

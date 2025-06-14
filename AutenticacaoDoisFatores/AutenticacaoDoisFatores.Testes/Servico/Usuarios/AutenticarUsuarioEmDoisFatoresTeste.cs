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

            var servico = mocker.CreateInstance<AutenticarUsuarioPorCodigo>();

            var idUsuario = Guid.NewGuid();
            var codigo = Seguranca.GerarCodigoAutenticacao();
            var codigoCriptografado = Encrypt.EncryptWithSha512(codigo);

            var usuario = ConstrutorDeUsuariosTeste
                .RetornarConstrutor(ativo: true, id: idUsuario)
                .Build();

            mocker.GetMock<IUserRepository>().Setup(r => r.GetByIdAsync(idUsuario)).ReturnsAsync(usuario);
            mocker.GetMock<IAuthCodeRepository>().Setup(r => r.GetByCodeAsync(idUsuario)).ReturnsAsync(codigoCriptografado);

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

            var servico = mocker.CreateInstance<AutenticarUsuarioPorCodigo>();

            var idUsuario = Guid.NewGuid();
            var codigo = Seguranca.GerarCodigoAutenticacao();
            var codigoCriptografado = Encrypt.EncryptWithSha512(codigo);

            var usuario = ConstrutorDeUsuariosTeste
                .RetornarConstrutor(ativo: true, id: idUsuario, tipoDeAutenticacao: AuthType.AppAutenticador)
                .Build();

            mocker.GetMock<IUserRepository>().Setup(r => r.GetByIdAsync(idUsuario)).ReturnsAsync(usuario);
            mocker.GetMock<IAuthService>().Setup(s => s.CodigoEhValido(codigo, usuario.ChaveSecreta)).Returns(true);

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

            var servico = mocker.CreateInstance<AutenticarUsuarioPorCodigo>();

            var idUsuario = Guid.NewGuid();
            var codigo = Seguranca.GerarCodigoAutenticacao();

            #endregion

            var resposta = await servico.ExecutarAsync(idUsuario, codigo);

            #region Verificação do teste

            Assert.Null(resposta);
            mocker.Verify<INotificador>(n => n.AddMensagemNaoAutorizado(UserValidationMessages.UserNotFound));

            #endregion
        }

        [Fact]
        internal async Task NaoDeveAutenticarQuandoUsuarioEstaInativo()
        {
            #region Preparação do teste

            var mocker = new AutoMocker();

            var servico = mocker.CreateInstance<AutenticarUsuarioPorCodigo>();

            var idUsuario = Guid.NewGuid();
            var codigo = Seguranca.GerarCodigoAutenticacao();
            var codigoCriptografado = Encrypt.EncryptWithSha512(codigo);

            var usuario = ConstrutorDeUsuariosTeste
                .RetornarConstrutor(ativo: false, id: idUsuario)
                .Build();

            mocker.GetMock<IUserRepository>().Setup(r => r.GetByIdAsync(idUsuario)).ReturnsAsync(usuario);
            mocker.GetMock<IAuthCodeRepository>().Setup(r => r.GetByCodeAsync(idUsuario)).ReturnsAsync(codigoCriptografado);

            #endregion

            var resposta = await servico.ExecutarAsync(idUsuario, codigo);

            #region Verificação do teste

            Assert.Null(resposta);
            mocker.Verify<INotificador>(n => n.AddMensagemNaoAutorizado(UserValidationMessages.UserNotFound));

            #endregion
        }

        [Fact]
        internal async Task NaoDeveAutenticarQuandoCodigoNaoExistir()
        {
            #region Preparação do teste

            var mocker = new AutoMocker();

            var servico = mocker.CreateInstance<AutenticarUsuarioPorCodigo>();

            var idUsuario = Guid.NewGuid();
            var codigo = Seguranca.GerarCodigoAutenticacao();

            var usuario = ConstrutorDeUsuariosTeste
                .RetornarConstrutor(ativo: true, id: idUsuario)
                .Build();

            mocker.GetMock<IUserRepository>().Setup(r => r.GetByIdAsync(idUsuario)).ReturnsAsync(usuario);

            #endregion

            var resposta = await servico.ExecutarAsync(idUsuario, codigo);

            #region Verificação do teste

            Assert.Null(resposta);
            mocker.Verify<INotificador>(n => n.AddMensagemNaoAutorizado(UserValidationMessages.Unauthorized));

            #endregion
        }

        [Fact]
        internal async Task NaoDeveAutenticarQuandoCodigoIncorreto()
        {
            #region Preparação do teste

            var mocker = new AutoMocker();

            var servico = mocker.CreateInstance<AutenticarUsuarioPorCodigo>();

            var idUsuario = Guid.NewGuid();
            var codigo = Seguranca.GerarCodigoAutenticacao();
            var codigoCriptografado = Encrypt.EncryptWithSha512(codigo);

            var usuario = ConstrutorDeUsuariosTeste
                .RetornarConstrutor(ativo: true, id: idUsuario)
                .Build();

            mocker.GetMock<IUserRepository>().Setup(r => r.GetByIdAsync(idUsuario)).ReturnsAsync(usuario);
            mocker.GetMock<IAuthCodeRepository>().Setup(r => r.GetByCodeAsync(idUsuario)).ReturnsAsync(codigoCriptografado);

            #endregion

            var resposta = await servico.ExecutarAsync(idUsuario, codigo + "1");

            #region Verificação do teste

            Assert.Null(resposta);
            mocker.Verify<INotificador>(n => n.AddMensagemNaoAutorizado(UserValidationMessages.Unauthorized));

            #endregion
        }

        [Fact]
        internal async Task NaoDeveAutenticarPorAppQuandoCodigoIncorreto()
        {
            #region Preparação do teste

            var mocker = new AutoMocker();

            var servico = mocker.CreateInstance<AutenticarUsuarioPorCodigo>();

            var idUsuario = Guid.NewGuid();
            var codigo = Seguranca.GerarCodigoAutenticacao();
            var codigoCriptografado = Encrypt.EncryptWithSha512(codigo);

            var usuario = ConstrutorDeUsuariosTeste
                .RetornarConstrutor(ativo: true, id: idUsuario, tipoDeAutenticacao: AuthType.AppAutenticador)
                .Build();

            mocker.GetMock<IUserRepository>().Setup(r => r.GetByIdAsync(idUsuario)).ReturnsAsync(usuario);
            mocker.GetMock<IAuthService>().Setup(s => s.CodigoEhValido(codigo, usuario.ChaveSecreta)).Returns(true);

            #endregion

            var resposta = await servico.ExecutarAsync(idUsuario, codigo + "1");

            #region Verificação do teste

            Assert.Null(resposta);
            mocker.Verify<INotificador>(n => n.AddMensagemNaoAutorizado(UserValidationMessages.Unauthorized));

            #endregion
        }
    }
}

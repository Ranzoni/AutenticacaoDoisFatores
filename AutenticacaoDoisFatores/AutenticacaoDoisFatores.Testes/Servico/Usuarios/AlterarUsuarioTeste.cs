using AutenticacaoDoisFatores.Dominio.Compartilhados.Mensagens;
using AutenticacaoDoisFatores.Dominio.Dominios;
using AutenticacaoDoisFatores.Dominio.Entidades;
using AutenticacaoDoisFatores.Dominio.Repositorios;
using AutenticacaoDoisFatores.Servico.CasosDeUso.Usuarios;
using AutenticacaoDoisFatores.Testes.Compartilhados;
using Bogus;
using Mensageiro;
using Moq;
using Moq.AutoMock;

namespace AutenticacaoDoisFatores.Testes.Servico.Usuarios
{
    public class AlterarUsuarioTeste
    {
        [Fact]
        internal async Task DeveAlterarUsuario()
        {
            #region Preparação do teste

            var mocker = new AutoMocker();
            var faker = new Faker();

            mocker.CreateInstance<UserDomain>();

            var servico = mocker.CreateInstance<AlterarUsuario>();

            var idUsuario = Guid.NewGuid();

            var usuarioParaAlterar = ConstrutorDeUsuariosTeste
                .RetornarConstrutor(id: idUsuario, nome: "Fulano de Tal", nomeUsuario: "user_test_12345", celular: 993000333, ativo: true)
                .Build();

            mocker.GetMock<IUserRepository>().Setup(r => r.GetByIdAsync(idUsuario)).ReturnsAsync(usuarioParaAlterar);

            var novoNome = faker.Person.FullName;
            var novoNomeUsuario = "user_test_54321";
            var novoCelular = 992222001;

            var novosDados = ConstrutorDeUsuariosTeste
                .RetornarConstrutorDeNovosDados(nome: novoNome, nomeUsuario: novoNomeUsuario, celular: novoCelular)
                .Construir();

            #endregion

            var resposta = await servico.ExecutarAsync(idUsuario, novosDados);

            #region Verificação do teste

            Assert.NotNull(resposta);
            Assert.Equal(novoNome, resposta.Nome);
            Assert.Equal(novoNomeUsuario, resposta.NomeUsuario);
            Assert.Equal(novoCelular, resposta.Celular);

            mocker.Verify<IUserRepository>(r => r.Editar(usuarioParaAlterar), Times.Once);
            mocker.Verify<IUserRepository>(r => r.SaveChangesAsync(), Times.Once);

            #endregion
        }

        [Fact]
        internal async Task NaoDeveAlterarUsuarioInexistente()
        {
            #region Preparação do teste

            var mocker = new AutoMocker();
            var faker = new Faker();

            var servico = mocker.CreateInstance<AlterarUsuario>();

            var idUsuario = Guid.NewGuid();

            var novosDados = ConstrutorDeUsuariosTeste
                .RetornarConstrutorDeNovosDados()
                .Construir();

            mocker.GetMock<INotificador>().Setup(n => n.ExisteMensagem()).Returns(true);

            #endregion

            var resposta = await servico.ExecutarAsync(idUsuario, novosDados);

            #region Verificação do teste

            Assert.Null(resposta);

            mocker.Verify<INotificador>(n => n.AddMensagemNaoEncontrado(UserValidationMessages.UserNotFound), Times.Once);
            mocker.Verify<IUserRepository>(r => r.Update(It.IsAny<User>()), Times.Never);
            mocker.Verify<IUserRepository>(r => r.SaveChangesAsync(), Times.Never);

            #endregion
        }

        [Fact]
        internal async Task NaoDeveAlterarUsuarioInativo()
        {
            #region Preparação do teste

            var mocker = new AutoMocker();
            var faker = new Faker();

            var servico = mocker.CreateInstance<AlterarUsuario>();

            var idUsuario = Guid.NewGuid();

            var usuarioParaAlterar = ConstrutorDeUsuariosTeste
                .RetornarConstrutor(id: idUsuario, nome: "Fulano de Tal", nomeUsuario: "user_test_12345", celular: 993000333, ativo: false)
                .Build();

            mocker.GetMock<IUserRepository>().Setup(r => r.GetByIdAsync(idUsuario)).ReturnsAsync(usuarioParaAlterar);

            var novoNome = faker.Person.FullName;
            var novoNomeUsuario = "user_test_54321";
            var novoCelular = 992222001;

            var novosDados = ConstrutorDeUsuariosTeste
                .RetornarConstrutorDeNovosDados(nome: novoNome, nomeUsuario: novoNomeUsuario, celular: novoCelular)
                .Construir();

            mocker.GetMock<INotificador>().Setup(n => n.ExisteMensagem()).Returns(true);

            #endregion

            var resposta = await servico.ExecutarAsync(idUsuario, novosDados);

            #region Verificação do teste

            Assert.Null(resposta);
            Assert.NotEqual(novoNome, usuarioParaAlterar.Nome);
            Assert.NotEqual(novoNomeUsuario, usuarioParaAlterar.NomeUsuario);
            Assert.NotEqual(novoCelular, usuarioParaAlterar.Celular);

            mocker.Verify<INotificador>(n => n.AddMensagemNaoEncontrado(UserValidationMessages.UserNotFound), Times.Once);
            mocker.Verify<IUserRepository>(r => r.Update(It.IsAny<User>()), Times.Never);
            mocker.Verify<IUserRepository>(r => r.SaveChangesAsync(), Times.Never);

            #endregion
        }

        [Fact]
        internal async Task NaoDeveAlterarUsuarioAdmin()
        {
            #region Preparação do teste

            var mocker = new AutoMocker();
            var faker = new Faker();

            var servico = mocker.CreateInstance<AlterarUsuario>();

            var idUsuario = Guid.NewGuid();

            var usuarioParaAlterar = ConstrutorDeUsuariosTeste
                .RetornarConstrutor(id: idUsuario, nome: "Fulano de Tal", nomeUsuario: "user_test_12345", ativo: true, celular: 993000333, ehAdm: true)
                .Build();

            mocker.GetMock<IUserRepository>().Setup(r => r.GetByIdAsync(idUsuario)).ReturnsAsync(usuarioParaAlterar);

            var novoNome = faker.Person.FullName;
            var novoNomeUsuario = "user_test_54321";
            var novoCelular = 992222001;

            var novosDados = ConstrutorDeUsuariosTeste
                .RetornarConstrutorDeNovosDados(nome: novoNome, nomeUsuario: novoNomeUsuario, celular: novoCelular)
                .Construir();

            mocker.GetMock<INotificador>().Setup(n => n.ExisteMensagem()).Returns(true);

            #endregion

            var resposta = await servico.ExecutarAsync(idUsuario, novosDados);

            #region Verificação do teste

            Assert.Null(resposta);
            Assert.NotEqual(novoNome, usuarioParaAlterar.Nome);
            Assert.NotEqual(novoNomeUsuario, usuarioParaAlterar.NomeUsuario);
            Assert.NotEqual(novoCelular, usuarioParaAlterar.Celular);

            mocker.Verify<INotificador>(n => n.AddMensagemNaoEncontrado(UserValidationMessages.UserNotFound), Times.Once);
            mocker.Verify<IUserRepository>(r => r.Update(It.IsAny<User>()), Times.Never);
            mocker.Verify<IUserRepository>(r => r.SaveChangesAsync(), Times.Never);

            #endregion
        }

        [Theory]
        [InlineData("")]
        [InlineData("         ")]
        [InlineData("ab")]
        [InlineData("Teste de nome grande Teste de nome grande Teste de ")]
        internal async Task NaoDeveAlterarParaNomeInvalido(string nomeInvalido)
        {
            #region Preparação do teste

            var mocker = new AutoMocker();
            var faker = new Faker();

            var servico = mocker.CreateInstance<AlterarUsuario>();

            var idUsuario = Guid.NewGuid();

            var usuarioParaAlterar = ConstrutorDeUsuariosTeste
                .RetornarConstrutor(id: idUsuario, nome: "Fulano de Tal", nomeUsuario: "user_test_12345", ativo: true)
                .Build();

            mocker.GetMock<IUserRepository>().Setup(r => r.GetByIdAsync(idUsuario)).ReturnsAsync(usuarioParaAlterar);

            var novoNomeUsuario = "user_test_54321";

            var novosDados = ConstrutorDeUsuariosTeste
                .RetornarConstrutorDeNovosDados(nome: nomeInvalido, nomeUsuario: novoNomeUsuario)
                .Construir();

            mocker.GetMock<INotificador>().Setup(n => n.ExisteMensagem()).Returns(true);

            #endregion

            var resposta = await servico.ExecutarAsync(idUsuario, novosDados);

            #region Verificação do teste

            Assert.Null(resposta);
            Assert.NotEqual(nomeInvalido, usuarioParaAlterar.Nome);
            Assert.NotEqual(novoNomeUsuario, usuarioParaAlterar.NomeUsuario);

            mocker.Verify<INotificador>(n => n.AddMensagem(UserValidationMessages.InvalidName), Times.Once);
            mocker.Verify<IUserRepository>(r => r.Update(It.IsAny<User>()), Times.Never);
            mocker.Verify<IUserRepository>(r => r.SaveChangesAsync(), Times.Never);

            #endregion
        }

        [Theory]
        [InlineData("")]
        [InlineData("         ")]
        [InlineData("abcd")]
        [InlineData("Teste de nome grande ")]
        internal async Task NaoDeveAlterarParaNomeUsuarioUsuarioInvalido(string nomeUsuarioInvalido)
        {
            #region Preparação do teste

            var mocker = new AutoMocker();
            var faker = new Faker();

            var servico = mocker.CreateInstance<AlterarUsuario>();

            var idUsuario = Guid.NewGuid();

            var usuarioParaAlterar = ConstrutorDeUsuariosTeste
                .RetornarConstrutor(id: idUsuario, nome: "Fulano de Tal", nomeUsuario: "user_test_12345", ativo: true)
                .Build();

            mocker.GetMock<IUserRepository>().Setup(r => r.GetByIdAsync(idUsuario)).ReturnsAsync(usuarioParaAlterar);

            var novoNome = faker.Person.FullName;

            var novosDados = ConstrutorDeUsuariosTeste
                .RetornarConstrutorDeNovosDados(nome: novoNome, nomeUsuario: nomeUsuarioInvalido)
                .Construir();

            mocker.GetMock<INotificador>().Setup(n => n.ExisteMensagem()).Returns(true);

            #endregion

            var resposta = await servico.ExecutarAsync(idUsuario, novosDados);

            #region Verificação do teste

            Assert.Null(resposta);
            Assert.NotEqual(novoNome, usuarioParaAlterar.Nome);
            Assert.NotEqual(nomeUsuarioInvalido, usuarioParaAlterar.NomeUsuario);

            mocker.Verify<INotificador>(n => n.AddMensagem(UserValidationMessages.InvalidUsername), Times.Once);
            mocker.Verify<IUserRepository>(r => r.Update(It.IsAny<User>()), Times.Never);
            mocker.Verify<IUserRepository>(r => r.SaveChangesAsync(), Times.Never);

            #endregion
        }

        [Theory]
        [InlineData((long)-9999)]
        [InlineData((long)1)]
        [InlineData((long)10334)]
        internal async Task NaoDeveAlterarParaCelularInvalido(long? novoCelular)
        {
            #region Preparação do teste

            var mocker = new AutoMocker();
            var faker = new Faker();

            var servico = mocker.CreateInstance<AlterarUsuario>();

            var idUsuario = Guid.NewGuid();

            var usuarioParaAlterar = ConstrutorDeUsuariosTeste
                .RetornarConstrutor(id: idUsuario, nome: "Fulano de Tal", celular: 993000333, ativo: true)
                .Build();

            mocker.GetMock<IUserRepository>().Setup(r => r.GetByIdAsync(idUsuario)).ReturnsAsync(usuarioParaAlterar);

            var novoNome = faker.Person.FullName;
            var novoNomeUsuario = "user_test_12345";

            var novosDados = ConstrutorDeUsuariosTeste
                .RetornarConstrutorDeNovosDados(nome: novoNome, nomeUsuario: novoNomeUsuario, celular: novoCelular)
                .Construir();

            mocker.GetMock<INotificador>().Setup(n => n.ExisteMensagem()).Returns(true);

            #endregion

            var resposta = await servico.ExecutarAsync(idUsuario, novosDados);

            #region Verificação do teste

            Assert.Null(resposta);
            Assert.NotEqual(novoNome, usuarioParaAlterar.Nome);
            Assert.NotEqual(novoNomeUsuario, usuarioParaAlterar.NomeUsuario);
            Assert.NotEqual(novoCelular, usuarioParaAlterar.Celular);

            mocker.Verify<INotificador>(n => n.AddMensagem(UserValidationMessages.InvalidPhone), Times.Once);
            mocker.Verify<IUserRepository>(r => r.Update(It.IsAny<User>()), Times.Never);
            mocker.Verify<IUserRepository>(r => r.SaveChangesAsync(), Times.Never);

            #endregion
        }

        [Fact]
        internal async Task NaoDeveAlterarUsuarioComNomeUsuarioJaCadastrado()
        {
            #region Preparação do teste

            var mocker = new AutoMocker();
            var faker = new Faker();

            var idUsuario = Guid.NewGuid();

            var usuarioParaAlterar = ConstrutorDeUsuariosTeste
                .RetornarConstrutor(id: idUsuario, nome: "Fulano de Tal", nomeUsuario: "user_test_12345", ativo: true)
                .Build();

            var servico = mocker.CreateInstance<AlterarUsuario>();

            var novoNome = faker.Person.FullName;
            var novoNomeUsuario = "user_test_54321";

            var novosDados = ConstrutorDeUsuariosTeste
                .RetornarConstrutorDeNovosDados(nome: novoNome, nomeUsuario: novoNomeUsuario)
                .Construir();

            mocker.GetMock<IUserRepository>().Setup(r => r.UsernameExistsAsync(novoNomeUsuario, idUsuario)).ReturnsAsync(true);
            mocker.GetMock<IUserRepository>().Setup(r => r.GetByIdAsync(idUsuario)).ReturnsAsync(usuarioParaAlterar);
            mocker.GetMock<INotificador>().Setup(n => n.ExisteMensagem()).Returns(true);

            #endregion

            var resposta = await servico.ExecutarAsync(idUsuario, novosDados);

            #region Verificação do teste

            Assert.Null(resposta);
            Assert.NotEqual(novoNome, usuarioParaAlterar.Nome);
            Assert.NotEqual(novoNomeUsuario, usuarioParaAlterar.NomeUsuario);

            mocker.Verify<INotificador>(n => n.AddMensagem(UserValidationMessages.UsernameAlreadyRegistered), Times.Once);
            mocker.Verify<IUserRepository>(r => r.Update(It.IsAny<User>()), Times.Never);
            mocker.Verify<IUserRepository>(r => r.SaveChangesAsync(), Times.Never);

            #endregion
        }
    }
}

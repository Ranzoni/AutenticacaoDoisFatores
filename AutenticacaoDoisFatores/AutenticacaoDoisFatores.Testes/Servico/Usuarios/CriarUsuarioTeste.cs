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
    public class CriarUsuarioTeste
    {
        private readonly AutoMocker _mocker = new();
        private readonly Faker _faker = new();

        [Fact]
        internal async Task DeveCriarUsuario()
        {
            #region Preparação do teste

            var nome = _faker.Person.FullName;
            var nomeUsuario = _faker.Person.UserName;
            var email = _faker.Person.Email;
            var celular = 55016993880077;

            var novoUsuario = ConstrutorDeUsuariosTeste
                .RetornarConstrutorDeNovo(nome: nome, nomeUsuario: nomeUsuario, email: email, celular: celular)
                .Build();

            _mocker.CreateInstance<UserDomain>();

            var servico = _mocker.CreateInstance<RegisterUser>();

            #endregion

            var usuarioCadastrado = await servico.CriarAsync(novoUsuario);

            #region Verificação do teste

            Assert.NotNull(usuarioCadastrado);
            Assert.Equal(nome, usuarioCadastrado.Nome);
            Assert.Equal(nomeUsuario, usuarioCadastrado.NomeUsuario);
            Assert.Equal(email, usuarioCadastrado.Email);
            Assert.False(usuarioCadastrado.Ativo);
            Assert.Equal(celular, usuarioCadastrado.Celular);

            _mocker.Verify<IUserRepository>(r => r.Add(It.IsAny<User>()), Times.Once);
            _mocker.Verify<IUserRepository>(r => r.SaveChangesAsync(), Times.Once);

            #endregion
        }

        [Theory]
        [InlineData("")]
        [InlineData("         ")]
        [InlineData("ab")]
        [InlineData("Teste de nome grande Teste de nome grande Teste de ")]
        internal async Task NaoDeveCriarUsuarioQuandoNomeEhInvalido(string nomeInvalido)
        {
            #region Preparação do teste

            var novoUsuario = ConstrutorDeUsuariosTeste
                .RetornarConstrutorDeNovo(nome: nomeInvalido)
                .Build();

            _mocker.CreateInstance<UserDomain>();

            _mocker.GetMock<INotificador>().Setup(n => n.ExisteMensagem()).Returns(true);

            var servico = _mocker.CreateInstance<RegisterUser>();

            #endregion

            var usuarioCadastrado = await servico.CriarAsync(novoUsuario);

            #region Verificação do teste

            Assert.Null(usuarioCadastrado);
            _mocker.Verify<IUserRepository>(r => r.Add(It.IsAny<User>()), Times.Never);
            _mocker.Verify<IUserRepository>(r => r.SaveChangesAsync(), Times.Never);
            _mocker.Verify<INotificador>(n => n.AddMensagem(UserValidationMessages.InvalidName), Times.Once);

            #endregion
        }

        [Theory]
        [InlineData("")]
        [InlineData("         ")]
        [InlineData("abcd")]
        [InlineData("Teste de nome grande ")]
        internal async Task NaoDeveCriarUsuarioQuandoNomeUsuarioEhInvalido(string nomeUsuarioInvalido)
        {
            #region Preparação do teste

            var novoUsuario = ConstrutorDeUsuariosTeste
                .RetornarConstrutorDeNovo(nomeUsuario: nomeUsuarioInvalido)
                .Build();

            _mocker.CreateInstance<UserDomain>();

            _mocker.GetMock<INotificador>().Setup(n => n.ExisteMensagem()).Returns(true);

            var servico = _mocker.CreateInstance<RegisterUser>();

            #endregion

            var usuarioCadastrado = await servico.CriarAsync(novoUsuario);

            #region Verificação do teste

            Assert.Null(usuarioCadastrado);
            _mocker.Verify<IUserRepository>(r => r.Add(It.IsAny<User>()), Times.Never);
            _mocker.Verify<IUserRepository>(r => r.SaveChangesAsync(), Times.Never);
            _mocker.Verify<INotificador>(n => n.AddMensagem(UserValidationMessages.InvalidUsername), Times.Once);

            #endregion
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("@")]
        [InlineData("a@")]
        [InlineData("a@.")]
        [InlineData("a@.com")]
        [InlineData("@.")]
        [InlineData("@.com")]
        [InlineData("@dominio.com")]
        [InlineData("abcdefghijklmnopqrstuvwxyz0123456789abcdefghijklmnopqrstuvwxyz0123456789abcdefghijklmnopqrstuvwxyz0123456789abcdefghijklmnopqrstuvwxyz0123456789abcdefghijklmnopqrstuvwxyz0123456789abcdefghijklmnopqrstuvwxyz0123456789abcdefghijklmnopqrstuvwxyz0123456789abcde")]
        internal async Task NaoDeveCriarUsuarioQuandoEmailEhInvalido(string emailInvalido)
        {
            #region Preparação do teste
            
            var novoUsuario = ConstrutorDeUsuariosTeste
                .RetornarConstrutorDeNovo(email: emailInvalido)
                .Build();

            _mocker.CreateInstance<UserDomain>();

            _mocker.GetMock<INotificador>().Setup(n => n.ExisteMensagem()).Returns(true);

            var servico = _mocker.CreateInstance<RegisterUser>();

            #endregion

            var usuarioCadastrado = await servico.CriarAsync(novoUsuario);

            #region Verificação do teste

            Assert.Null(usuarioCadastrado);
            _mocker.Verify<IUserRepository>(r => r.Add(It.IsAny<User>()), Times.Never);
            _mocker.Verify<IUserRepository>(r => r.SaveChangesAsync(), Times.Never);
            _mocker.Verify<INotificador>(n => n.AddMensagem(UserValidationMessages.InvalidEmail), Times.Once);

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
        internal async Task NaoDeveCriarUsuarioQuandoSenhaEhInvalida(string senhaInvalida)
        {
            #region Preparação do teste

            var novoUsuario = ConstrutorDeUsuariosTeste
                .RetornarConstrutorDeNovo(senha: senhaInvalida)
                .Build();

            _mocker.CreateInstance<UserDomain>();

            _mocker.GetMock<INotificador>().Setup(n => n.ExisteMensagem()).Returns(true);

            var servico = _mocker.CreateInstance<RegisterUser>();

            #endregion

            var usuarioCadastrado = await servico.CriarAsync(novoUsuario);

            #region Verificação do teste

            Assert.Null(usuarioCadastrado);
            _mocker.Verify<IUserRepository>(r => r.Add(It.IsAny<User>()), Times.Never);
            _mocker.Verify<IUserRepository>(r => r.SaveChangesAsync(), Times.Never);
            _mocker.Verify<INotificador>(n => n.AddMensagem(UserValidationMessages.InvalidPassword), Times.Once);

            #endregion
        }

        [Fact]
        internal async Task NaoDeveCriarUsuarioQuandoNomeUsuarioJaCadastrado()
        {
            #region Preparação do teste

            var novoUsuario = ConstrutorDeUsuariosTeste
                .RetornarConstrutorDeNovo()
                .Build();

            _mocker.CreateInstance<UserDomain>();
            _mocker.GetMock<IUserRepository>().Setup(r => r.UsernameExistsAsync(novoUsuario.NomeUsuario, It.IsAny<Guid?>())).ReturnsAsync(true);
            _mocker.GetMock<INotificador>().Setup(n => n.ExisteMensagem()).Returns(true);

            var servico = _mocker.CreateInstance<RegisterUser>();

            #endregion

            var usuarioCadastrado = await servico.CriarAsync(novoUsuario);

            #region Verificação do teste

            Assert.Null(usuarioCadastrado);
            _mocker.Verify<IUserRepository>(r => r.Add(It.IsAny<User>()), Times.Never);
            _mocker.Verify<IUserRepository>(r => r.SaveChangesAsync(), Times.Never);
            _mocker.Verify<INotificador>(n => n.AddMensagem(UserValidationMessages.UsernameAlreadyRegistered), Times.Once);

            #endregion
        }

        [Fact]
        internal async Task NaoDeveCriarUsuarioQuandoEmailJaCadastrado()
        {
            #region Preparação do teste

            var novoUsuario = ConstrutorDeUsuariosTeste
                .RetornarConstrutorDeNovo()
                .Build();

            _mocker.CreateInstance<UserDomain>();
            _mocker.GetMock<IUserRepository>().Setup(r => r.EmailExistsAsync(novoUsuario.Email, It.IsAny<Guid?>())).ReturnsAsync(true);
            _mocker.GetMock<INotificador>().Setup(n => n.ExisteMensagem()).Returns(true);

            var servico = _mocker.CreateInstance<RegisterUser>();

            #endregion

            var usuarioCadastrado = await servico.CriarAsync(novoUsuario);

            #region Verificação do teste

            Assert.Null(usuarioCadastrado);
            _mocker.Verify<IUserRepository>(r => r.Add(It.IsAny<User>()), Times.Never);
            _mocker.Verify<IUserRepository>(r => r.SaveChangesAsync(), Times.Never);
            _mocker.Verify<INotificador>(n => n.AddMensagem(UserValidationMessages.EmailAlreadyRegistered), Times.Once);

            #endregion
        }
    }
}

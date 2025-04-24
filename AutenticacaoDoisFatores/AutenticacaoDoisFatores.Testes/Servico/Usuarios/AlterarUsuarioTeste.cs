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

            mocker.CreateInstance<DominioDeUsuarios>();

            var servico = mocker.CreateInstance<AlterarUsuario>();

            var idUsuario = Guid.NewGuid();

            var usuarioParaAlterar = ConstrutorDeUsuariosTeste
                .RetornarConstrutor(id: idUsuario, nome: "Fulano de Tal", nomeUsuario: "user_test_12345", celular: 993000333, ativo: true)
                .ConstruirCadastrado();

            mocker.GetMock<IRepositorioDeUsuarios>().Setup(r => r.BuscarUnicoAsync(idUsuario)).ReturnsAsync(usuarioParaAlterar);

            var novoNome = faker.Person.FullName;
            var novoNomeUsuario = "user_test_54321";
            var novoEmail = faker.Person.Email;
            var novoCelular = 992222001;

            var novosDados = ConstrutorDeUsuariosTeste
                .RetornarConstrutorDeNovosDados(nome: novoNome, nomeUsuario: novoNomeUsuario, email: novoEmail, celular: novoCelular)
                .Construir();

            #endregion

            var resposta = await servico.ExecutarAsync(idUsuario, novosDados);

            #region Verificação do teste

            Assert.NotNull(resposta);
            Assert.Equal(novoNome, resposta.Nome);
            Assert.Equal(novoNomeUsuario, resposta.NomeUsuario);
            Assert.Equal(novoEmail, resposta.Email);
            Assert.Equal(novoCelular, resposta.Celular);

            mocker.Verify<IRepositorioDeUsuarios>(r => r.Editar(usuarioParaAlterar), Times.Once);
            mocker.Verify<IRepositorioDeUsuarios>(r => r.SalvarAlteracoesAsync(), Times.Once);

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

            mocker.Verify<INotificador>(n => n.AddMensagemNaoEncontrado(MensagensValidacaoUsuario.UsuarioNaoEncontrado), Times.Once);
            mocker.Verify<IRepositorioDeUsuarios>(r => r.Editar(It.IsAny<Usuario>()), Times.Never);
            mocker.Verify<IRepositorioDeUsuarios>(r => r.SalvarAlteracoesAsync(), Times.Never);

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
                .ConstruirCadastrado();

            mocker.GetMock<IRepositorioDeUsuarios>().Setup(r => r.BuscarUnicoAsync(idUsuario)).ReturnsAsync(usuarioParaAlterar);

            var novoNome = faker.Person.FullName;
            var novoNomeUsuario = "user_test_54321";
            var novoEmail = faker.Person.Email;
            var novoCelular = 992222001;

            var novosDados = ConstrutorDeUsuariosTeste
                .RetornarConstrutorDeNovosDados(nome: novoNome, nomeUsuario: novoNomeUsuario, email: novoEmail, celular: novoCelular)
                .Construir();

            mocker.GetMock<INotificador>().Setup(n => n.ExisteMensagem()).Returns(true);

            #endregion

            var resposta = await servico.ExecutarAsync(idUsuario, novosDados);

            #region Verificação do teste

            Assert.Null(resposta);
            Assert.NotEqual(novoNome, usuarioParaAlterar.Nome);
            Assert.NotEqual(novoNomeUsuario, usuarioParaAlterar.NomeUsuario);
            Assert.NotEqual(novoEmail, usuarioParaAlterar.Email);
            Assert.NotEqual(novoCelular, usuarioParaAlterar.Celular);

            mocker.Verify<INotificador>(n => n.AddMensagemNaoEncontrado(MensagensValidacaoUsuario.UsuarioNaoEncontrado), Times.Once);
            mocker.Verify<IRepositorioDeUsuarios>(r => r.Editar(It.IsAny<Usuario>()), Times.Never);
            mocker.Verify<IRepositorioDeUsuarios>(r => r.SalvarAlteracoesAsync(), Times.Never);

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
                .ConstruirCadastrado();

            mocker.GetMock<IRepositorioDeUsuarios>().Setup(r => r.BuscarUnicoAsync(idUsuario)).ReturnsAsync(usuarioParaAlterar);

            var novoNome = faker.Person.FullName;
            var novoNomeUsuario = "user_test_54321";
            var novoEmail = faker.Person.Email;
            var novaSenha = "teste_nova_senh@!!4";
            var novoCelular = 992222001;

            var novosDados = ConstrutorDeUsuariosTeste
                .RetornarConstrutorDeNovosDados(nome: novoNome, nomeUsuario: novoNomeUsuario, email: novoEmail, senha: novaSenha, celular: novoCelular)
                .Construir();

            mocker.GetMock<INotificador>().Setup(n => n.ExisteMensagem()).Returns(true);

            #endregion

            var resposta = await servico.ExecutarAsync(idUsuario, novosDados);

            #region Verificação do teste

            Assert.Null(resposta);
            Assert.NotEqual(novoNome, usuarioParaAlterar.Nome);
            Assert.NotEqual(novoNomeUsuario, usuarioParaAlterar.NomeUsuario);
            Assert.NotEqual(novoEmail, usuarioParaAlterar.Email);
            Assert.NotEqual(novoCelular, usuarioParaAlterar.Celular);

            mocker.Verify<INotificador>(n => n.AddMensagemNaoEncontrado(MensagensValidacaoUsuario.UsuarioNaoEncontrado), Times.Once);
            mocker.Verify<IRepositorioDeUsuarios>(r => r.Editar(It.IsAny<Usuario>()), Times.Never);
            mocker.Verify<IRepositorioDeUsuarios>(r => r.SalvarAlteracoesAsync(), Times.Never);

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
                .ConstruirCadastrado();

            mocker.GetMock<IRepositorioDeUsuarios>().Setup(r => r.BuscarUnicoAsync(idUsuario)).ReturnsAsync(usuarioParaAlterar);

            var novoNomeUsuario = "user_test_54321";
            var novoEmail = faker.Person.Email;

            var novosDados = ConstrutorDeUsuariosTeste
                .RetornarConstrutorDeNovosDados(nome: nomeInvalido, nomeUsuario: novoNomeUsuario, email: novoEmail)
                .Construir();

            mocker.GetMock<INotificador>().Setup(n => n.ExisteMensagem()).Returns(true);

            #endregion

            var resposta = await servico.ExecutarAsync(idUsuario, novosDados);

            #region Verificação do teste

            Assert.Null(resposta);
            Assert.NotEqual(nomeInvalido, usuarioParaAlterar.Nome);
            Assert.NotEqual(novoNomeUsuario, usuarioParaAlterar.NomeUsuario);
            Assert.NotEqual(novoEmail, usuarioParaAlterar.Email);

            mocker.Verify<INotificador>(n => n.AddMensagem(MensagensValidacaoUsuario.NomeInvalido), Times.Once);
            mocker.Verify<IRepositorioDeUsuarios>(r => r.Editar(It.IsAny<Usuario>()), Times.Never);
            mocker.Verify<IRepositorioDeUsuarios>(r => r.SalvarAlteracoesAsync(), Times.Never);

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
                .ConstruirCadastrado();

            mocker.GetMock<IRepositorioDeUsuarios>().Setup(r => r.BuscarUnicoAsync(idUsuario)).ReturnsAsync(usuarioParaAlterar);

            var novoNome = faker.Person.FullName;
            var novoEmail = faker.Person.Email;

            var novosDados = ConstrutorDeUsuariosTeste
                .RetornarConstrutorDeNovosDados(nome: novoNome, nomeUsuario: nomeUsuarioInvalido, email: novoEmail)
                .Construir();

            mocker.GetMock<INotificador>().Setup(n => n.ExisteMensagem()).Returns(true);

            #endregion

            var resposta = await servico.ExecutarAsync(idUsuario, novosDados);

            #region Verificação do teste

            Assert.Null(resposta);
            Assert.NotEqual(novoNome, usuarioParaAlterar.Nome);
            Assert.NotEqual(nomeUsuarioInvalido, usuarioParaAlterar.NomeUsuario);
            Assert.NotEqual(novoEmail, usuarioParaAlterar.Email);

            mocker.Verify<INotificador>(n => n.AddMensagem(MensagensValidacaoUsuario.NomeUsuarioInvalido), Times.Once);
            mocker.Verify<IRepositorioDeUsuarios>(r => r.Editar(It.IsAny<Usuario>()), Times.Never);
            mocker.Verify<IRepositorioDeUsuarios>(r => r.SalvarAlteracoesAsync(), Times.Never);

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
        internal async Task NaoDeveAlterarParaEmailInvalido(string emailInvalido)
        {
            #region Preparação do teste

            var mocker = new AutoMocker();
            var faker = new Faker();

            var servico = mocker.CreateInstance<AlterarUsuario>();

            var idUsuario = Guid.NewGuid();

            var usuarioParaAlterar = ConstrutorDeUsuariosTeste
                .RetornarConstrutor(id: idUsuario, nome: "Fulano de Tal", ativo: true)
                .ConstruirCadastrado();

            mocker.GetMock<IRepositorioDeUsuarios>().Setup(r => r.BuscarUnicoAsync(idUsuario)).ReturnsAsync(usuarioParaAlterar);

            var novoNome = faker.Person.FullName;
            var novoNomeUsuario = "user_test_12345";

            var novosDados = ConstrutorDeUsuariosTeste
                .RetornarConstrutorDeNovosDados(nome: novoNome, nomeUsuario: novoNomeUsuario, email: emailInvalido)
                .Construir();

            mocker.GetMock<INotificador>().Setup(n => n.ExisteMensagem()).Returns(true);

            #endregion

            var resposta = await servico.ExecutarAsync(idUsuario, novosDados);

            #region Verificação do teste

            Assert.Null(resposta);
            Assert.NotEqual(novoNome, usuarioParaAlterar.Nome);
            Assert.NotEqual(novoNomeUsuario, usuarioParaAlterar.NomeUsuario);
            Assert.NotEqual(emailInvalido, usuarioParaAlterar.Email);

            mocker.Verify<INotificador>(n => n.AddMensagem(MensagensValidacaoUsuario.EmailInvalido), Times.Once);
            mocker.Verify<IRepositorioDeUsuarios>(r => r.Editar(It.IsAny<Usuario>()), Times.Never);
            mocker.Verify<IRepositorioDeUsuarios>(r => r.SalvarAlteracoesAsync(), Times.Never);

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
        internal async Task NaoDeveAlterarParaSenhaInvalido(string senhaInvalida)
        {
            #region Preparação do teste

            var mocker = new AutoMocker();
            var faker = new Faker();

            var servico = mocker.CreateInstance<AlterarUsuario>();

            var idUsuario = Guid.NewGuid();

            var usuarioParaAlterar = ConstrutorDeUsuariosTeste
                .RetornarConstrutor(id: idUsuario, nome: "Fulano de Tal", ativo: true)
                .ConstruirCadastrado();

            mocker.GetMock<IRepositorioDeUsuarios>().Setup(r => r.BuscarUnicoAsync(idUsuario)).ReturnsAsync(usuarioParaAlterar);

            var novoNome = faker.Person.FullName;
            var novoEmail = faker.Person.Email;
            var novoNomeUsuario = "user_test_12345";

            var novosDados = ConstrutorDeUsuariosTeste
                .RetornarConstrutorDeNovosDados(nome: novoNome, nomeUsuario: novoNomeUsuario, email: novoEmail, senha: senhaInvalida)
                .Construir();

            mocker.GetMock<INotificador>().Setup(n => n.ExisteMensagem()).Returns(true);

            #endregion

            var resposta = await servico.ExecutarAsync(idUsuario, novosDados);

            #region Verificação do teste

            Assert.Null(resposta);
            Assert.NotEqual(novoNome, usuarioParaAlterar.Nome);
            Assert.NotEqual(novoNomeUsuario, usuarioParaAlterar.NomeUsuario);
            Assert.NotEqual(novoEmail, usuarioParaAlterar.Email);

            mocker.Verify<INotificador>(n => n.AddMensagem(MensagensValidacaoUsuario.SenhaInvalida), Times.Once);
            mocker.Verify<IRepositorioDeUsuarios>(r => r.Editar(It.IsAny<Usuario>()), Times.Never);
            mocker.Verify<IRepositorioDeUsuarios>(r => r.SalvarAlteracoesAsync(), Times.Never);

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
                .ConstruirCadastrado();

            mocker.GetMock<IRepositorioDeUsuarios>().Setup(r => r.BuscarUnicoAsync(idUsuario)).ReturnsAsync(usuarioParaAlterar);

            var novoNome = faker.Person.FullName;
            var novoNomeUsuario = "user_test_12345";
            var novoEmail = faker.Person.Email;

            var novosDados = ConstrutorDeUsuariosTeste
                .RetornarConstrutorDeNovosDados(nome: novoNome, nomeUsuario: novoNomeUsuario, email: novoEmail, celular: novoCelular)
                .Construir();

            mocker.GetMock<INotificador>().Setup(n => n.ExisteMensagem()).Returns(true);

            #endregion

            var resposta = await servico.ExecutarAsync(idUsuario, novosDados);

            #region Verificação do teste

            Assert.Null(resposta);
            Assert.NotEqual(novoNome, usuarioParaAlterar.Nome);
            Assert.NotEqual(novoNomeUsuario, usuarioParaAlterar.NomeUsuario);
            Assert.NotEqual(novoEmail, usuarioParaAlterar.Email);
            Assert.NotEqual(novoCelular, usuarioParaAlterar.Celular);

            mocker.Verify<INotificador>(n => n.AddMensagem(MensagensValidacaoUsuario.CelularInvalido), Times.Once);
            mocker.Verify<IRepositorioDeUsuarios>(r => r.Editar(It.IsAny<Usuario>()), Times.Never);
            mocker.Verify<IRepositorioDeUsuarios>(r => r.SalvarAlteracoesAsync(), Times.Never);

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
                .ConstruirCadastrado();

            var servico = mocker.CreateInstance<AlterarUsuario>();

            var novoNome = faker.Person.FullName;
            var novoNomeUsuario = "user_test_54321";
            var novoEmail = faker.Person.Email;
            var novaSenha = "teste_nova_senh@!!4";

            var novosDados = ConstrutorDeUsuariosTeste
                .RetornarConstrutorDeNovosDados(nome: novoNome, nomeUsuario: novoNomeUsuario, email: novoEmail, senha: novaSenha)
                .Construir();

            mocker.GetMock<IRepositorioDeUsuarios>().Setup(r => r.ExisteNomeUsuarioAsync(novoNomeUsuario, idUsuario)).ReturnsAsync(true);
            mocker.GetMock<IRepositorioDeUsuarios>().Setup(r => r.BuscarUnicoAsync(idUsuario)).ReturnsAsync(usuarioParaAlterar);
            mocker.GetMock<INotificador>().Setup(n => n.ExisteMensagem()).Returns(true);

            #endregion

            var resposta = await servico.ExecutarAsync(idUsuario, novosDados);

            #region Verificação do teste

            Assert.Null(resposta);
            Assert.NotEqual(novoNome, usuarioParaAlterar.Nome);
            Assert.NotEqual(novoNomeUsuario, usuarioParaAlterar.NomeUsuario);

            mocker.Verify<INotificador>(n => n.AddMensagem(MensagensValidacaoUsuario.NomeUsuarioJaCadastrado), Times.Once);
            mocker.Verify<IRepositorioDeUsuarios>(r => r.Editar(It.IsAny<Usuario>()), Times.Never);
            mocker.Verify<IRepositorioDeUsuarios>(r => r.SalvarAlteracoesAsync(), Times.Never);

            #endregion
        }

        [Fact]
        internal async Task NaoDeveAlterarUsuarioComEmailJaCadastrado()
        {
            #region Preparação do teste

            var mocker = new AutoMocker();
            var faker = new Faker();

            var idUsuario = Guid.NewGuid();

            var usuarioParaAlterar = ConstrutorDeUsuariosTeste
                .RetornarConstrutor(id: idUsuario, nome: "Fulano de Tal", nomeUsuario: "user_test_12345", ativo: true)
                .ConstruirCadastrado();

            var servico = mocker.CreateInstance<AlterarUsuario>();

            var novoNome = faker.Person.FullName;
            var novoNomeUsuario = "user_test_54321";
            var novoEmail = faker.Person.Email;
            var novaSenha = "teste_nova_senh@!!4";

            var novosDados = ConstrutorDeUsuariosTeste
                .RetornarConstrutorDeNovosDados(nome: novoNome, nomeUsuario: novoNomeUsuario, email: novoEmail, senha: novaSenha)
                .Construir();

            mocker.GetMock<IRepositorioDeUsuarios>().Setup(r => r.ExisteEmailAsync(novoEmail, idUsuario)).ReturnsAsync(true);
            mocker.GetMock<IRepositorioDeUsuarios>().Setup(r => r.BuscarUnicoAsync(idUsuario)).ReturnsAsync(usuarioParaAlterar);
            mocker.GetMock<INotificador>().Setup(n => n.ExisteMensagem()).Returns(true);

            #endregion

            var resposta = await servico.ExecutarAsync(idUsuario, novosDados);

            #region Verificação do teste

            Assert.Null(resposta);
            Assert.NotEqual(novoNome, usuarioParaAlterar.Nome);
            Assert.NotEqual(novoNomeUsuario, usuarioParaAlterar.NomeUsuario);

            mocker.Verify<INotificador>(n => n.AddMensagem(MensagensValidacaoUsuario.EmailJaCadastrado), Times.Once);
            mocker.Verify<IRepositorioDeUsuarios>(r => r.Editar(It.IsAny<Usuario>()), Times.Never);
            mocker.Verify<IRepositorioDeUsuarios>(r => r.SalvarAlteracoesAsync(), Times.Never);

            #endregion
        }
    }
}

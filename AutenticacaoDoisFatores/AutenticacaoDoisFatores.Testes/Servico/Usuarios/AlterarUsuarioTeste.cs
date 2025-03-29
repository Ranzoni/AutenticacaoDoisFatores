using AutenticacaoDoisFatores.Dominio.Compartilhados.Mensagens;
using AutenticacaoDoisFatores.Dominio.Dominios;
using AutenticacaoDoisFatores.Dominio.Entidades;
using AutenticacaoDoisFatores.Dominio.Repositorios;
using AutenticacaoDoisFatores.Servico.CasosDeUso.Usuarios;
using AutenticacaoDoisFatores.Servico.Mapeadores;
using AutenticacaoDoisFatores.Testes.Compartilhados;
using AutoMapper;
using Bogus;
using Mensageiro;
using Moq;
using Moq.AutoMock;

namespace AutenticacaoDoisFatores.Testes.Servico.Usuarios
{
    public class AlterarUsuarioTeste
    {
        private readonly IMapper _mapeador;

        public AlterarUsuarioTeste()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<MapeadorDeUsuario>();
            });
            _mapeador = config.CreateMapper();
        }

        [Fact]
        internal async Task DeveAlterarUsuario()
        {
            #region Preparação do teste

            var mocker = new AutoMocker();
            var faker = new Faker();

            mocker.CreateInstance<DominioDeUsuarios>();
            mocker.Use(_mapeador);

            var servico = mocker.CreateInstance<AlterarUsuario>();

            var idUsuario = Guid.NewGuid();

            var usuarioParaAlterar = ConstrutorDeUsuariosTeste
                .RetornarConstrutor(id: idUsuario, nome: "Fulano de Tal", nomeUsuario: "user_test_12345", ativo: true)
                .ConstruirCadastrado();

            mocker.GetMock<IRepositorioDeUsuarios>().Setup(r => r.BuscarUnicoAsync(idUsuario)).ReturnsAsync(usuarioParaAlterar);

            var novoNome = faker.Person.FullName;
            var novoNomeUsuario = "user_test_54321";

            var novosDadosUsuario = ConstrutorDeUsuariosTeste
                .RetornarConstrutorDeNovosDadosUsuario(nome: novoNome, nomeUsuario: novoNomeUsuario)
                .Construir();

            #endregion

            var resposta = await servico.ExecutarAsync(idUsuario, novosDadosUsuario);

            #region Verificação do teste

            Assert.NotNull(resposta);
            Assert.Equal(novoNome, resposta.Nome);
            Assert.Equal(novoNomeUsuario, resposta.NomeUsuario);

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

            var novosDadosUsuario = ConstrutorDeUsuariosTeste
                .RetornarConstrutorDeNovosDadosUsuario()
                .Construir();

            mocker.GetMock<INotificador>().Setup(n => n.ExisteMensagem()).Returns(true);

            #endregion

            var resposta = await servico.ExecutarAsync(idUsuario, novosDadosUsuario);

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
                .RetornarConstrutor(id: idUsuario, nome: "Fulano de Tal", nomeUsuario: "user_test_12345", ativo: false)
                .ConstruirCadastrado();

            mocker.GetMock<IRepositorioDeUsuarios>().Setup(r => r.BuscarUnicoAsync(idUsuario)).ReturnsAsync(usuarioParaAlterar);

            var novoNome = faker.Person.FullName;
            var novoNomeUsuario = "user_test_54321";

            var novosDadosUsuario = ConstrutorDeUsuariosTeste
                .RetornarConstrutorDeNovosDadosUsuario(nome: novoNome, nomeUsuario: novoNomeUsuario)
                .Construir();

            mocker.GetMock<INotificador>().Setup(n => n.ExisteMensagem()).Returns(true);

            #endregion

            var resposta = await servico.ExecutarAsync(idUsuario, novosDadosUsuario);

            #region Verificação do teste

            Assert.Null(resposta);
            Assert.NotEqual(novoNome, usuarioParaAlterar.Nome);
            Assert.NotEqual(novoNomeUsuario, usuarioParaAlterar.NomeUsuario);

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
                .RetornarConstrutor(id: idUsuario, nome: "Fulano de Tal", nomeUsuario: "user_test_12345", ativo: true, ehAdm: true)
                .ConstruirCadastrado();

            mocker.GetMock<IRepositorioDeUsuarios>().Setup(r => r.BuscarUnicoAsync(idUsuario)).ReturnsAsync(usuarioParaAlterar);

            var novoNome = faker.Person.FullName;
            var novoNomeUsuario = "user_test_54321";

            var novosDadosUsuario = ConstrutorDeUsuariosTeste
                .RetornarConstrutorDeNovosDadosUsuario(nome: novoNome, nomeUsuario: novoNomeUsuario)
                .Construir();

            mocker.GetMock<INotificador>().Setup(n => n.ExisteMensagem()).Returns(true);

            #endregion

            var resposta = await servico.ExecutarAsync(idUsuario, novosDadosUsuario);

            #region Verificação do teste

            Assert.Null(resposta);
            Assert.NotEqual(novoNome, usuarioParaAlterar.Nome);
            Assert.NotEqual(novoNomeUsuario, usuarioParaAlterar.NomeUsuario);

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

            var novosDadosUsuario = ConstrutorDeUsuariosTeste
                .RetornarConstrutorDeNovosDadosUsuario(nome: nomeInvalido, nomeUsuario: novoNomeUsuario)
                .Construir();

            mocker.GetMock<INotificador>().Setup(n => n.ExisteMensagem()).Returns(true);

            #endregion

            var resposta = await servico.ExecutarAsync(idUsuario, novosDadosUsuario);

            #region Verificação do teste

            Assert.Null(resposta);
            Assert.NotEqual(nomeInvalido, usuarioParaAlterar.Nome);
            Assert.NotEqual(novoNomeUsuario, usuarioParaAlterar.NomeUsuario);

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

            var novosDadosUsuario = ConstrutorDeUsuariosTeste
                .RetornarConstrutorDeNovosDadosUsuario(nome: novoNome, nomeUsuario: nomeUsuarioInvalido)
                .Construir();

            mocker.GetMock<INotificador>().Setup(n => n.ExisteMensagem()).Returns(true);

            #endregion

            var resposta = await servico.ExecutarAsync(idUsuario, novosDadosUsuario);

            #region Verificação do teste

            Assert.Null(resposta);
            Assert.NotEqual(novoNome, usuarioParaAlterar.Nome);
            Assert.NotEqual(nomeUsuarioInvalido, usuarioParaAlterar.NomeUsuario);

            mocker.Verify<INotificador>(n => n.AddMensagem(MensagensValidacaoUsuario.NomeUsuarioInvalido), Times.Once);
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

            var novoNome = faker.Person.FullName;
            var novoNomeUsuario = "user_test_54321";

            var usuarioParaAlterar = ConstrutorDeUsuariosTeste
                .RetornarConstrutor(id: idUsuario, nome: "Fulano de Tal", nomeUsuario: "user_test_12345", ativo: true)
                .ConstruirCadastrado();

            var servico = mocker.CreateInstance<AlterarUsuario>();
            mocker.GetMock<IRepositorioDeUsuarios>().Setup(r => r.ExisteNomeUsuarioAsync(novoNomeUsuario, idUsuario)).ReturnsAsync(true);
            mocker.GetMock<IRepositorioDeUsuarios>().Setup(r => r.BuscarUnicoAsync(idUsuario)).ReturnsAsync(usuarioParaAlterar);

            var novosDadosUsuario = ConstrutorDeUsuariosTeste
                .RetornarConstrutorDeNovosDadosUsuario(nome: novoNome, nomeUsuario: novoNomeUsuario)
                .Construir();

            mocker.GetMock<INotificador>().Setup(n => n.ExisteMensagem()).Returns(true);

            #endregion

            var resposta = await servico.ExecutarAsync(idUsuario, novosDadosUsuario);

            #region Verificação do teste

            Assert.Null(resposta);
            Assert.NotEqual(novoNome, usuarioParaAlterar.Nome);
            Assert.NotEqual(novoNomeUsuario, usuarioParaAlterar.NomeUsuario);

            mocker.Verify<INotificador>(n => n.AddMensagem(MensagensValidacaoUsuario.NomeUsuarioJaCadastrado), Times.Once);
            mocker.Verify<IRepositorioDeUsuarios>(r => r.Editar(It.IsAny<Usuario>()), Times.Never);
            mocker.Verify<IRepositorioDeUsuarios>(r => r.SalvarAlteracoesAsync(), Times.Never);

            #endregion
        }
    }
}

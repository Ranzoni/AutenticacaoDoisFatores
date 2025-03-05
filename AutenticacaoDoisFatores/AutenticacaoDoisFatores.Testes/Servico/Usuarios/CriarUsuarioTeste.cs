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
    public class CriarUsuarioTeste
    {
        private readonly IMapper _mapeador;
        private readonly AutoMocker _mocker = new();
        private readonly Faker _faker = new();
        
        public CriarUsuarioTeste()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<MapeadorDeUsuario>();
            });
            _mapeador = config.CreateMapper();
        }

        [Fact]
        internal async Task DeveCriarUsuario()
        {
            #region Preparação do teste

            var nome = _faker.Person.FullName;
            var nomeUsuario = _faker.Person.UserName;
            var email = _faker.Person.Email;

            var novoUsuario = ConstrutorDeUsuariosTeste
                .RetornarConstrutorDeNovo(nome: nome, nomeUsuario: nomeUsuario, email: email)
                .Construir();

            _mocker.CreateInstance<DominioDeUsuarios>();
            _mocker.Use(_mapeador);

            var servico = _mocker.CreateInstance<CriarUsuario>();

            #endregion

            var usuarioCadastrado = await servico.CriarAsync(novoUsuario);

            #region Verificação do teste

            Assert.NotNull(usuarioCadastrado);
            Assert.Equal(nome, usuarioCadastrado.Nome);
            Assert.Equal(nomeUsuario, usuarioCadastrado.NomeUsuario);
            Assert.Equal(email, usuarioCadastrado.Email);
            Assert.False(usuarioCadastrado.Ativo);
            _mocker.Verify<IRepositorioDeUsuarios>(r => r.Adicionar(It.IsAny<Usuario>()), Times.Once);
            _mocker.Verify<IRepositorioDeUsuarios>(r => r.SalvarAlteracoesAsync(), Times.Once);

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
                .Construir();

            _mocker.CreateInstance<DominioDeUsuarios>();
            _mocker.Use(_mapeador);

            _mocker.GetMock<INotificador>().Setup(n => n.ExisteMensagem()).Returns(true);

            var servico = _mocker.CreateInstance<CriarUsuario>();

            #endregion

            var usuarioCadastrado = await servico.CriarAsync(novoUsuario);

            #region Verificação do teste

            Assert.Null(usuarioCadastrado);
            _mocker.Verify<IRepositorioDeUsuarios>(r => r.Adicionar(It.IsAny<Usuario>()), Times.Never);
            _mocker.Verify<IRepositorioDeUsuarios>(r => r.SalvarAlteracoesAsync(), Times.Never);
            _mocker.Verify<INotificador>(n => n.AddMensagem(MensagensValidacaoUsuario.NomeInvalido), Times.Once);

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
                .Construir();

            _mocker.CreateInstance<DominioDeUsuarios>();
            _mocker.Use(_mapeador);

            _mocker.GetMock<INotificador>().Setup(n => n.ExisteMensagem()).Returns(true);

            var servico = _mocker.CreateInstance<CriarUsuario>();

            #endregion

            var usuarioCadastrado = await servico.CriarAsync(novoUsuario);

            #region Verificação do teste

            Assert.Null(usuarioCadastrado);
            _mocker.Verify<IRepositorioDeUsuarios>(r => r.Adicionar(It.IsAny<Usuario>()), Times.Never);
            _mocker.Verify<IRepositorioDeUsuarios>(r => r.SalvarAlteracoesAsync(), Times.Never);
            _mocker.Verify<INotificador>(n => n.AddMensagem(MensagensValidacaoUsuario.NomeUsuarioInvalido), Times.Once);

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
                .Construir();

            _mocker.CreateInstance<DominioDeUsuarios>();
            _mocker.Use(_mapeador);

            _mocker.GetMock<INotificador>().Setup(n => n.ExisteMensagem()).Returns(true);

            var servico = _mocker.CreateInstance<CriarUsuario>();

            #endregion

            var usuarioCadastrado = await servico.CriarAsync(novoUsuario);

            #region Verificação do teste

            Assert.Null(usuarioCadastrado);
            _mocker.Verify<IRepositorioDeUsuarios>(r => r.Adicionar(It.IsAny<Usuario>()), Times.Never);
            _mocker.Verify<IRepositorioDeUsuarios>(r => r.SalvarAlteracoesAsync(), Times.Never);
            _mocker.Verify<INotificador>(n => n.AddMensagem(MensagensValidacaoUsuario.EmailInvalido), Times.Once);

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
                .Construir();

            _mocker.CreateInstance<DominioDeUsuarios>();
            _mocker.Use(_mapeador);

            _mocker.GetMock<INotificador>().Setup(n => n.ExisteMensagem()).Returns(true);

            var servico = _mocker.CreateInstance<CriarUsuario>();

            #endregion

            var usuarioCadastrado = await servico.CriarAsync(novoUsuario);

            #region Verificação do teste

            Assert.Null(usuarioCadastrado);
            _mocker.Verify<IRepositorioDeUsuarios>(r => r.Adicionar(It.IsAny<Usuario>()), Times.Never);
            _mocker.Verify<IRepositorioDeUsuarios>(r => r.SalvarAlteracoesAsync(), Times.Never);
            _mocker.Verify<INotificador>(n => n.AddMensagem(MensagensValidacaoUsuario.SenhaInvalida), Times.Once);

            #endregion
        }

        [Fact]
        internal async Task NaoDeveCriarUsuarioQuandoNomeUsuarioJaCadastrado()
        {
            #region Preparação do teste

            var novoUsuario = ConstrutorDeUsuariosTeste
                .RetornarConstrutorDeNovo()
                .Construir();

            _mocker.CreateInstance<DominioDeUsuarios>();
            _mocker.GetMock<IRepositorioDeUsuarios>().Setup(r => r.ExisteNomeUsuarioAsync(novoUsuario.NomeUsuario, It.IsAny<Guid?>())).ReturnsAsync(true);
            _mocker.GetMock<INotificador>().Setup(n => n.ExisteMensagem()).Returns(true);

            var servico = _mocker.CreateInstance<CriarUsuario>();

            #endregion

            var usuarioCadastrado = await servico.CriarAsync(novoUsuario);

            #region Verificação do teste

            Assert.Null(usuarioCadastrado);
            _mocker.Verify<IRepositorioDeUsuarios>(r => r.Adicionar(It.IsAny<Usuario>()), Times.Never);
            _mocker.Verify<IRepositorioDeUsuarios>(r => r.SalvarAlteracoesAsync(), Times.Never);
            _mocker.Verify<INotificador>(n => n.AddMensagem(MensagensValidacaoUsuario.NomeUsuarioJaCadastrado), Times.Once);

            #endregion
        }

        [Fact]
        internal async Task NaoDeveCriarUsuarioQuandoEmailJaCadastrado()
        {
            #region Preparação do teste

            var novoUsuario = ConstrutorDeUsuariosTeste
                .RetornarConstrutorDeNovo()
                .Construir();

            _mocker.CreateInstance<DominioDeUsuarios>();
            _mocker.GetMock<IRepositorioDeUsuarios>().Setup(r => r.ExisteEmailAsync(novoUsuario.Email, It.IsAny<Guid?>())).ReturnsAsync(true);
            _mocker.GetMock<INotificador>().Setup(n => n.ExisteMensagem()).Returns(true);

            var servico = _mocker.CreateInstance<CriarUsuario>();

            #endregion

            var usuarioCadastrado = await servico.CriarAsync(novoUsuario);

            #region Verificação do teste

            Assert.Null(usuarioCadastrado);
            _mocker.Verify<IRepositorioDeUsuarios>(r => r.Adicionar(It.IsAny<Usuario>()), Times.Never);
            _mocker.Verify<IRepositorioDeUsuarios>(r => r.SalvarAlteracoesAsync(), Times.Never);
            _mocker.Verify<INotificador>(n => n.AddMensagem(MensagensValidacaoUsuario.EmailJaCadastrado), Times.Once);

            #endregion
        }
    }
}

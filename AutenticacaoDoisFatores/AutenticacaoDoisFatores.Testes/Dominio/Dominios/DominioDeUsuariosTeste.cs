using AutenticacaoDoisFatores.Dominio.Compartilhados;
using AutenticacaoDoisFatores.Dominio.Compartilhados.Mensagens;
using AutenticacaoDoisFatores.Dominio.Dominios;
using AutenticacaoDoisFatores.Dominio.Entidades;
using AutenticacaoDoisFatores.Dominio.Excecoes;
using AutenticacaoDoisFatores.Dominio.Filtros;
using AutenticacaoDoisFatores.Dominio.Repositorios;
using AutenticacaoDoisFatores.Testes.Compartilhados;
using Bogus;
using Moq;
using Moq.AutoMock;

namespace AutenticacaoDoisFatores.Testes.Dominio.Dominios
{
    public class DominioDeUsuariosTeste
    {
        private readonly AutoMocker _mocker = new();
        private readonly Faker _faker = new();

        #region Teste de cadastro

        [Fact]
        internal async Task DeveCriarUsuario()
        {
            #region Preparação do teste

            var dominio = _mocker.CreateInstance<UserDomain>();

            var usuarioParaCriar = ConstrutorDeUsuariosTeste
                .RetornarConstrutor()
                .BuildNew();

            #endregion

            await dominio.CriarAsync(usuarioParaCriar);

            #region Verificação do teste

            _mocker.Verify<IUserRepository>(r => r.Adicionar(usuarioParaCriar), Times.Once);
            _mocker.Verify<IUserRepository>(r => r.SaveChangesAsync(), Times.Once);

            #endregion
        }

        [Fact]
        internal async Task DeveCriarUsuarioComDominio()
        {
            #region Preparação do teste

            var dominio = _mocker.CreateInstance<UserDomain>();

            var usuarioParaCriar = ConstrutorDeUsuariosTeste
                .RetornarConstrutor()
                .BuildNew();

            var nomeDominio = _faker.Internet.DomainName();

            #endregion

            await dominio.CriarUsuarioComDominioAsync(usuarioParaCriar, nomeDominio);

            #region Verificação do teste

            _mocker.Verify<IUserRepository>(r => r.Adicionar(usuarioParaCriar, nomeDominio), Times.Once);
            _mocker.Verify<IUserRepository>(r => r.SaveChangesAsync(), Times.Once);

            #endregion
        }

        [Fact]
        internal async Task DeveRetornarExcecaoAoCriarUsuarioComNomeUsuarioJaExistente()
        {
            #region Preparação do teste

            var dominio = _mocker.CreateInstance<UserDomain>();

            var usuarioParaCriar = ConstrutorDeUsuariosTeste
                .RetornarConstrutor()
                .BuildNew();

            _mocker.GetMock<IUserRepository>().Setup(r => r.ExisteNomeUsuarioAsync(usuarioParaCriar.NomeUsuario, It.IsAny<Guid>())).ReturnsAsync(true);

            #endregion

            var excecao = await Assert.ThrowsAsync<UserException>(() => dominio.CriarAsync(usuarioParaCriar));

            #region Verificação do teste

            Assert.Equal(UserValidationMessages.UsernameAlreadyRegistered.Description(), excecao.Message);
            _mocker.Verify<IUserRepository>(r => r.Add(It.IsAny<User>()), Times.Never);
            _mocker.Verify<IUserRepository>(r => r.SaveChangesAsync(), Times.Never);

            #endregion
        }

        [Fact]
        internal async Task DeveRetornarExcecaoAoCriarUsuarioComDominioComNomeUsuarioJaExistente()
        {
            #region Preparação do teste

            var dominio = _mocker.CreateInstance<UserDomain>();

            var usuarioParaCriar = ConstrutorDeUsuariosTeste
                .RetornarConstrutor()
                .BuildNew();

            var nomeDominio = _faker.Internet.DomainName();

            _mocker.GetMock<IUserRepository>().Setup(r => r.ExisteNomeUsuarioAsync(usuarioParaCriar.NomeUsuario, nomeDominio)).ReturnsAsync(true);

            #endregion

            var excecao = await Assert.ThrowsAsync<UserException>(() => dominio.CriarUsuarioComDominioAsync(usuarioParaCriar, nomeDominio));

            #region Verificação do teste

            Assert.Equal(UserValidationMessages.UsernameAlreadyRegistered.Description(), excecao.Message);
            _mocker.Verify<IUserRepository>(r => r.Add(It.IsAny<User>(), It.IsAny<string>()), Times.Never);
            _mocker.Verify<IUserRepository>(r => r.SaveChangesAsync(), Times.Never);

            #endregion
        }

        [Fact]
        internal async Task DeveRetornarExcecaoAoCriarUsuarioComEmailJaExistente()
        {
            #region Preparação do teste

            var dominio = _mocker.CreateInstance<UserDomain>();

            var usuarioParaCriar = ConstrutorDeUsuariosTeste
                .RetornarConstrutor()
                .BuildNew();

            _mocker.GetMock<IUserRepository>().Setup(r => r.ExisteEmailAsync(usuarioParaCriar.Email, It.IsAny<Guid>())).ReturnsAsync(true);

            #endregion

            var excecao = await Assert.ThrowsAsync<UserException>(() => dominio.CriarAsync(usuarioParaCriar));

            #region Verificação do teste

            Assert.Equal(UserValidationMessages.EmailAlreadyRegistered.Description(), excecao.Message);
            _mocker.Verify<IUserRepository>(r => r.Add(It.IsAny<User>()), Times.Never);
            _mocker.Verify<IUserRepository>(r => r.SaveChangesAsync(), Times.Never);

            #endregion
        }

        [Fact]
        internal async Task DeveRetornarExcecaoAoCriarUsuarioComDominioComEmailJaExistente()
        {
            #region Preparação do teste

            var dominio = _mocker.CreateInstance<UserDomain>();

            var usuarioParaCriar = ConstrutorDeUsuariosTeste
                .RetornarConstrutor()
                .BuildNew();

            var nomeDominio = _faker.Internet.DomainName();

            _mocker.GetMock<IUserRepository>().Setup(r => r.ExisteEmailAsync(usuarioParaCriar.Email, nomeDominio)).ReturnsAsync(true);

            #endregion

            var excecao = await Assert.ThrowsAsync<UserException>(() => dominio.CriarUsuarioComDominioAsync(usuarioParaCriar, nomeDominio));

            #region Verificação do teste

            Assert.Equal(UserValidationMessages.EmailAlreadyRegistered.Description(), excecao.Message);
            _mocker.Verify<IUserRepository>(r => r.Add(It.IsAny<User>(), It.IsAny<string>()), Times.Never);
            _mocker.Verify<IUserRepository>(r => r.SaveChangesAsync(), Times.Never);

            #endregion
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        internal async Task DeveRetornarSeExisteNomeUsuarioJaCadastrado(bool resultadoEsperado)
        {
            #region Preparação do teste

            var dominio = _mocker.CreateInstance<UserDomain>();

            var nomeUsuarioParaTeste = _faker.Person.UserName;

            _mocker.GetMock<IUserRepository>().Setup(r => r.UsernameExistsAsync(nomeUsuarioParaTeste, It.IsAny<Guid?>())).ReturnsAsync(resultadoEsperado);

            #endregion

            var existeNomeUsuario = await dominio.UsernameExistsAsync(nomeUsuarioParaTeste);

            #region Verificação do teste

            Assert.Equal(resultadoEsperado, existeNomeUsuario);
            _mocker.Verify<IUserRepository>(r => r.UsernameExistsAsync(nomeUsuarioParaTeste, It.IsAny<Guid?>()), Times.Once);

            #endregion
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        internal async Task DeveRetornarSeExisteEmailJaCadastrado(bool resultadoEsperado)
        {
            #region Preparação do teste

            var dominio = _mocker.CreateInstance<UserDomain>();

            var nomeUsuarioParaTeste = _faker.Person.Email;

            _mocker.GetMock<IUserRepository>().Setup(r => r.EmailExistsAsync(nomeUsuarioParaTeste, It.IsAny<Guid?>())).ReturnsAsync(resultadoEsperado);

            #endregion

            var existeEmail = await dominio.EmailExistsAsync(nomeUsuarioParaTeste);

            #region Verificação do teste

            Assert.Equal(resultadoEsperado, existeEmail);
            _mocker.Verify<IUserRepository>(r => r.EmailExistsAsync(nomeUsuarioParaTeste, It.IsAny<Guid?>()), Times.Once);

            #endregion
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        internal async Task DeveRetornarSeUsuarioEhAdmin(bool resultadoEsperado)
        {
            #region Preparação do teste

            var idUsuario = Guid.NewGuid();

            var dominio = _mocker.CreateInstance<UserDomain>();

            _mocker.GetMock<IUserRepository>().Setup(r => r.IsAdminAsync(idUsuario)).ReturnsAsync(resultadoEsperado);

            #endregion

            var retorno = await dominio.IsAdminAsync(idUsuario);

            #region Verificação do teste

            Assert.Equal(resultadoEsperado, retorno);
            _mocker.Verify<IUserRepository>(r => r.IsAdminAsync(idUsuario), Times.Once);

            #endregion
        }

        #endregion

        #region Teste de alteração

        [Fact]
        internal async Task DeveAlterarUsuario()
        {
            #region Preparação do teste

            var dominio = _mocker.CreateInstance<UserDomain>();

            var usuarioParaEditar = ConstrutorDeUsuariosTeste
                .RetornarConstrutor()
                .BuildNew();

            _mocker.GetMock<IUserRepository>().Setup(r => r.BuscarUnicoAsync(usuarioParaEditar.Id)).ReturnsAsync(usuarioParaEditar);

            #endregion

            await dominio.AlterarAsync(usuarioParaEditar);

            #region Verificação do teste

            _mocker.Verify<IUserRepository>(r => r.Editar(usuarioParaEditar), Times.Once);
            _mocker.Verify<IUserRepository>(r => r.SaveChangesAsync(), Times.Once);

            #endregion
        }

        [Fact]
        internal async Task DeveRetornarExcecaoAoAlterarUsuarioComNomeUsuarioJaExistente()
        {
            #region Preparação do teste

            var dominio = _mocker.CreateInstance<UserDomain>();

            var usuarioParaAlterar = ConstrutorDeUsuariosTeste
                .RetornarConstrutor()
                .Build();

            _mocker.GetMock<IUserRepository>().Setup(r => r.ExisteNomeUsuarioAsync(usuarioParaAlterar.NomeUsuario, usuarioParaAlterar.Id)).ReturnsAsync(true);

            #endregion

            var excecao = await Assert.ThrowsAsync<UserException>(() => dominio.AlterarAsync(usuarioParaAlterar));

            #region Verificação do teste

            Assert.Equal(UserValidationMessages.UsernameAlreadyRegistered.Description(), excecao.Message);
            _mocker.Verify<IUserRepository>(r => r.Update(It.IsAny<User>()), Times.Never);
            _mocker.Verify<IUserRepository>(r => r.SaveChangesAsync(), Times.Never);

            #endregion
        }

        [Fact]
        internal async Task DeveRetornarExcecaoAoAlterarUsuarioComEmailJaExistente()
        {
            #region Preparação do teste

            var dominio = _mocker.CreateInstance<UserDomain>();

            var usuarioParaAlterar = ConstrutorDeUsuariosTeste
                .RetornarConstrutor()
                .Build();

            _mocker.GetMock<IUserRepository>().Setup(r => r.ExisteEmailAsync(usuarioParaAlterar.Email, usuarioParaAlterar.Id)).ReturnsAsync(true);

            #endregion

            var excecao = await Assert.ThrowsAsync<UserException>(() => dominio.AlterarAsync(usuarioParaAlterar));

            #region Verificação do teste

            Assert.Equal(UserValidationMessages.EmailAlreadyRegistered.Description(), excecao.Message);
            _mocker.Verify<IUserRepository>(r => r.Update(It.IsAny<User>()), Times.Never);
            _mocker.Verify<IUserRepository>(r => r.SaveChangesAsync(), Times.Never);

            #endregion
        }

        #endregion

        #region Teste de busca

        [Fact]
        internal async Task DeveBuscarUnicoUsuario()
        {
            #region Preparação do teste

            var idUsuario = Guid.NewGuid();
            var dominio = _mocker.CreateInstance<UserDomain>();

            var usuarioCadastrado = ConstrutorDeUsuariosTeste
                .RetornarConstrutor()
                .Build();

            _mocker.GetMock<IUserRepository>().Setup(r => r.GetByIdAsync(idUsuario)).ReturnsAsync(usuarioCadastrado);

            #endregion

            var usuario = await dominio.GetByIdAsync(idUsuario);

            #region Verificação do teste

            Assert.NotNull(usuario);
            Assert.Equal(usuarioCadastrado, usuario);

            #endregion
        }

        [Fact]
        internal async Task DeveRetornarNuloAoBuscarUnicoUsuarioNaoExistente()
        {
            #region Preparação do teste

            var idUsuario = Guid.NewGuid();
            var dominio = _mocker.CreateInstance<UserDomain>();

            #endregion

            var usuario = await dominio.GetByIdAsync(idUsuario);

            #region Verificação do teste

            Assert.Null(usuario);

            #endregion
        }

        [Fact]
        internal async Task DeveBuscarVarios()
        {
            #region Preparação do teste

            var mocker = new AutoMocker();

            var filtros = new UserFilter();
            var maximoRegistros = _faker.Random.Int(2, filtros.Quantity);

            var listaDeUsuarios = GerarVarios(maximoRegistros, ativo: true);

            var maximoPaginacao = _faker.Random.Int(1, maximoRegistros);

            var dominio = mocker.CreateInstance<UserDomain>();
            mocker.GetMock<IUserRepository>().Setup(r => r.GetAllAsync(filtros)).ReturnsAsync(listaDeUsuarios);

            #endregion

            var resposta = await dominio.BuscarVariosAsync(filtros);

            #region Verificação do teste

            Assert.NotNull(resposta);
            Assert.Equal(listaDeUsuarios, resposta);
            mocker.Verify<IUserRepository>(r => r.GetAllAsync(filtros), Times.Once);

            #endregion
        }

        [Fact]
        internal async Task DeveRetornarListaVaziaAoBuscarVariosQueNaoEncontrou()
        {
            #region Preparação do teste

            var mocker = new AutoMocker();

            var filtros = new UserFilter();

            var dominio = mocker.CreateInstance<UserDomain>();

            #endregion

            var resposta = await dominio.BuscarVariosAsync(filtros);

            #region Verificação do teste

            Assert.Empty(resposta);
            mocker.Verify<IUserRepository>(r => r.GetAllAsync(filtros), Times.Once);

            #endregion
        }

        [Fact]
        internal async Task DeveBuscarPorEmail()
        {
            #region Preparação do teste

            var mocker = new AutoMocker();

            var email = _faker.Person.Email;
            var usuario = ConstrutorDeUsuariosTeste
                .RetornarConstrutor(email: email)
                .Build();

            var dominio = mocker.CreateInstance<UserDomain>();
            mocker.GetMock<IUserRepository>().Setup(r => r.GetByEmailAsync(email)).ReturnsAsync(usuario);

            #endregion

            var resposta = await dominio.GetByEmailAsync(email);

            #region Verificação do teste

            Assert.NotNull(resposta);
            Assert.Equal(usuario, resposta);
            mocker.Verify<IUserRepository>(r => r.GetByEmailAsync(email), Times.Once);

            #endregion
        }

        [Fact]
        internal async Task DeveRetornarNuloBuscarPorEmailClienteInexistente()
        {
            #region Preparação do teste

            var mocker = new AutoMocker();

            var email = _faker.Person.Email;

            var dominio = mocker.CreateInstance<UserDomain>();

            #endregion

            var resposta = await dominio.GetByEmailAsync(email);

            #region Verificação do teste

            Assert.Null(resposta);
            mocker.Verify<IUserRepository>(r => r.GetByEmailAsync(email), Times.Once);

            #endregion
        }

        [Fact]
        internal async Task DeveBuscarPorNomeUsuario()
        {
            #region Preparação do teste

            var mocker = new AutoMocker();

            var nomeUsuario = "user_12345@";
            var usuario = ConstrutorDeUsuariosTeste
                .RetornarConstrutor(nomeUsuario: nomeUsuario)
                .Build();

            var dominio = mocker.CreateInstance<UserDomain>();
            mocker.GetMock<IUserRepository>().Setup(r => r.GetByUsernameAsync(nomeUsuario)).ReturnsAsync(usuario);

            #endregion

            var resposta = await dominio.GetByUsernameAsync(nomeUsuario);

            #region Verificação do teste

            Assert.NotNull(resposta);
            Assert.Equal(usuario, resposta);
            mocker.Verify<IUserRepository>(r => r.GetByUsernameAsync(nomeUsuario), Times.Once);

            #endregion
        }

        [Fact]
        internal async Task DeveRetornarNuloBuscarPorNomeUsuarioInexistente()
        {
            #region Preparação do teste

            var mocker = new AutoMocker();

            var nomeUsuario = "user_12345@";

            var dominio = mocker.CreateInstance<UserDomain>();

            #endregion

            var resposta = await dominio.GetByUsernameAsync(nomeUsuario);

            #region Verificação do teste

            Assert.Null(resposta);
            mocker.Verify<IUserRepository>(r => r.GetByUsernameAsync(nomeUsuario), Times.Once);

            #endregion
        }

        #endregion

        #region Teste de exclusão

        [Fact]
        internal async Task DeveExcluirUsuario()
        {
            #region Preparação do teste

            var dominio = _mocker.CreateInstance<UserDomain>();

            var usuarioParaExcluir = ConstrutorDeUsuariosTeste
                .RetornarConstrutor()
                .BuildNew();

            _mocker.GetMock<IUserRepository>().Setup(r => r.BuscarUnicoAsync(usuarioParaExcluir.Id)).ReturnsAsync(usuarioParaExcluir);

            #endregion

            await dominio.ExcluirUsuarioAsync(usuarioParaExcluir.Id);

            #region Verificação do teste

            _mocker.Verify<IUserRepository>(r => r.Excluir(usuarioParaExcluir), Times.Once);
            _mocker.Verify<IUserRepository>(r => r.SaveChangesAsync(), Times.Once);

            #endregion
        }

        [Fact]
        internal async Task DeveRetornarExcecaoAoTentarExcluirUsuarioInexistente()
        {
            #region Preparação do teste

            var dominio = _mocker.CreateInstance<UserDomain>();

            #endregion

            var excecao = await Assert.ThrowsAsync<UserException>(() => dominio.RemoveAsync(Guid.NewGuid()));

            #region Verificação do teste

            Assert.Equal(UserValidationMessages.UserNotFound.Description(), excecao.Message);
            _mocker.Verify<IUserRepository>(r => r.Remove(It.IsAny<User>()), Times.Never);
            _mocker.Verify<IUserRepository>(r => r.SaveChangesAsync(), Times.Never);

            #endregion
        }

        #endregion

        private static List<User> GerarVarios(int qtd, bool? ativo = null)
        {
            var listaDeUsuarios = new List<User>();

            for (var i = 1; i <= qtd; i++)
            {
                var usuario = ConstrutorDeUsuariosTeste
                    .RetornarConstrutor(ativo: ativo)
                    .Build();

                listaDeUsuarios.Add(usuario);
            }

            return listaDeUsuarios;
        }
    }
}

using AutenticacaoDoisFatores.Dominio.Compartilhados;
using AutenticacaoDoisFatores.Dominio.Compartilhados.Mensagens;
using AutenticacaoDoisFatores.Dominio.Compartilhados.Usuarios;
using AutenticacaoDoisFatores.Dominio.Excecoes;
using AutenticacaoDoisFatores.Testes.Compartilhados;
using Bogus;

namespace AutenticacaoDoisFatores.Testes.Dominio.Entidades
{
    public class UsuarioTeste
    {
        private readonly Faker _faker = new();

        [Theory]
        [InlineData("Teste.De.Senha_1")]
        [InlineData("2senha@Valida")]
        [InlineData("2senhaéValida")]
        [InlineData("S3nha.m")]
        [InlineData("Teste.de.senh@.que.é.muito.grand3.Teste.de.senh@.que.é.muito.grand3.Teste.de.senh@.que.é.muito.grand3.Teste.de.senh@.que.é.muito.grand3.Teste.de.senh@.que.é.muito.grand3.Teste.de.senh@.que.é.muito.grand3.Teste.de.senh@.que.é.muito.grand3.Teste.de.senh@.que")]
        internal void DeveInstanciarNovoUsuario(string senha)
        {
            #region Preparação do teste

            var nome = _faker.Person.FullName;
            var nomeUsuario = "teste_user_12398";
            var email = _faker.Person.Email;
            var celular = 5516993388778;

            #endregion

            var usuario = ConstrutorDeUsuariosTeste
                .RetornarConstrutor(nome: nome, nomeUsuario: nomeUsuario, email: email, senha: senha, celular: celular)
                .BuildNew();

            #region Verificação do teste

            Assert.Equal(nome, usuario.Nome);
            Assert.Equal(nomeUsuario, usuario.NomeUsuario);
            Assert.Equal(email, usuario.Email);
            Assert.Equal(senha, usuario.Senha);
            Assert.Equal(celular, usuario.Celular);

            #endregion
        }

        [Theory]
        [InlineData("")]
        [InlineData("         ")]
        [InlineData("ab")]
        [InlineData("Teste de nome grande Teste de nome grande Teste de ")]
        internal void NaoDeveInstanciarNovoUsuarioQuandoNomeEhInvalido(string nomeInvalido)
        {
            var excecao = Assert.Throws<UserException>
                (() => ConstrutorDeUsuariosTeste
                    .RetornarConstrutor(nome: nomeInvalido)
                    .BuildNew()
                );

            Assert.Equal(UserValidationMessages.InvalidName.Description(), excecao.Message);
        }

        [Theory]
        [InlineData("")]
        [InlineData("         ")]
        [InlineData("abcd")]
        [InlineData("Teste de nome grande ")]
        internal void NaoDeveInstanciarNovoUsuarioQuandoNomeUsuarioEhInvalido(string nomeUsuarioInvalido)
        {
            var excecao = Assert.Throws<UserException>
                (() => ConstrutorDeUsuariosTeste
                    .RetornarConstrutor(nomeUsuario: nomeUsuarioInvalido)
                    .BuildNew()
                );

            Assert.Equal(UserValidationMessages.InvalidUsername.Description(), excecao.Message);
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
        internal void NaoDeveInstanciarNovoUsuarioQuandoEmailEhInvalido(string emailInvalido)
        {
            var excecao = Assert.Throws<UserException>
                (() => ConstrutorDeUsuariosTeste
                    .RetornarConstrutor(email: emailInvalido)
                    .BuildNew()
                );

            Assert.Equal(UserValidationMessages.InvalidEmail.Description(), excecao.Message);
        }

        [Theory]
        [InlineData("")]
        [InlineData("       ")]
        [InlineData("Teste.de.senh@.que.é.muito.grand3.Teste.de.senh@.que.é.muito.grand3.Teste.de.senh@.que.é.muito.grand3.Teste.de.senh@.que.é.muito.grand3.Teste.de.senh@.que.é.muito.grand3.Teste.de.senh@.que.é.muito.grand3.Teste.de.senh@.que.é.muito.grand3.Teste.de.senh@.que.")]
        internal void NaoDeveInstanciarNovoUsuarioQuandoSenhaEhInvalida(string senhaInvalida)
        {
            var excecao = Assert.Throws<UserException>
                (() => ConstrutorDeUsuariosTeste
                    .RetornarConstrutor(senha: senhaInvalida)
                    .BuildNew()
                );

            Assert.Equal(UserValidationMessages.InvalidPassword.Description(), excecao.Message);
        }

        [Fact]
        internal void DeveAlterarNome()
        {
            #region Preparação do teste

            var nomeAtual = "Fulano de Tal";
            var novoNome = _faker.Person.FullName;

            var usuario = ConstrutorDeUsuariosTeste
                .RetornarConstrutor(nome: nomeAtual)
                .Build();

            #endregion

            usuario.AlterarNome(novoNome);

            #region Verificação do teste

            Assert.Equal(novoNome, usuario.Nome);
            Assert.NotNull(usuario.DataAlteracao);

            #endregion
        }

        [Theory]
        [InlineData("")]
        [InlineData("         ")]
        [InlineData("ab")]
        [InlineData("Teste de nome grande Teste de nome grande Teste de ")]
        internal void NaoDeveAlterarParaNomeInvalido(string nomeInvalido)
        {
            #region Preparação do teste

            var nomeAtual = "Fulano de Tal";

            var usuario = ConstrutorDeUsuariosTeste
                .RetornarConstrutor(nome: nomeAtual)
                .Build();

            #endregion

            var excecao = Assert.Throws<UserException>(() => usuario.AlterarNome(nomeInvalido));

            #region Verificação do teste

            Assert.Equal(UserValidationMessages.InvalidName.Description(), excecao.Message);

            #endregion
        }

        [Fact]
        internal void DeveAlterarNomeUsuario()
        {
            #region Preparação do teste

            var nomeUsuarioAtual = "user_test_12345";
            var novoNomeUsuario = "user_test_54321";

            var usuario = ConstrutorDeUsuariosTeste
                .RetornarConstrutor(nomeUsuario: nomeUsuarioAtual)
                .Build();

            #endregion

            usuario.AlterarNomeUsuario(novoNomeUsuario);

            #region Verificação do teste

            Assert.Equal(novoNomeUsuario, usuario.NomeUsuario);
            Assert.NotNull(usuario.DataAlteracao);

            #endregion
        }

        [Theory]
        [InlineData("")]
        [InlineData("         ")]
        [InlineData("abcd")]
        [InlineData("Teste de nome grande ")]
        internal void NaoDeveAlterarParaNomeUsuarioInvalido(string nomeUsuarioInvalido)
        {
            #region Preparação do teste

            var nomeUsuarioAtual = "user_test_12345";

            var usuario = ConstrutorDeUsuariosTeste
                .RetornarConstrutor(nomeUsuario: nomeUsuarioAtual)
                .Build();

            #endregion

            var excecao = Assert.Throws<UserException>(() => usuario.AlterarNomeUsuario(nomeUsuarioInvalido));

            #region Verificação do teste

            Assert.Equal(UserValidationMessages.InvalidUsername.Description(), excecao.Message);

            #endregion
        }

        [Fact]
        internal void DeveAlterarEmail()
        {
            #region Preparação do teste

            var emailAtual = "fulano@test.com";
            var novoEmail = _faker.Person.Email;

            var usuario = ConstrutorDeUsuariosTeste
                .RetornarConstrutor(email: emailAtual)
                .Build();

            #endregion

            usuario.AlterarEmail(novoEmail);

            #region Verificação do teste

            Assert.Equal(novoEmail, usuario.Email);
            Assert.NotNull(usuario.DataAlteracao);

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
        internal void NaoDeveAlterarParaEmailInvalido(string emailInvalido)
        {
            #region Preparação do teste

            var emailAtual = "fulano@test.com";

            var usuario = ConstrutorDeUsuariosTeste
                .RetornarConstrutor(email: emailAtual)
                .Build();

            #endregion

            var excecao = Assert.Throws<UserException>(() => usuario.AlterarEmail(emailInvalido));

            #region Verificação do teste

            Assert.Equal(UserValidationMessages.InvalidEmail.Description(), excecao.Message);

            #endregion
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        internal void DeveAtivarEDesativarUsuario(bool ativar)
        {
            #region Preparação do teste

            var usuario = ConstrutorDeUsuariosTeste
                .RetornarConstrutor(ativo: !ativar)
                .Build();

            #endregion

            usuario.Ativar(ativar);

            #region Verificação do teste

            Assert.Equal(ativar, usuario.Ativo);
            Assert.NotNull(usuario.DataAlteracao);

            #endregion
        }

        [Fact]
        internal void DeveAlterarSenhaUsuario()
        {
            #region Preparação do teste

            var senhaAtual = "Senh4#Atu@al";
            var novaSenha = "Nov@Senh4_!";

            var usuario = ConstrutorDeUsuariosTeste
                .RetornarConstrutor(senha: senhaAtual)
                .Build();

            #endregion

            usuario.AlterarSenha(novaSenha);

            #region Verificação do teste

            Assert.Equal(novaSenha, usuario.Senha);
            Assert.NotNull(usuario.DataAlteracao);

            #endregion
        }

        [Theory]
        [InlineData("")]
        [InlineData("       ")]
        [InlineData("Teste.de.senh@.que.é.muito.grand3.Teste.de.senh@.que.é.muito.grand3.Teste.de.senh@.que.é.muito.grand3.Teste.de.senh@.que.é.muito.grand3.Teste.de.senh@.que.é.muito.grand3.Teste.de.senh@.que.é.muito.grand3.Teste.de.senh@.que.é.muito.grand3.Teste.de.senh@.que.")]
        internal void NaoDeveAlterarParaSenhaInvalida(string senhaInvalida)
        {
            #region Preparação do teste

            var senhaAtual = "Senh4#Atu@al";

            var usuario = ConstrutorDeUsuariosTeste
                .RetornarConstrutor(senha: senhaAtual)
                .Build();

            #endregion

            var excecao = Assert.Throws<UserException>(() => usuario.AlterarSenha(senhaInvalida));

            #region Verificação do teste

            Assert.Equal(UserValidationMessages.InvalidPassword.Description(), excecao.Message);

            #endregion
        }

        [Fact]
        internal void DeveAlterarCelularUsuario()
        {
            #region Preparação do teste

            var numCelularAtual = 55016990011001;
            var novoNumCelular = 16990222200;

            var usuario = ConstrutorDeUsuariosTeste
                .RetornarConstrutor(celular: numCelularAtual)
                .Build();

            #endregion

            usuario.AlterarCelular(novoNumCelular);

            #region Verificação do teste

            Assert.Equal(novoNumCelular, usuario.Celular);
            Assert.NotNull(usuario.DataAlteracao);

            #endregion
        }

        [Theory]
        [InlineData(-99999)]
        [InlineData(1)]
        [InlineData(10092)]
        internal void NaoDeveAlterarCelularQuandoNumeroEhInvalido(long novoNumCelularInvalido)
        {
            #region Preparação do teste

            var numCelularAtual = 55016990011001;

            var usuario = ConstrutorDeUsuariosTeste
                .RetornarConstrutor(celular: numCelularAtual)
                .Build();

            #endregion

            var excecao = Assert.Throws<UserException>(() => usuario.AlterarCelular(novoNumCelularInvalido));

            Assert.Equal(UserValidationMessages.InvalidPhone.Description(), excecao.Message);
        }

        [Fact]
        internal void DeveAtualizarDataUltimoAcessoUsuario()
        {
            #region Preparação do teste

            var usuario = ConstrutorDeUsuariosTeste
                .RetornarConstrutor()
                .Build();

            #endregion

            usuario.AtualizarDataUltimoAcesso();

            #region Verificação do teste

            Assert.NotNull(usuario.DataUltimoAcesso);
            Assert.True(usuario.DataUltimoAcesso <= DateTime.Now);
            Assert.NotNull(usuario.DataAlteracao);

            #endregion
        }

        [Fact]
        internal void DeveAlterarTipoDeAutenticacaoEmDoisFatoresParaUsuario()
        {
            #region Preparação do teste

            var tipoDeAutenticacao = _faker.Random.Enum<AuthType>();

            var usuario = ConstrutorDeUsuariosTeste
                .RetornarConstrutor()
                .Build();

            #endregion

            usuario.ConfigurarTipoDeAutenticacao(tipoDeAutenticacao);

            #region Verificação do teste

            Assert.Equal(tipoDeAutenticacao, usuario.TipoDeAutenticacao);
            Assert.NotNull(usuario.DataAlteracao);

            #endregion
        }

        [Fact]
        internal void DeveRetornarVerdadeiroQuandoExisteTipoDeAutenticacaoConfigurado()
        {
            var usuario = ConstrutorDeUsuariosTeste
                .RetornarConstrutor(tipoDeAutenticacao: AuthType.Email)
                .Build();

            var valor = usuario.ExisteTipoDeAutenticacaoConfigurado();

            Assert.True(valor);
        }

        [Fact]
        internal void DeveRetornarFalsoQuandoNaoExisteTipoDeAutenticacaoConfigurado()
        {
            var usuario = ConstrutorDeUsuariosTeste
                .RetornarConstrutor()
                .Build();

            var valor = usuario.ExisteTipoDeAutenticacaoConfigurado();

            Assert.False(valor);
        }

        [Theory]
        [InlineData("")]
        [InlineData("       ")]
        [InlineData("012345678901")]
        [InlineData("0123456789012345678")]
        internal void NaoDeveInstanciarUsuarioiCadastradoParaChaveSecretaInvalida(string chaveSecretaInvalida)
        {
            var usuario = ConstrutorDeUsuariosTeste
                .RetornarConstrutor(chaveSecreta: chaveSecretaInvalida);
            
            var excecao = Assert.Throws<UserException>(usuario.Build);

            Assert.Equal(UserValidationMessages.InvalidSecretKey.Description(), excecao.Message);
        }
    }
}

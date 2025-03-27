using AutenticacaoDoisFatores.Dominio.Compartilhados;
using AutenticacaoDoisFatores.Dominio.Compartilhados.Mensagens;
using AutenticacaoDoisFatores.Dominio.Excecoes;
using AutenticacaoDoisFatores.Testes.Compartilhados;
using Bogus;

namespace AutenticacaoDoisFatores.Testes.Dominio.Entidades
{
    public class UsuarioTeste
    {
        [Theory]
        [InlineData("Teste.De.Senha_1")]
        [InlineData("2senha@Valida")]
        [InlineData("2senhaéValida")]
        [InlineData("S3nha.m")]
        [InlineData("Teste.de.senh@.que.é.muito.grand3.Teste.de.senh@.que.é.muito.grand3.Teste.de.senh@.que.é.muito.grand3.Teste.de.senh@.que.é.muito.grand3.Teste.de.senh@.que.é.muito.grand3.Teste.de.senh@.que.é.muito.grand3.Teste.de.senh@.que.é.muito.grand3.Teste.de.senh@.que")]
        internal void DeveInstanciarNovoUsuario(string senha)
        {
            #region Preparação do teste

            var faker = new Faker();

            var nome = faker.Person.FullName;
            var nomeUsuario = faker.Person.UserName;
            var email = faker.Person.Email;

            #endregion

            var usuario = ConstrutorDeUsuariosTeste
                .RetornarConstrutor(nome: nome, nomeUsuario: nomeUsuario, email: email, senha: senha)
                .ConstruirNovo();

            #region Verificação do teste

            Assert.Equal(nome, usuario.Nome);
            Assert.Equal(nomeUsuario, usuario.NomeUsuario);
            Assert.Equal(email, usuario.Email);
            Assert.Equal(senha, usuario.Senha);

            #endregion
        }

        [Theory]
        [InlineData("")]
        [InlineData("         ")]
        [InlineData("ab")]
        [InlineData("Teste de nome grande Teste de nome grande Teste de ")]
        internal void NaoDeveInstanciarNovoUsuarioQuandoNomeEhInvalido(string nomeInvalido)
        {
            var excecao = Assert.Throws<ExcecoesUsuario>
                (() => ConstrutorDeUsuariosTeste
                    .RetornarConstrutor(nome: nomeInvalido)
                    .ConstruirNovo()
                );

            Assert.Equal(MensagensValidacaoUsuario.NomeInvalido.Descricao(), excecao.Message);
        }

        [Theory]
        [InlineData("")]
        [InlineData("         ")]
        [InlineData("abcd")]
        [InlineData("Teste de nome grande ")]
        internal void NaoDeveInstanciarNovoUsuarioQuandoNomeUsuarioEhInvalido(string nomeUsuarioInvalido)
        {
            var excecao = Assert.Throws<ExcecoesUsuario>
                (() => ConstrutorDeUsuariosTeste
                    .RetornarConstrutor(nomeUsuario: nomeUsuarioInvalido)
                    .ConstruirNovo()
                );

            Assert.Equal(MensagensValidacaoUsuario.NomeUsuarioInvalido.Descricao(), excecao.Message);
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
            var excecao = Assert.Throws<ExcecoesUsuario>
                (() => ConstrutorDeUsuariosTeste
                    .RetornarConstrutor(email: emailInvalido)
                    .ConstruirNovo()
                );

            Assert.Equal(MensagensValidacaoUsuario.EmailInvalido.Descricao(), excecao.Message);
        }

        [Theory]
        [InlineData("")]
        [InlineData("       ")]
        [InlineData("Teste.de.senh@.que.é.muito.grand3.Teste.de.senh@.que.é.muito.grand3.Teste.de.senh@.que.é.muito.grand3.Teste.de.senh@.que.é.muito.grand3.Teste.de.senh@.que.é.muito.grand3.Teste.de.senh@.que.é.muito.grand3.Teste.de.senh@.que.é.muito.grand3.Teste.de.senh@.que.")]
        internal void NaoDeveInstanciarNovoUsuarioQuandoSenhaEhInvalida(string senhaInvalida)
        {
            var excecao = Assert.Throws<ExcecoesUsuario>
                (() => ConstrutorDeUsuariosTeste
                    .RetornarConstrutor(senha: senhaInvalida)
                    .ConstruirNovo()
                );

            Assert.Equal(MensagensValidacaoUsuario.SenhaInvalida.Descricao(), excecao.Message);
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        internal void DeveAtivarEDesativarUsuario(bool ativar)
        {
            #region Preparação do teste

            var usuario = ConstrutorDeUsuariosTeste
                .RetornarConstrutor(ativo: !ativar)
                .ConstruirCadastrado();

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
                .ConstruirCadastrado();

            #endregion

            usuario.AlterarSenha(novaSenha);

            #region Verificação do teste

            Assert.Equal(novaSenha, usuario.Senha);
            Assert.NotNull(usuario.DataAlteracao);

            #endregion
        }
    }
}

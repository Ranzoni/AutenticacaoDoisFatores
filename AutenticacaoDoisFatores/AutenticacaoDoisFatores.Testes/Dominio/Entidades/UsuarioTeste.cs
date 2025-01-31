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
        [InlineData("M@1or_senha_possivel_M@1or_senha_possivel_M@1or_se")]
        internal void DeveInstanciarNovoUsuario(string senha)
        {
            #region Preparação do teste

            var faker = new Faker();

            var nome = faker.Person.FullName;
            var nomeUsuario = faker.Person.UserName;
            var email = faker.Person.Email;

            #endregion

            var usuario = ConstrutorDeUsuariosTeste
                .RetornarConstrutorDeNovo(nome: nome, nomeUsuario: nomeUsuario, email: email, senha: senha)
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
                    .RetornarConstrutorDeNovo(nome: nomeInvalido)
                    .ConstruirNovo()
                );

            Assert.Equal(MensagensValidacaoUsuario.NomeInvalido.Descricao(), excecao.Message);
        }
    }
}

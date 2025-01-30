using AutenticacaoDoisFatores.Testes.Compartilhados;
using Bogus;

namespace AutenticacaoDoisFatores.Testes.Dominio.Entidades
{
    public class UsuarioTeste
    {
        [Fact]
        internal void DeveInstanciarNovoUsuario()
        {
            #region Preparação do teste

            var faker = new Faker();

            var nome = faker.Person.FullName;
            var nomeUsuario = faker.Person.UserName;
            var email = faker.Person.Email;
            var senha = faker.Random.AlphaNumeric(15);

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
    }
}

using AutenticacaoDoisFatores.Dominio.Construtores;
using Bogus;

namespace AutenticacaoDoisFatores.Testes.Compartilhados
{
    internal static class ConstrutorDeUsuariosTeste
    {
        private static readonly string _senhaParaTeste = "Teste.De.Senha_1";

        internal static ConstrutorDeUsuario RetornarConstrutorDeNovo(string? nome = null, string? nomeUsuario = null, string? email = null, string? senha = null)
        {
            var faker = new Faker();

            return new ConstrutorDeUsuario()
                .ComNome(nome ?? faker.Person.FullName)
                .ComNomeUsuario(nomeUsuario ?? faker.Person.UserName)
                .ComEmail(email ?? faker.Person.Email)
                .ComSenha(senha ?? _senhaParaTeste);
        }
    }
}

using AutenticacaoDoisFatores.Dominio.Compartilhados.Usuarios;
using AutenticacaoDoisFatores.Dominio.Construtores;
using AutenticacaoDoisFatores.Servico.Construtores;
using Bogus;

namespace AutenticacaoDoisFatores.Testes.Compartilhados
{
    internal static class ConstrutorDeUsuariosTeste
    {
        private static readonly string _senhaParaTeste = "Teste.De.Senha_1";

        internal static UserBuilder RetornarConstrutor
        (
            Guid? id = null,
            string? nome = null,
            string? nomeUsuario = null,
            string? email = null,
            string? senha = null,
            long? celular = null,
            bool? ativo = null,
            DateTime? dataUltimoAcesso = null,
            DateTime? dataCadastro = null,
            DateTime? dataAlteracao = null,
            bool? ehAdm = null,
            AuthType? tipoDeAutenticacao = null,
            string? chaveSecreta = null
        )
        {
            var faker = new Faker();

            return new UserBuilder()
                .WithId(id ?? Guid.NewGuid())
                .WithName(nome ?? faker.Person.FullName)
                .WithUserName(nomeUsuario ?? "teste_user_2010234")
                .WithEmail(email ?? faker.Person.Email)
                .WithPassword(senha ?? _senhaParaTeste)
                .WithPhone(celular)
                .WithActive(ativo ?? faker.Random.Bool())
                .WithLastAccess(dataUltimoAcesso)
                .WithCreatedAt(dataCadastro ?? faker.Date.Past())
                .WithUpdatedAt(dataAlteracao)
                .WithIsAdminFlag(ehAdm ?? false)
                .WithAuthType(tipoDeAutenticacao)
                .WithSecretKey(chaveSecreta ?? "01234567890123456789");
        }

        internal static NewUserBuilder RetornarConstrutorDeNovo(string? nome = null, string? nomeUsuario = null, string? email = null, string? senha = null, long? celular = null)
        {
            var faker = new Faker();

            return new NewUserBuilder()
                .WithName(nome ?? faker.Person.FullName)
                .ComUsername(nomeUsuario ?? "teste_user_2010234")
                .WithEmail(email ?? faker.Person.Email)
                .WithPassword(senha ?? "T3ste.de.Senh@")
                .WithPhone(celular);
        }

        internal static NewUserDataBuilder RetornarConstrutorDeNovosDados(string? nome = null, string? nomeUsuario = null, long? celular = null)
        {
            var faker = new Faker();

            return new NewUserDataBuilder()
                .WithName(nome ?? faker.Person.FullName)
                .WithUsername(nomeUsuario ?? "teste_user_2010234")
                .WithPhone(celular);
        }
    }
}

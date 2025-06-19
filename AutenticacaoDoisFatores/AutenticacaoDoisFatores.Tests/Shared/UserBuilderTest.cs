using AutenticacaoDoisFatores.Domain.Shared.Users;
using AutenticacaoDoisFatores.Domain.Builders;
using AutenticacaoDoisFatores.Service.Builders;
using Bogus;

namespace AutenticacaoDoisFatores.Tests.Shared
{
    internal static class UserBuilderTest
    {
        private static readonly string _fakePassword = "Teste.De.Senha_1";

        internal static UserBuilder GetBuilder
        (
            Guid? id = null,
            string? name = null,
            string? username = null,
            string? email = null,
            string? password = null,
            long? phone = null,
            bool? active = null,
            DateTime? lastAccess = null,
            DateTime? createdAt = null,
            DateTime? udpatedAt = null,
            bool? isAdmin = null,
            AuthType? authType = null,
            string? secretKey = null
        )
        {
            var faker = new Faker();

            return new UserBuilder()
                .WithId(id ?? Guid.NewGuid())
                .WithName(name ?? faker.Person.FullName)
                .WithUsername(username ?? "teste_user_2010234")
                .WithEmail(email ?? faker.Person.Email)
                .WithPassword(password ?? _fakePassword)
                .WithPhone(phone)
                .WithActive(active ?? faker.Random.Bool())
                .WithLastAccess(lastAccess)
                .WithCreatedAt(createdAt ?? faker.Date.Past())
                .WithUpdatedAt(udpatedAt)
                .WithIsAdminFlag(isAdmin ?? false)
                .WithAuthType(authType)
                .WithSecretKey(secretKey ?? "01234567890123456789");
        }

        internal static NewUserBuilder GetBuilderOfNew(string? name = null, string? username = null, string? email = null, string? password = null, long? phone = null)
        {
            var faker = new Faker();

            return new NewUserBuilder()
                .WithName(name ?? faker.Person.FullName)
                .ComUsername(username ?? "teste_user_2010234")
                .WithEmail(email ?? faker.Person.Email)
                .WithPassword(password ?? "T3ste.de.Senh@")
                .WithPhone(phone);
        }

        internal static NewUserDataBuilder GetBuilderOfNewData(string? name = null, string? username = null, long? phone = null)
        {
            var faker = new Faker();

            return new NewUserDataBuilder()
                .WithName(name ?? faker.Person.FullName)
                .WithUsername(username ?? "teste_user_2010234")
                .WithPhone(phone);
        }
    }
}

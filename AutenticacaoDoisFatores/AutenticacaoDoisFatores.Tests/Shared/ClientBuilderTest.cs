using AutenticacaoDoisFatores.Domain.Builders;
using AutenticacaoDoisFatores.Service.Builders;
using Bogus;

namespace AutenticacaoDoisFatores.Tests.Shared
{
    internal static class ClientBuilderTest
    {
        internal static ClientBuilder GetBuilder(Guid? id = null, string? name = null, string? email = null, string? domainName = null, string? accessKey = null, bool? active = null, DateTime? createdAt = null, DateTime? updatedAt = null)
        {
            var faker = new Faker();

            var builder = new ClientBuilder();
            builder
                .WithId(id ?? Guid.NewGuid())
                .WithName(name ?? faker.Company.CompanyName())
                .WithEmail(email ?? faker.Internet.Email())
                .WithDomainName(domainName ?? "dominio_cliente")
                .WithAccessKey(accessKey ?? faker.Random.AlphaNumeric(20))
                .WithActive(active ?? faker.Random.Bool())
                .WithCreatedAt(createdAt ?? faker.Date.Past())
                .WithUpdatedAt(updatedAt);

            return builder;
        }

        internal static NewClientBuilder GetBuilderOfNew(string? name = null, string? email = null, string? domainName = null, string? adminPassword = null)
        {
            var faker = new Faker();

            return new NewClientBuilder()
                .WithName(name ?? faker.Company.CompanyName())
                .WithEmail(email ?? faker.Internet.Email())
                .WithDomainName(domainName ?? "dominio_cliente")
                .WithAdminPassword(adminPassword ?? "T3ste.de.Senh@");
        }
    }
}

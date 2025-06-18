using AutenticacaoDoisFatores.Domain.Domains;
using Bogus;

namespace AutenticacaoDoisFatores.Tests.Domain.Domains
{
    public class EncryptTest
    {
        [Fact]
        internal void ShouldEncrypt()
        {
            var faker = new Faker();

            var value = faker.Internet.Password();

            var encryptedValue = Encrypt.EncryptWithSha512(value);

            Assert.NotEqual(value, encryptedValue);
        }
    }
}

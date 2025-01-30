using AutenticacaoDoisFatores.Dominio.Dominios;
using Bogus;

namespace AutenticacaoDoisFatores.Testes.Dominio.Dominios
{
    public class CriptografiaTeste
    {
        [Fact]
        internal void DeveCriptografar()
        {
            var faker = new Faker();

            var valor = faker.Internet.Password();

            var valorCriptografado = Criptografia.CriptografarComSha512(valor);

            Assert.NotEqual(valor, valorCriptografado);
        }
    }
}

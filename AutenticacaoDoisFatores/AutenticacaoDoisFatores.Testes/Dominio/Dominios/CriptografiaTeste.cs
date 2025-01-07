using AutenticacaoDoisFatores.Dominio.Construtores;
using Bogus;

namespace AutenticacaoDoisFatores.Testes.Dominio.Dominios
{
    public class CriptografiaTeste
    {
        private readonly Faker _faker = new();

        [Fact]
        internal void DeveCriptografar()
        {
            var criptografia = new ConstrutorDeCriptografia().ConstruirCriptografia();
            var valor = _faker.Internet.Password();

            var valorCriptografado = criptografia.Criptografar(valor);

            Assert.NotEqual(valor, valorCriptografado);
        }
    }
}

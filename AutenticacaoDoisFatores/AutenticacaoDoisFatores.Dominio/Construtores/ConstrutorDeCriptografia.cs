using AutenticacaoDoisFatores.Dominio.Dominios;
using System.Security.Cryptography;

namespace AutenticacaoDoisFatores.Dominio.Construtores
{
    public class ConstrutorDeCriptografia
    {
        private readonly HashAlgorithm _algoritmo = SHA512.Create();

        public Criptografia ConstruirCriptografia()
        {
            return new Criptografia(algoritmo: _algoritmo);
        }
    }
}

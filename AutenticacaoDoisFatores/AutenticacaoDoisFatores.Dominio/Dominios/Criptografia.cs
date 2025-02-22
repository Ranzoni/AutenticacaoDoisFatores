using System.Security.Cryptography;
using System.Text;

namespace AutenticacaoDoisFatores.Dominio.Dominios
{
    public static class Criptografia
    {
        public static string CriptografarComSha512(string valor)
        {
            return Criptografar(valor, SHA512.Create());
        }

        private static string Criptografar(string valor, HashAlgorithm algoritmo)
        {
            var valorEmBytes = Encoding.UTF8.GetBytes(valor);
            var valorCriptografado = algoritmo.ComputeHash(valorEmBytes);

            var sb = new StringBuilder();
            foreach (var caracter in valorCriptografado)
            {
                sb.Append(caracter.ToString("X2"));
            }

            return sb.ToString();
        }
    }
}

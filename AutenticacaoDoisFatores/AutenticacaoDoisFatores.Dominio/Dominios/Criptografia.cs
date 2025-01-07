using System.Security.Cryptography;
using System.Text;

namespace AutenticacaoDoisFatores.Dominio.Dominios
{
    public class Criptografia(HashAlgorithm algoritmo)
    {
        private readonly HashAlgorithm _algoritmo = algoritmo;

        public string Criptografar(string valor)
        {
            var valorEmBytes = Encoding.UTF8.GetBytes(valor);
            var valorCriptografado = _algoritmo.ComputeHash(valorEmBytes);

            var sb = new StringBuilder();
            foreach (var caracter in valorCriptografado)
            {
                sb.Append(caracter.ToString("X2"));
            }

            return sb.ToString();
        }
    }
}

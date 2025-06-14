using System.Security.Cryptography;
using System.Text;

namespace AutenticacaoDoisFatores.Domain.Domains
{
    public static class Encrypt
    {
        public static string EncryptWithSha512(string value)
        {
            return HashEncrypt(value, SHA512.Create());
        }

        private static string HashEncrypt(string value, HashAlgorithm algorithm)
        {
            var bytesValue = Encoding.UTF8.GetBytes(value);
            var encryptedValue = algorithm.ComputeHash(bytesValue);

            var sb = new StringBuilder();
            foreach (var character in encryptedValue)
                sb.Append(character.ToString("X2"));

            return sb.ToString();
        }
    }
}

using AutenticacaoDoisFatores.Domain.Shared;
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

        public static string AesEncrypt(string value)
        {
            if (value.IsNullOrEmptyOrWhiteSpaces())
                throw new ArgumentNullException(nameof(value));

            var (key, iv) = GetEncryptKeys();

            using var aesAlg = Aes.Create();
            aesAlg.Key = key;
            aesAlg.IV = iv;

            var encryptedValue = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

            using var msEncrypt = new MemoryStream();
            using var csEncrypt = new CryptoStream(msEncrypt, encryptedValue, CryptoStreamMode.Write);
            using (var swEncrypt = new StreamWriter(csEncrypt))
            {
                swEncrypt.Write(value);
            }
            return Convert.ToBase64String(msEncrypt.ToArray());
        }

        public static string AesDecrypt(string value)
        {
            if (value.IsNullOrEmptyOrWhiteSpaces())
                throw new ArgumentNullException(nameof(value));

            var (key, iv) = GetEncryptKeys();

            using var aesAlg = Aes.Create();
            aesAlg.Key = key;
            aesAlg.IV = iv;

            var decryptedValue = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

            using var msDecrypt = new MemoryStream(Convert.FromBase64String(value));
            using var csDecrypt = new CryptoStream(msDecrypt, decryptedValue, CryptoStreamMode.Read);
            using var srDecrypt = new StreamReader(csDecrypt);
            return srDecrypt.ReadToEnd();
        }

        private static (byte[] key, byte[] iv) GetEncryptKeys()
        {
            var secretKey = Environment.GetEnvironmentVariable("ADF_CHAVE_CRIPTOGRAFIA");
            if (secretKey is null || secretKey.IsNullOrEmptyOrWhiteSpaces())
                throw new ApplicationException("A chave de autenticação não foi encontrada.");

            return (Encoding.UTF8.GetBytes(secretKey), Encoding.UTF8.GetBytes(secretKey));
        }
    }
}

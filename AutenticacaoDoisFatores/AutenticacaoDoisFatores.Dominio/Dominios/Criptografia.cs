using AutenticacaoDoisFatores.Dominio.Compartilhados;
using System.Security.Cryptography;
using System.Text;

namespace AutenticacaoDoisFatores.Dominio.Dominios
{
    public static class Criptografia
    {
        public static string CriptografarComSha512(string valor)
        {
            return CriptografarEmHash(valor, SHA512.Create());
        }

        private static string CriptografarEmHash(string valor, HashAlgorithm algoritmo)
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

        public static string CriptografarEmAes(string valor)
        {
            if (valor.EstaVazio())
                throw new ArgumentNullException(nameof(valor));

            var (chave, iv) = RetornarChavesDeCriptografia();

            using var aesAlg = Aes.Create();
            aesAlg.Key = chave;
            aesAlg.IV = iv;

            var criptografia = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

            using var msEncrypt = new MemoryStream();
            using var csEncrypt = new CryptoStream(msEncrypt, criptografia, CryptoStreamMode.Write);
            using (var swEncrypt = new StreamWriter(csEncrypt))
            {
                swEncrypt.Write(valor);
            }
            return Convert.ToBase64String(msEncrypt.ToArray());
        }

        public static string DescriptografarEmAes(string valor)
        {
            if (valor.EstaVazio())
                throw new ArgumentNullException(nameof(valor));

            var (chave, iv) = RetornarChavesDeCriptografia();

            using var aesAlg = Aes.Create();
            aesAlg.Key = chave;
            aesAlg.IV = iv;

            var descriptografia = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

            using var msDecrypt = new MemoryStream(Convert.FromBase64String(valor));
            using var csDecrypt = new CryptoStream(msDecrypt, descriptografia, CryptoStreamMode.Read);
            using var srDecrypt = new StreamReader(csDecrypt);
            return srDecrypt.ReadToEnd();
        }

        private static (byte[] chave, byte[] iv) RetornarChavesDeCriptografia()
        {
            var chaveSecreta = Environment.GetEnvironmentVariable("ADF_CHAVE_CRIPTOGRAFIA");
            if (chaveSecreta is null || chaveSecreta.EstaVazio())
                throw new ApplicationException("A chave de autenticação não foi encontrada.");

            return (Encoding.UTF8.GetBytes(chaveSecreta), Encoding.UTF8.GetBytes(chaveSecreta));
        }
    }
}

using System.Text;

namespace AutenticacaoDoisFatores.Domain.Shared
{
    internal static class Secrets
    {
        private const string AvaibleCharacters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ234567";

        internal static string Generate(int size = 20)
        {
            var random = new Random();
            var result = new StringBuilder(size);

            for (int i = 0; i < size; i++)
            {
                var idx = random.Next(AvaibleCharacters.Length);
                result.Append(AvaibleCharacters[idx]);
            }

            return result.ToString();
        }
    }
}

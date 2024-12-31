using System.Globalization;
using System.Text;

namespace AutenticacaoDoisFatores.Dominio.Compartilhados
{
    public static class ExtensaoString
    {
        public static bool EstaVazio(this string valor)
        {
            return string.IsNullOrEmpty(valor) || string.IsNullOrWhiteSpace(valor);
        }

        public static string RemoverCaracteresEspeciais(this string valor)
        {
            var sb = new StringBuilder();

            var valorEmLista = valor.Normalize(NormalizationForm.FormD).ToCharArray();
            foreach (var letter in valorEmLista)
                if (CharUnicodeInfo.GetUnicodeCategory(letter) != UnicodeCategory.NonSpacingMark)
                    sb.Append(letter);

            valor = sb.ToString();
            sb.Clear();

            foreach (var c in valor)
                if ((c >= '0' && c <= '9') || (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || c == '.' || c == '_')
                    sb.Append(c);

            return sb.ToString();
        }
    }
}

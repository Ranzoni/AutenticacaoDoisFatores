using System.Text.RegularExpressions;

namespace AutenticacaoDoisFatores.Dominio.Compartilhados
{
    public static partial class ExtensaoString
    {
        public static bool EstaVazio(this string valor)
        {
            return string.IsNullOrEmpty(valor) || string.IsNullOrWhiteSpace(valor);
        }

        public static bool ExistemCaracteresEspeciaisAcentosOuPontuacoes(this string valor)
        {
            var regex = RegexCaracteresEspeciaisOuPontuacoes();
            return regex.IsMatch(valor) || ExistemAcentos(valor);
        }

        public static bool ExistemAcentos(this string valor)
        {
            const string ACENTOS = "ÄÅÁÂÀÃäáâàãÉÊËÈéêëèÍÎÏÌíîïìÖÓÔÒÕöóôòõÜÚÛüúûùÇç";

            return ACENTOS.Any(a => valor.Contains(a));
        }

        public static bool EhEmail(this string valor)
        {
            var regex = RegexEmail();
            return regex.IsMatch(valor);
        }

        #region Regex

        [GeneratedRegex(@"[^\w\s]")]
        private static partial Regex RegexCaracteresEspeciaisOuPontuacoes();

        [GeneratedRegex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$")]
        private static partial Regex RegexEmail();

        #endregion Regex
    }
}

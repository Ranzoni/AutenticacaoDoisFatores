using System.Text.RegularExpressions;

namespace AutenticacaoDoisFatores.Dominio.Compartilhados
{
    public static partial class ExtensaoString
    {
        private static readonly string _letrasMinusculasComAcento = "äáâàãéêëèíîïìöóôòõüúûùç";
        private static readonly string _letrasMaiusculasComAcento = "ÄÅÁÂÀÃÉÊËÈÍÎÏÌÖÓÔÒÕÜÚÛÇ";
        private static readonly string _letrasComAcento = _letrasMinusculasComAcento + _letrasMaiusculasComAcento;
        private static readonly string _letrasMaiusculas = "ABCDEFGHIJKLMNOPQRSTUVWXYZ" + _letrasMaiusculasComAcento;
        private static readonly string _letrasMinusculas = "abcdefghijklmnopqrstuvwxyz" + _letrasMinusculasComAcento;

        public static bool EstaVazio(this string valor)
        {
            return string.IsNullOrEmpty(valor) || string.IsNullOrWhiteSpace(valor);
        }

        public static bool ExistemCaracteresEspeciaisAcentosOuPontuacoes(this string valor)
        {
            var regex = RegexCaracteresEspeciaisOuPontuacoes();
            return regex.IsMatch(valor) || ExistemAcentos(valor);
        }

        public static bool ExistemLetrasMaiusculas(this string valor)
        {
            return _letrasMaiusculas.Any(valor.Contains);
        }

        public static bool ExistemLetrasMinusculas(this string valor)
        {
            return _letrasMinusculas.Any(valor.Contains);
        }

        public static bool ExistemAcentos(this string valor)
        {
            return _letrasComAcento.Any(valor.Contains);
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

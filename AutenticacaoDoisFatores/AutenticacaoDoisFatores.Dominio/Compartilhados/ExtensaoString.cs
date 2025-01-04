using System.Globalization;
using System.Text;
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
            var regex = CaracteresEspeciaisOuPontuacoes();
            return regex.IsMatch(valor) || ExistemAcentos(valor);
        }

        public static bool ExistemAcentos(this string valor)
        {
            const string ACENTOS = "ÄÅÁÂÀÃäáâàãÉÊËÈéêëèÍÎÏÌíîïìÖÓÔÒÕöóôòõÜÚÛüúûùÇç";

            return ACENTOS.Any(a => valor.Contains(a));
        }

        [GeneratedRegex(@"[^\w\s]")]
        private static partial Regex CaracteresEspeciaisOuPontuacoes();
    }
}

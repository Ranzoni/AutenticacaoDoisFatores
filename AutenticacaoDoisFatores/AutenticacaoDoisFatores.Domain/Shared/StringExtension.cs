using System.Text.RegularExpressions;

namespace AutenticacaoDoisFatores.Domain.Shared
{
    public static partial class StringExtension
    {
        private static readonly string _lowerCaseLettersWithAccent = "äáâàãéêëèíîïìöóôòõüúûùç";
        private static readonly string _upperCaseLettersWithAccent = "ÄÅÁÂÀÃÉÊËÈÍÎÏÌÖÓÔÒÕÜÚÛÇ";
        private static readonly string _lettersWithAccent = _lowerCaseLettersWithAccent + _upperCaseLettersWithAccent;
        private static readonly string _upperCaseLetters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ" + _upperCaseLettersWithAccent;
        private static readonly string _lowerCaseLetters = "abcdefghijklmnopqrstuvwxyz" + _lowerCaseLettersWithAccent;
        private static readonly string _numbers = "0123456789";

        public static bool IsNullOrEmptyOrWhiteSpaces(this string value)
        {
            return string.IsNullOrEmpty(value) || string.IsNullOrWhiteSpace(value);
        }

        public static bool AnySpecialCharactersOrPontuations(this string value)
        {
            var regex = SpecialCharactersOrPontuationsRegex();
            return regex.IsMatch(value) || AnyAccent(value);
        }

        public static bool AnyUpperCase(this string value)
        {
            return _upperCaseLetters.Any(value.Contains);
        }

        public static bool AnyLowerCase(this string value)
        {
            return _lowerCaseLetters.Any(value.Contains);
        }

        public static bool AnyAccent(this string value)
        {
            return _lettersWithAccent.Any(value.Contains);
        }

        public static bool AnyNumber(this string value)
        {
            return _numbers.Any(value.Contains);
        }

        public static bool IsValidEmail(this string value)
        {
            var regex = EmailRegex();
            return regex.IsMatch(value);
        }

        #region Regex

        [GeneratedRegex(@"[^\w\s]")]
        private static partial Regex SpecialCharactersOrPontuationsRegex();

        [GeneratedRegex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$")]
        private static partial Regex EmailRegex();

        #endregion Regex
    }
}

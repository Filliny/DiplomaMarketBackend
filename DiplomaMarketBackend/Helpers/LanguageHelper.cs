using System.Runtime.CompilerServices;

namespace DiplomaMarketBackend.Helpers
{
    public static class LanguageHelper
    {
        public static string NormalizeLang (this string lang) {

            lang = lang.ToUpper();

            if (lang != "UK" && lang != "RU")
             return "UK";

            return lang;
        }
    }
}

using System.Globalization;

namespace EventHub._01.Web.Helpers
{
    public static class CurrencyHelper
    {
        private static readonly CultureInfo UsCulture = new CultureInfo("en-US");

        public static string FormatCurrency(this decimal value)
        {
            return value.ToString("C2", UsCulture);
        }

        public static string FormatCurrency(this decimal? value)
        {
            return (value ?? 0).ToString("C2", UsCulture);
        }
    }
}

using System.Linq;

namespace SimpleConfig
{
    public static class Extensions
    {
        public static string ToCamelCase(this string value)
        {
            if (string.IsNullOrEmpty(value)) return value;
            return value.First().ToString().ToLower() + (value.Length > 1 ? value.Substring(1) : "");
        }
    }
}
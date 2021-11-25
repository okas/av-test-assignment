using System.Text.RegularExpressions;
using static System.Net.WebRequestMethods;

namespace Backend.WebApi.Utilities;
public static class StringUtilities
{
    /// <summary>
    /// Coneverts string to camelCase.
    /// </summary>
    /// <permission href="https://newbedev.com/convert-string-to-camelcase-from-titlecase-c"></permission>
    public static string ToCamelCase(string s)
    {
        var x = s.Replace("_", "");
        if (x.Length == 0) return "null";
        x = Regex.Replace(x, "([A-Z])([A-Z]+)($|[A-Z])",
            m => m.Groups[1].Value + m.Groups[2].Value.ToLower() + m.Groups[3].Value);
        return char.ToLower(x[0]) + x.Substring(1);
    }
}
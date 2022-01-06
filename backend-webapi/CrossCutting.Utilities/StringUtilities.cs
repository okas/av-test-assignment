using System.Text.RegularExpressions;

namespace Backend.WebApi.CrossCutting.Utilities;
public static class StringUtilities
{
    /// <summary>
    /// Coneverts string to camelCase.
    /// </summary>
    /// <remarks><seealso href="https://newbedev.com/convert-string-to-camelcase-from-titlecase-c"></seealso></remarks>
    public static string ToCamelCase(string s)
    {
        string temp = s.Replace("_", "", StringComparison.InvariantCulture);

        if (temp.Length == 0)
        {
            return "";
        }

        Regex regexp = new(
            @"([A-Z])([A-Z]+)($|[A-Z])",
            RegexOptions.ExplicitCapture,
            TimeSpan.FromSeconds(1)
            );

        temp = regexp.Replace(
            temp,
            m => $"{m.Groups[1].Value}{m.Groups[2].Value.ToLowerInvariant()}{m.Groups[3].Value}"
            );

        char lowFirst = char.ToLowerInvariant(temp[0]);

        return $"{lowFirst}{temp[1..]}";
    }
}

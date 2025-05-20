using System.Text.RegularExpressions;

namespace ImGuiWS.Utils;

public static class ToControlIdExtension
{
    public static string ToControlId(this string str)
    {
        if (string.IsNullOrWhiteSpace(str))
            return string.Empty;

        // Convert to lower case and replace spaces with underscores
        var cleaned = str.ToLower().Replace(' ', '_').Trim();

        // Remove all characters except letters, digits, and underscores
        cleaned = Regex.Replace(cleaned, @"[^a-z0-9_]", string.Empty);

        return cleaned;
    }
}
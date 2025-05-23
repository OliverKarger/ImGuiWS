using System.Text.RegularExpressions;

namespace ImGuiWS.Utils;

public static class ToControlIdExtension {
    public static String ToControlId(this String str) {
        if(String.IsNullOrWhiteSpace(str)) {
            return String.Empty;
        }

        // Convert to lower case and replace spaces with underscores
        String cleaned = str.ToLower().Replace(' ', '_').Trim();

        // Remove all characters except letters, digits, and underscores
        cleaned = Regex.Replace(cleaned, @"[^a-z0-9_]", String.Empty);

        return cleaned;
    }
}
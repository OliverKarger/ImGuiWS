namespace ImGuiWS.Controls.Utils;

public static class ToControlIdExtension
{
    public static string ToControlId(this string s)
    {
        return s.Trim().Replace(' ', '_').ToLower();
    }
}
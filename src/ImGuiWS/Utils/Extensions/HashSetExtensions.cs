namespace ImGuiWS.Utils.Extensions;

public static class HashSetExtensions
{
    public static void ForEach<T>(this HashSet<T> hashSet, Action<T> action)
    {
        foreach (T item in hashSet) action(item);
    }
}
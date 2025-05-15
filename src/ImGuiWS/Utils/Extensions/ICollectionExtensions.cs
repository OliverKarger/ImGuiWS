namespace ImGuiWS.Utils.Extensions;

public static class ICollectionExtensions
{
    public static void ForEach<T>(this ICollection<T> collection, Action<T> action)
    {
        foreach (T item in collection) action(item);
    }
}
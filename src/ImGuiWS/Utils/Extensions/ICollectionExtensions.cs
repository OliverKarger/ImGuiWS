namespace ImGuiWS.Utils.Extensions;

/// <summary>
///     Extension Methods for <see cref="ICollection{T}" />
/// </summary>
public static class ICollectionExtensions {
    /// <summary>
    ///     Simple ForEach Loop Extension for <see cref="ICollection{T}" />
    ///     Behaves like foreach()
    /// </summary>
    public static void ForEach<T>(this ICollection<T> collection, Action<T> action) {
        foreach(T item in collection) {
            action(item);
        }
    }
}
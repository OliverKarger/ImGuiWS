using System.Reflection;
using ImGuiWS.Components;
using ImGuiWS.Components.Controls;

namespace ImGuiWS.Components;

/// <summary>
///     Extensions to <see cref="RenderableComponent"/>
/// </summary>
public static class RenderableComponentSearchExtensions
{
    /// <summary>
    ///     Searches for a <see cref="RenderableComponent"/> of type <typeparamref name="T"/>
    ///     starting at <paramref name="window"/> including all sub-windows and components
    /// </summary>
    /// <param name="window">
    ///     Window as starting Place
    /// </param>
    /// <param name="predicate">
    ///     Predicate to match the Component against
    /// </param>
    /// <typeparam name="T">
    ///     Type to match the Component against
    /// </typeparam>
    /// <returns>
    ///     Component of type <typeparamref name="T"/> or <c>null</c>
    /// </returns>
    public static T? FindComponent<T>(this Window window, Func<T?, bool> predicate) where T : RenderableComponent
    {
        // check the controls of the main window
        foreach (Control control in window.Controls.Components)
        {
            if (control is T)
            {
                if (predicate(control as T))
                {
                    return control as T;
                }
            }
        }
        
        // check each sub window of the window
        foreach (Window subWindowOfMain in window.SubWindows.Components)
        {
            if (subWindowOfMain is T)
            {
                if (predicate(subWindowOfMain as T))
                {
                    return subWindowOfMain as T;
                }
            }
            
            return FindComponent(subWindowOfMain, predicate);
        }

        return null;
    }

}
using System.Reflection;
using ImGuiWS.Components;
using ImGuiWS.Components.Controls;

namespace ImGuiWS.Components;

public static class RenderableComponentSearchExtensions
{
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
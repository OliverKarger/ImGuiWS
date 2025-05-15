using System.Data;

namespace ImGuiWS.Controls;

public class WindowCollection(Window parent)
{
    private readonly HashSet<Window> SubWindows = new();
    private readonly Window Parent = parent;

    public WindowCollection Add(Window window)
    {
        if (SubWindows.Any(e => e.Id == window.Id))
        {
            throw new DuplicateNameException("Duplicate SubWindow name/id");
        }
        
        SubWindows.Add(window);
        return this;
    }

    public WindowCollection Add(string label, Action<Window> configure)
    {
        Window window = new Window(label);
        configure(window);
        return Add(window);
    }

    internal void Render()
    {
        foreach (var subWindow in SubWindows) subWindow.Update();
    }
}
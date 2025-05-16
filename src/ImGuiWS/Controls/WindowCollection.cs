using System.Data;
using ImGuiNET;
using ImGuiWS.Renderer;

namespace ImGuiWS.Controls;

/// <summary>
///     Contains SubWindows to a Window
/// </summary>
/// <param name="parent">
///     Parent Window
/// </param>
public class WindowCollection(Window parent) : IRenderable
{
    private readonly HashSet<Window> SubWindows = new();
    private readonly Window Parent = parent;

    /// <summary>
    ///     Adds a new Window
    /// </summary>
    /// <param name="window">
    ///     Window Object. Id must be unique!
    /// </param>
    /// <returns></returns>
    /// <exception cref="DuplicateNameException">
    ///     Thrown when a Window with the same Id already exists
    /// </exception>
    public WindowCollection Add(Window window)
    {
        if (SubWindows.Any(e => e.Id == window.Id))
        {
            throw new DuplicateNameException("Duplicate SubWindow name/id");
        }

        window.Parent = Parent;
        SubWindows.Add(window);
        return this;
    }

    /// <summary>
    ///     Adds a new Window
    /// </summary>
    /// <param name="label">
    ///     Window Label
    /// </param>
    /// <param name="configure">
    ///     Delegate to Configure the Window
    /// </param>
    public WindowCollection Add(string label, Action<Window> configure)
    {
        Window window = new Window(label);
        configure(window);
        return Add(window);
    }

    public void Start()
    {
        foreach (var subWindow in SubWindows)
        {
            ImGui.PushID(subWindow.Id); 
            subWindow.Start();
            ImGui.PopID();
        }
    }

    public void Update()
    {
        foreach (var subWindow in SubWindows)
        {
            ImGui.PushID(subWindow.Id); 
            subWindow.Update();
            ImGui.PopID();
        }
    }

    public void Shutdown()
    {
        foreach (var subWindow in SubWindows)
        {
            ImGui.PushID(subWindow.Id); 
            subWindow.Shutdown();
            ImGui.PopID();
        }
    }
}
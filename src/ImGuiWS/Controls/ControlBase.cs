using ImGuiWS.Renderer;

namespace ImGuiWS.Controls;

/// <summary>
///     Base Class for all Controls
/// </summary>
/// <param name="id"></param>
public abstract class ControlBase(string id) : IRenderable
{
    /// <summary>
    ///     ImGui ID
    /// </summary>
    public readonly string Id = id;

    /// <summary>
    ///     Controls is visible
    /// </summary>
    public bool Visible { get; set; } = true;
    
    /// <summary>
    ///     Window, the Control is associated to
    /// </summary>
    protected internal Window? DirectParent { get; set; }

    protected internal MainWindow RootWindow { get; set; }

    public override bool Equals(object obj)
    {
        if (obj is ControlBase control)
        {
            return Id == control.Id;
        }

        return false;
    }

    public override int GetHashCode()
    {
        return Id?.GetHashCode() ?? 0;
    }

    public abstract void Start();
    public abstract void Update();
    public abstract void Shutdown();
}
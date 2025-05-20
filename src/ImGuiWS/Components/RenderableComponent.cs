using ImGuiNET;
using ImGuiWS.Renderer;

namespace ImGuiWS.Components;

public abstract class RenderableComponent
{
    protected RenderableComponent(string id)
    {
        Id = id;
    }

    protected RenderableComponent(string id, Window? parentWindow = null)
    {
        Id = id;
        ParentWindow = parentWindow;
    }
    
    /// <summary>
    ///     Reference to Main Window 
    /// </summary>
    public MainWindow MainWindow { get; protected internal set; }
    
    /// <summary>
    ///     Reference to Parent Window
    /// </summary>
    public Window? ParentWindow { get; protected internal set; }

    /// <summary>
    ///     Visiblity Flag
    /// </summary>
    public bool Visible { get; set; } = true;

    public void ToggleVisibility() => Visible = !Visible;
    
    /// <summary>
    ///       ImGui ID
    /// </summary>
    public string Id { get; protected set; }

    protected internal void SelfCheck()
    {
        if (MainWindow == null)
        {
            throw new NullReferenceException("MainWindow is null");
        }
    }

    /// <summary>
    ///       Called at Component Initialization Phase
    /// </summary>
    public virtual void Startup() {}

    /// <summary>
    ///       Called each Frame
    /// </summary>
    /// <param name="delta">
    ///       Frame Time Delta
    /// </param>
    public virtual void Update(float delta) {}

    /// <summary>
    ///       Called at Component Shutdown Phase
    /// </summary>
    public virtual void Shutdown() {}

    /// <summary>
    ///     Used to Track the amount of applied Style Variables
    ///     for ImGui.PushStyleVar() and ImGui.PopStyleVar()
    /// </summary>
    protected int AppliedStyles = 0;

    /// <summary>
    ///     Used to Track the amount of applied Color Variables
    ///     for ImGui.PushStyleColor() and ImGui.PopStyleColor()
    /// </summary>
    protected int AppliedColors = 0;

    /// <summary>
    ///     Used to Track if the Style was already applied
    /// </summary>
    protected bool StylesApplied = false;
    
    /// <summary>
    ///     Applies the current Style
    /// </summary>
    protected virtual void PushStyle()
    {
    }

    /// <summary>
    ///     ImGui.Pop() for the current Style
    /// </summary>
    protected virtual void PopStyle()
    {
        if (StylesApplied) return;
        
        if (AppliedStyles > 0)
        {
            ImGui.PopStyleVar(AppliedStyles);
        }

        if (AppliedColors > 0)
        {
            ImGui.PopStyleColor(AppliedColors);
        }
    }
}
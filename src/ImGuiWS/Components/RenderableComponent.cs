using ImGuiNET;
using ImGuiWS.Renderer;

namespace ImGuiWS.Components;

/// <summary>
///     Represents a renderable Component with
///     Lifetime
/// </summary>
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
    
    #region Properties
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
    
    /// <summary>
    ///       ImGui ID
    /// </summary>
    public string Id { get; protected set; }
    
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
    #endregion
    
    #region Methods
    public void ToggleVisibility() => Visible = !Visible;

    protected internal void SelfCheck()
    {
        if (MainWindow == null)
        {
            throw new NullReferenceException("MainWindow is null");
        }
    }
    #endregion
    
    #region Virtual Methods
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
    #endregion
}
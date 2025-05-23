namespace ImGuiWS.Components;

/// <summary>
///     Represents a renderable Component with
///     Lifetime
/// </summary>
public abstract class RenderableComponent {
    protected RenderableComponent(String id) {
        this.Id = id;
    }

    protected RenderableComponent(String id, Window? parentWindow = null) {
        this.Id = id;
        this.ParentWindow = parentWindow;
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
    public Boolean Visible { get; set; } = true;

    /// <summary>
    ///     ImGui ID
    /// </summary>
    public String Id { get; protected set; }

    /// <summary>
    ///     Used to Track the amount of applied Style Variables
    ///     for ImGui.PushStyleVar() and ImGui.PopStyleVar()
    /// </summary>
    protected Int32 AppliedStyles = 0;

    /// <summary>
    ///     Used to Track the amount of applied Color Variables
    ///     for ImGui.PushStyleColor() and ImGui.PopStyleColor()
    /// </summary>
    protected Int32 AppliedColors = 0;

    /// <summary>
    ///     Used to Track if the Style was already applied
    /// </summary>
    protected Boolean StylesApplied = false;

    #endregion

    #region Methods

    public void ToggleVisibility() {
        this.Visible = !this.Visible;
    }

    protected internal void SelfCheck() {
        if(this.MainWindow == null) {
            throw new NullReferenceException("MainWindow is null");
        }
    }

    #endregion

    #region Virtual Methods

    /// <summary>
    ///     Called at Component Initialization Phase
    /// </summary>
    public virtual void Startup() { }

    /// <summary>
    ///     Called each Frame
    /// </summary>
    /// <param name="delta">
    ///     Frame Time Delta
    /// </param>
    public virtual void Update(Single delta) { }

    /// <summary>
    ///     Called at Component Shutdown Phase
    /// </summary>
    public virtual void Shutdown() { }

    #endregion
}
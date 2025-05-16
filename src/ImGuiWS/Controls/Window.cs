using ImGuiNET;
using ImGuiWS.Controls.Utils;
using ImGuiWS.Logging;
using ImGuiWS.Renderer;
using Serilog;

namespace ImGuiWS.Controls;

public enum WindowRenderMode
{
    ControlsFirst,
    SubWindowsFirst
}

/// <summary>
///     Base Class for all Windows
/// </summary>
public class Window(string label) : IRenderable
{
    private readonly ILogger _logger = LoggerFactory.Create<Window>();

    /// <summary>
    ///     ImGui ID
    /// </summary>
    /// <remarks>
    ///     Automatically inferred from Label
    /// </remarks>
    public string Id { get; internal set; } = label.ToControlId();
    
    /// <summary>
    ///     Window Controls
    /// </summary>
    public WindowControlsCollection Controls { get; internal set; }
    
    /// <summary>
    ///     Sub Windows
    /// </summary>
    public WindowCollection Windows { get; internal set; }
    
    /// <summary>
    ///     Handle to direct parent Window
    /// </summary>
    public Window? DirectParent { get; internal set; }
    
    /// <summary>
    ///     Handle to Root Window
    /// </summary>
    public MainWindow RootWindow { get; internal set; }
    
    /// <summary>
    ///     Window Render Mode
    /// </summary>
    public WindowRenderMode RenderMode { get; set; } = WindowRenderMode.SubWindowsFirst;
    
    /// <summary>
    ///     Wether the Window is open or not
    /// </summary>
    public bool Open { get; set; } = true;
    
    /// <summary>
    ///     Invoked when Window is opened
    /// </summary>
    public event Action OnOpened;
    
    /// <summary>
    ///     Invoked when Window is closed
    /// </summary>
    public event Action OnClosed;

    /// <summary>
    ///     Window Label / Title
    /// </summary>
    public string Label { get; set; } = label;

    /// <summary>
    ///     Will be executed when Window is first registered
    /// </summary>
    /// <remarks>
    ///     Should be used for resource Allocations
    /// </remarks>
    public virtual void Start()
    {
        _logger.Debug("Initializing Controls ({controlCount}) and SubWindows ({subWindowCount}) using {mode} for {windowName}", 
            Controls.Count,
            Windows.Count,
            RenderMode,
            Id);
        
        switch (RenderMode)
        {
            case WindowRenderMode.ControlsFirst:
                Controls.Start();
                Windows.Start();
                break;
            case WindowRenderMode.SubWindowsFirst:
                Windows.Update();
                Controls.Update();
                break;
        }
    }

    /// <summary>
    ///     Will be called each Frame
    /// </summary>
    /// <remarks>
    ///     Should not be overriddenn unless you know what you're doing!
    /// </remarks>
    public virtual void Update()
    {
        bool open = Open;
        if (ImGui.Begin(Label, ref open))
        {
            Open = open;

            switch (RenderMode)
            {
                case WindowRenderMode.ControlsFirst:
                    Controls.Update();
                    Windows.Update();
                    break;
                case WindowRenderMode.SubWindowsFirst:
                    Windows.Update();
                    Controls.Update();
                    break;
            }
        }

        ImGui.End();
    }

    /// <summary>
    ///     Called when the Window Object is destroyed
    /// </summary>
    public virtual void Shutdown()
    {
        switch (RenderMode)
        {
            case WindowRenderMode.ControlsFirst:
                Controls.Start();
                Windows.Start();
                break;
            case WindowRenderMode.SubWindowsFirst:
                Windows.Update();
                Controls.Update();
                break;
        }
    }
}
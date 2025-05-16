using ImGuiNET;
using ImGuiWS.Controls.Utils;
using ImGuiWS.Renderer;

namespace ImGuiWS.Controls;

public enum WindowRenderMode
{
    ControlsFirst,
    SubWindowsFirst
}

/// <summary>
///     Base Class for all Windows
/// </summary>
public class Window : IRenderable
{
    /// <summary>
    ///     ImGui ID
    /// </summary>
    /// <remarks>
    ///     Automatically inferred from Label
    /// </remarks>
    public string Id { get; internal set; }
    
    /// <summary>
    ///     Window Controls
    /// </summary>
    public WindowControlsCollection Controls { get; internal set; }
    
    /// <summary>
    ///     Sub Windows
    /// </summary>
    public WindowCollection Windows { get; internal set; }
    
    /// <summary>
    ///     Handle to parent Window
    /// </summary>
    public Window? Parent { get; internal set; }
    
    /// <summary>
    ///     Window Render Mode
    /// </summary>
    public WindowRenderMode RenderMode { get; set; } = WindowRenderMode.ControlsFirst;
    
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
    public string Label { get; set; }

    public Window(string label)
    {
        Id = label.ToControlId();
        Label = label;
        Controls = new WindowControlsCollection(this);
        Windows = new WindowCollection(this);
    }

    /// <summary>
    ///     Will be executed when Window is first registered
    /// </summary>
    /// <remarks>
    ///     Should be used for resource Allocations
    /// </remarks>
   public virtual void Start(){}

    /// <summary>
    ///     Will be called each Frame
    /// </summary>
    /// <remarks>
    ///     Should not be overriddenn unless you know what you're doing!
    /// </remarks>
    public virtual void Update()
    {
        bool open = Open;
        bool beginSucceeded = ImGui.Begin(Label, ref open);

        if (open != Open)
        {
            Open = open;
            if (Open)
                OnOpened?.Invoke();
            else
                OnClosed?.Invoke();
        }

        if (beginSucceeded)
        {
            if (RenderMode == WindowRenderMode.ControlsFirst)
            {
                Controls.Update();
                Windows.Update();
            }

            if (RenderMode == WindowRenderMode.SubWindowsFirst)
            {
                Windows.Update();
                Controls.Update();
            }
        }

        ImGui.End(); // Always call End if Begin was called
    }
    
    /// <summary>
    ///     Called when the Window Object is destroyed
    /// </summary>
    public virtual void Shutdown(){}
}
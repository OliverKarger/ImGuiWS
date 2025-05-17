using System.Numerics;
using ImGuiNET;
using ImGuiWS.Controls.Utils;
using ImGuiWS.Design;
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
///     Options Class for Windows
/// </summary>
public class WindowOptions
{
    public Vector2 Position { get; set; } = Vector2.Zero;
    public Vector2 Size { get; set; } = Vector2.One;
    public bool FixedSize { get; set; } = true;
    public bool FixedPosition { get; set; } = true;
    public bool Open { get; set; } = true;
    public bool Collapsed { get; set; } = false;
    public string Label { get; set; } = string.Empty;

    public Styles Style { get; set; } = new();
    public Colors Colors { get; set; } = new();
}

/// <summary>
///     Base Class for all Windows
/// </summary>
public class Window(string label) : ControlBase(label.ToControlId())
{
    private readonly ILogger _logger = LoggerFactory.Create<Window>(label);

    public readonly WindowOptions Options = new()
    {
        Label = label
    };

    private bool _firstRenderDone { get; set; } = false;
    private bool _collapsedLastFrame { get; set; } = false;
    private bool _colorsApplied { get; set; } = false;
    private bool _stylesApplied { get; set; } = false;
    
    /// <summary>
    ///     Window Controls
    /// </summary>
    public WindowControlsCollection Controls { get; internal set; }
    
    /// <summary>
    ///     Sub Windows
    /// </summary>
    public WindowCollection Windows { get; internal set; }
    
    /// <summary>
    ///     Window Render Mode
    /// </summary>
    public WindowRenderMode RenderMode { get; set; } = WindowRenderMode.SubWindowsFirst;
    
    /// <summary>
    ///     Invoked when Window is opened
    /// </summary>
    public event Action OnOpened;
    
    /// <summary>
    ///     Invoked when Window is closed
    /// </summary>
    public event Action OnClosed;

    /// <summary>
    ///     Will be executed when Window is first registered
    /// </summary>
    /// <remarks>
    ///     Should be used for resource Allocations
    /// </remarks>
    public override void Start()
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
                Windows.Start();
                Controls.Start();
                break;
        }
    }

    /// <summary>
    ///     Will be called each Frame
    /// </summary>
    /// <remarks>
    ///     Should not be overriddenn unless you know what you're doing!
    /// </remarks>
    public override void Update()
    {
        if (Options.FixedSize || !_firstRenderDone)
        {
            ImGui.SetNextWindowSize(Options.Size);
        }

        if (Options.FixedPosition || !_firstRenderDone)
        {
            ImGui.SetNextWindowPos(Options.Position);
        }

        if (!_firstRenderDone)
        {
            // Set initial collapsed state before the window is created
            ImGui.SetNextWindowCollapsed(Options.Collapsed);
        }
        
        bool open = Options.Open;
        if (ImGui.Begin($"{Options.Label}##{Id}", ref open))
        {
            // Update Options.Open in case user closed the window
            Options.Open = open;

            // Handle collapsed state detection
            bool isCollapsed = ImGui.IsWindowCollapsed();
            if (isCollapsed != _collapsedLastFrame)
            {
                Options.Collapsed = isCollapsed;

                // Optional: Add custom logic for on-collapse/expand
                if (isCollapsed)
                {
                    Console.WriteLine($"Window '{Options.Label}' was just collapsed.");
                }
                else
                {
                    Console.WriteLine($"Window '{Options.Label}' was just expanded.");
                }
            }

            _collapsedLastFrame = isCollapsed;

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
        else
        {
            // If Begin returns false, still update Options.Open
            Options.Open = open;
        }

        ImGui.End();
        
        _firstRenderDone = true;
    }

    /// <summary>
    ///     Called when the Window Object is destroyed
    /// </summary>
    public override void Shutdown()
    {
        switch (RenderMode)
        {
            case WindowRenderMode.ControlsFirst:
                Controls.Shutdown();
                Windows.Shutdown();
                break;
            case WindowRenderMode.SubWindowsFirst:
                Windows.Shutdown();
                Controls.Shutdown();
                break;
        }
    }
}
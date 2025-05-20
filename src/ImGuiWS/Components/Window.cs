using System.Numerics;
using ImGuiNET;
using ImGuiWS.Components.Controls;
using ImGuiWS.Components.Style;
using ImGuiWS.Renderer;
using ImGuiWS.Utils;

namespace ImGuiWS.Components;

/// <summary>
///     Window
/// </summary>
/// <remarks>
///     Represents both a ImGui Internal Window and the Main Window
/// </remarks>
public class Window : RenderableComponent 
{
    #region Properties
    /// <summary>
    ///     Size of the Window
    /// </summary>
    public Vector2 Size { get; set; } = Vector2.Zero;

    /// <summary>
    ///     Fix Size
    /// </summary>
    public bool FixedSize { get; set; } = false;
    
    /// <summary>
    ///     Position of the Window
    /// </summary>
    public Vector2 Position { get; set; } = Vector2.Zero;

    /// <summary>
    ///     Fix Position
    /// </summary>
    public bool FixedPosition { get; set; } = false;

    /// <summary>
    ///     Label/Title of the Window
    /// </summary>
    public string Label { get; set; }
    
    /// <summary>
    ///     Window Collapsed
    /// </summary>
    public bool Collapsed { get; set; } = false;
    
    #endregion

    private bool _firstRenderDone = false;
    private Vector2 _currentPosition = Vector2.Zero;
    private Vector2 _currentSize = Vector2.Zero;
    
    #region Events
    
    /// <summary>
    ///     Invoked when Window was Moved. Passed the new Position
    /// </summary>
    public event Action<Vector2> OnWindowMoved;
    
    /// <summary>
    ///     Invoked when Window was resized. Passed the new Size
    /// </summary>
    public event Action<Vector2> OnWindowResized;

    /// <summary>
    ///     Invoked when Window is collapsed/uncollapsed. Passed the State
    /// </summary>
    public event Action<bool> OnWindowCollapsed;

    /// <summary>
    ///     Invoked when the Window is opened
    /// </summary>
    public event Action OnWindowOpened;
    
    /// <summary>
    ///     Invoked when the Window is opened
    /// </summary>
    public event Action OnWindowClosed;
    
    #endregion
    
    /// <summary>
    ///     Sub Windows
    /// </summary>
    public WindowComponentCollection<Window> SubWindows { get; protected internal set; }
    
    /// <summary>
    ///     Controls
    /// </summary>
    public WindowComponentCollection<Control> Controls { get; protected internal set; }
    
    public Window(string name) : base(name.ToControlId(), null)
    {
        Label = name;
    }

    public override void Startup()
    {
        _currentPosition = Position;
        _currentSize = Size;
        
        SubWindows.Startup();
        Controls.Startup();
    }

    public override void Update(float delta)
    {
        if (!Visible)
            return;

        if (!_firstRenderDone)
        {
            if (FixedSize) ImGui.SetNextWindowSize(Size);
            if (FixedPosition) ImGui.SetNextWindowPos(Position);
        }
        
        bool prevVisible = Visible;
        bool isOpen = Visible; // Capture current visible state
        bool began = ImGui.Begin(Label, ref isOpen);
        
        if (isOpen != prevVisible)
        {
            if (isOpen)
                OnWindowOpened?.Invoke();
            else
                OnWindowClosed?.Invoke();
        } 
        
        // Always update collapsed state after Begin
        bool isCollapsed = ImGui.IsWindowCollapsed();
        if (isCollapsed != Collapsed)
        {
            Collapsed = isCollapsed;
            OnWindowCollapsed?.Invoke(Collapsed);
        }

        if (began)
        {
            SubWindows.Update(delta);
            Controls.Update(delta);

            var newPos = ImGui.GetWindowPos();
            var newSize = ImGui.GetWindowSize();

            if (!FixedPosition && newPos != Position)
            {
                Position = newPos;
                OnWindowMoved?.Invoke(Position);
            }

            if (!FixedSize && newSize != Size)
            {
                Size = newSize;
                OnWindowResized?.Invoke(Size);
            }
        }

        ImGui.End();

        // Update the external Visible flag in case ImGui changed it
        Visible = isOpen;

        _firstRenderDone = true;
    }

    public override void Shutdown()
    {
        SubWindows.Shutdown();
        Controls.Shutdown();
    }
}
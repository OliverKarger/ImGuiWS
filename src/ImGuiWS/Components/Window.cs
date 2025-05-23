using System.Numerics;
using ImGuiNET;
using ImGuiWS.Components.Controls;
using ImGuiWS.Utils;

namespace ImGuiWS.Components;

/// <summary>
///     Window
/// </summary>
/// <remarks>
///     Represents both a ImGui Internal Window and the Main Window
/// </remarks>
public class Window : RenderableComponent {
    private Vector2 _currentPosition = Vector2.Zero;
    private Vector2 _currentSize = Vector2.Zero;

    private Boolean _firstRenderDone;
    private ImGuiWindowFlags _windowFlags = 0;

    public Window(String name) : base(name.ToControlId(), null) {
        this.Label = name;
    }

    /// <summary>
    ///     Sub Windows
    /// </summary>
    public WindowComponentCollection<Window> SubWindows { get; protected internal set; }

    /// <summary>
    ///     Controls
    /// </summary>
    public WindowComponentCollection<Control> Controls { get; protected internal set; }

    public override void Startup() {
        this._currentPosition = this.Position;
        this._currentSize = this.Size;

        this.SubWindows.Startup();
        this.Controls.Startup();
    }

    public override void Update(Single delta) {
        if(!this.Visible) {
            return;
        }

        if(this.FixedPosition) {
            this._windowFlags |= ImGuiWindowFlags.NoMove;
            ImGui.SetNextWindowPos(this.Position);
        }
        else {
            this._windowFlags &= ~ImGuiWindowFlags.NoMove;
        }

        if(this.FixedSize) {
            this._windowFlags |= ImGuiWindowFlags.NoResize;
            ImGui.SetNextWindowSize(this.Size);
        }
        else {
            this._windowFlags &= ~ImGuiWindowFlags.NoResize;
        }

        if(!this._firstRenderDone) {
            ImGui.SetNextWindowPos(this.Position);
        }

        if(!this._firstRenderDone) {
            ImGui.SetNextWindowSize(this.Size);
        }

        Boolean prevVisible = this.Visible;
        Boolean isOpen = this.Visible; // Capture current visible state
        Boolean began = ImGui.Begin(this.Label, ref isOpen, this._windowFlags);

        if(isOpen != prevVisible) {
            if(isOpen) {
                this.OnWindowOpened?.Invoke();
            }
            else {
                this.OnWindowClosed?.Invoke();
            }
        }

        // Always update collapsed state after Begin
        Boolean isCollapsed = ImGui.IsWindowCollapsed();
        if(isCollapsed != this.Collapsed) {
            this.Collapsed = isCollapsed;
            this.OnWindowCollapsed?.Invoke(this.Collapsed);
        }

        if(began) {
            this.SubWindows.Update(delta);
            this.Controls.Update(delta);

            this.ContentOrigin = ImGui.GetCursorScreenPos();

            Vector2 newPos = ImGui.GetWindowPos();
            Vector2 newSize = ImGui.GetWindowSize();

            if(newPos != this.Position) {
                if(!this.FixedPosition) {
                    this.Position = newPos;
                }

                this.OnWindowMoved?.Invoke(this.Position);
            }

            if(newSize != this.Size && !this.FixedPosition) {
                if(!this.FixedSize) {
                    this.Size = newSize;
                }

                this.OnWindowResized?.Invoke(this.Size);
            }
        }

        ImGui.End();

        // Update the external Visible flag in case ImGui changed it
        this.Visible = isOpen;

        this._firstRenderDone = true;
    }

    public override void Shutdown() {
        this.SubWindows.Shutdown();
        this.Controls.Shutdown();
    }

    #region Properties

    /// <summary>
    ///     Size of the Window
    /// </summary>
    public Vector2 Size { get; set; } = Vector2.Zero;

    /// <summary>
    ///     Fix Size
    /// </summary>
    public Boolean FixedSize { get; set; } = false;

    /// <summary>
    ///     Position of the Window
    /// </summary>
    public Vector2 Position { get; set; } = Vector2.Zero;

    /// <summary>
    ///     Fix Position
    /// </summary>
    public Boolean FixedPosition { get; set; } = false;

    /// <summary>
    ///     Label/Title of the Window
    /// </summary>
    public String Label { get; set; }

    /// <summary>
    ///     Window Collapsed
    /// </summary>
    public Boolean Collapsed { get; set; }

    public Vector2 ContentOrigin { get; set; } = Vector2.Zero;

    #endregion

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
    public event Action<Boolean> OnWindowCollapsed;

    /// <summary>
    ///     Invoked when the Window is opened
    /// </summary>
    public event Action OnWindowOpened;

    /// <summary>
    ///     Invoked when the Window is opened
    /// </summary>
    public event Action OnWindowClosed;

    #endregion
}
using System.Diagnostics;
using System.Numerics;
using ImGuiWS.Components.Controls;
using ImGuiWS.Renderer;
using ImGuiWS.Utils;
using Veldrid;

namespace ImGuiWS.Components;

/// <summary>
///     Main Window Component
///     Represents the Main Application Window
/// </summary>
public class MainWindow : Window {
    private Single _delta;
    private Stopwatch _stopWatch = new();

    public MainWindow(WindowSetupOptions setupOptions) : base(setupOptions.Title.ToControlId()) {
        this.Backend = new WindowBackend(setupOptions, this);
        this.Label = setupOptions.Title;

        this.MainWindow = this;
        this.ParentWindow = null;
        this.SubWindows = new WindowComponentCollection<Window>(this, this);
        this.Controls = new WindowComponentCollection<Control>(this, this);
    }

    /// <summary>
    ///     Window Backend
    /// </summary>
    public WindowBackend Backend { get; }

    /// <summary>
    ///     Indicates wether the Window should be closed
    /// </summary>
    public Boolean WindowExists => this.Backend.Context.Window.Exists;

    /// <summary>
    ///     Clear/Background Color
    /// </summary>
    public Vector4 ClearColor {
        get => this.Backend.State.ClearColor;
        set => this.Backend.State.ClearColor = value;
    }

    public override void Startup() {
        String imguiIniPath = Path.Join(Environment.CurrentDirectory, "imgui.ini");

        // Backend.LoadFontFromMemory("Roboto", 13);
        this.Backend.LoadDefaultFont(13);

        if(File.Exists(imguiIniPath)) {
            File.Delete(imguiIniPath);
        }

        this.Backend.SetupContext();
        this._stopWatch = Stopwatch.StartNew();

        base.Startup();
    }

    public override void Update(Single deltaTime) {
        this._delta = this._stopWatch.ElapsedTicks / (Single)Stopwatch.Frequency;
        this._stopWatch.Restart();

        InputSnapshot inputSnapshot = this.Backend.Context.Window.PumpEvents();

        if(!this.WindowExists) {
            return;
        }

        this.Backend.BeginRender();
        this.Backend.UpdateInput(this._delta, inputSnapshot);

        this.SubWindows.Update(this._delta);
        this.Controls.Update(this._delta);

        this.Backend.Render();
        this.Backend.EndRender();
    }

    public override void Shutdown() {
        base.Shutdown();
        this.Backend.Dispose();
    }
}
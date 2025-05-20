using System.Diagnostics;
using System.Numerics;
using ImGuiWS.Components.Controls;
using ImGuiWS.Renderer;
using ImGuiWS.Utils;
using Veldrid;
using Veldrid.StartupUtilities;

namespace ImGuiWS.Components;

public class MainWindow : Window
{
    private Stopwatch _stopWatch = new Stopwatch();
    private float _delta = .0f;
    
    /// <summary>
    ///     Window Backend
    /// </summary>
    public WindowBackend Backend { get; private set; }
    
    /// <summary>
    ///     Indicates wether the Window should be closed
    /// </summary>
    public bool WindowExists => Backend.Context.Window.Exists;
    
    /// <summary>
    ///     Clear/Background Color
    /// </summary>
    public Vector4 ClearColor
    {
        get => Backend.State.ClearColor;
        set => Backend.State.ClearColor = value;
    }

    public MainWindow(WindowSetupOptions setupOptions) : base(setupOptions.Title.ToControlId())
    {
        Backend = new WindowBackend(setupOptions, this);
        Label = setupOptions.Title;

        MainWindow = this;
        ParentWindow = null;
        SubWindows = new WindowComponentCollection<Window>(this, this);
        Controls = new WindowComponentCollection<Control>(this, this);
    }

    public override void Startup()
    {
        string imguiIniPath = Path.Join(Environment.CurrentDirectory, "imgui.ini");

        // Backend.LoadFontFromMemory("Roboto", 13);
        Backend.LoadDefaultFont(13);
        
        if (File.Exists(imguiIniPath)) File.Delete(imguiIniPath);
        
        Backend.SetupContext(); 
        _stopWatch = Stopwatch.StartNew();
        
        base.Startup();
    }

    public override void Update(float deltaTime)
    {
        _delta = _stopWatch.ElapsedTicks / (float)Stopwatch.Frequency;
        _stopWatch.Restart();

        InputSnapshot inputSnapshot = Backend.Context.Window.PumpEvents();

        if (!WindowExists) return;
        
        Backend.BeginRender();
        Backend.UpdateInput(_delta, inputSnapshot);
        
        SubWindows.Update(_delta);
        Controls.Update(_delta);

        Backend.Render();
        Backend.EndRender();
    }

    public override void Shutdown()
    {
        base.Shutdown();
        Backend.Dispose();
    }
}
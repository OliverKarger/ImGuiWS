using System.Diagnostics;
using System.Numerics;
using ImGuiNET;
using ImGuiWS.Controls;
using Veldrid;
using Veldrid.StartupUtilities;

using ImGuiWS.Renderer;

namespace ImGuiWS;

public class MainWindow : Window
{
    protected WindowBackend Backend { get; set; }
    internal WindowEvents Events { get; set; } = new();
    protected WindowUtils Utils { get; set; }

    public WindowRenderMode RenderMode { get; set; } = WindowRenderMode.ControlsFirst;
    
    protected internal Stopwatch Stopwatch = Stopwatch.StartNew();
    protected internal float DeltaTime = .0f;

    public bool WindowExists => Backend.Context.Window.Exists;

    public Vector4 ClearColor
    {
        get => Backend.State.ClearColor;
        set => Backend.State.ClearColor = value;
    }

    public MainWindow(WindowCreateInfo createInfo) : base("MainWindow") 
    {
        Backend = new WindowBackend(createInfo, this);
        Utils = new WindowUtils(Backend, this);
    }

    public override void Start()
    {
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
        
        UserStart();
        
        Backend.SetupContext();
    }
    
    public override void Update()
    {
        DeltaTime = Stopwatch.ElapsedTicks / (float)Stopwatch.Frequency;
        Stopwatch.Restart();
        InputSnapshot snapshot = Backend.Context.Window.PumpEvents();
        if (!WindowExists) { return; }
        Backend.UpdateInput(DeltaTime, snapshot);

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
        
        UserUpdate();
        
        Backend.Submit();
    }

    public void RenderLoop()
    {
        Start();

        while (WindowExists)
        {
            Update();
        }
        
        Shutdown();
    }
    
    public override void Shutdown()
    {
        UserShutdown();
        
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

        Backend.Dispose();
    }

    protected virtual void UserStart()
    {
    }

    protected virtual void UserUpdate()
    {
        
    }

    protected virtual void UserShutdown()
    {
        
    }
}
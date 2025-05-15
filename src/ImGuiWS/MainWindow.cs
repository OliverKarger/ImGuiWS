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
    public WindowBackend Backend { get; internal protected set; }
    public WindowEvents Events { get; internal protected set; } = new();

    public WindowRenderMode RenderMode { get; set; } = WindowRenderMode.ControlsFirst;
    
    internal protected Stopwatch Stopwatch = Stopwatch.StartNew();
    internal protected float DeltaTime = .0f;

    public bool WindowExists => Backend.Context.Window.Exists;

    public Vector4 ClearColor
    {
        get => Backend.State.ClearColor;
        set => Backend.State.ClearColor = value;
    }

    public MainWindow(WindowCreateInfo createInfo) : base("MainWindow") 
    {
        Backend = new WindowBackend(createInfo, this);
    }
    
    public void Render()
    {
        Start();
        
        while (WindowExists)
        {
            DeltaTime = Stopwatch.ElapsedTicks / (float)Stopwatch.Frequency;
            Stopwatch.Restart();
            InputSnapshot snapshot = Backend.Context.Window.PumpEvents();
            if (!WindowExists) { break; }
            Backend.Update(DeltaTime, snapshot);

            switch (RenderMode)
            {
                case WindowRenderMode.ControlsFirst:
                    Controls.Render();
                    Windows.Render();
                    break;
                case WindowRenderMode.SubWindowsFirst:
                    Windows.Render();
                    Controls.Render();
                    break;
            }
            
            Update();
            
            Backend.Submit();
        }

        Shutdown();
        
        Backend.Dispose();
    }
}
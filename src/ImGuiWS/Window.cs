using System.Diagnostics;
using System.Numerics;
using ImGuiNET;
using Veldrid;
using Veldrid.StartupUtilities;

using ImGuiWS.Renderer;

namespace ImGuiWS;

public class Window
{
    public WindowBackend Backend { get; internal protected set; }
    public WindowEvents Events { get; internal protected set; } = new();
    public WindowControlsCollection Controls { get; } = new();

    internal protected Stopwatch Stopwatch = Stopwatch.StartNew();
    internal protected float DeltaTime = .0f;

    public bool WindowExists => Backend.Context.Window.Exists;

    public Vector4 ClearColor
    {
        get => Backend.State.ClearColor;
        set => Backend.State.ClearColor = value;
    }

    public Window(WindowCreateInfo createInfo) 
    {
        Backend = new WindowBackend(createInfo, this);
        Start();
    }

    public void RunLoop()
    {
        // Main application loop
        while (WindowExists)
        {
            DeltaTime = Stopwatch.ElapsedTicks / (float)Stopwatch.Frequency;
            Stopwatch.Restart();
            InputSnapshot snapshot = Backend.Context.Window.PumpEvents();
            if (!WindowExists) { break; }
            Backend.Update(DeltaTime, snapshot);

            Update();
            Controls.Render();
            Backend.Submit();
        }

        Stop();
        
        Backend.Dispose();
    }

    protected virtual void Start()
    {
        
    }

    protected virtual void Update()
    {
        
    }

    protected virtual void Stop()
    {
        
    }
}
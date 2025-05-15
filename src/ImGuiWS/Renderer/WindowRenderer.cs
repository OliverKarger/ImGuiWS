using System.Diagnostics;
using Veldrid;
using Veldrid.Sdl2;
using Veldrid.StartupUtilities;

namespace ImGuiWS.Renderer;

public class WindowRenderer : IDisposable
{
    public WindowResourceContainer Resources { get; internal set; }
    internal readonly Window Window;

    internal Sdl2Window Sdl2Window;
    internal CommandList CommandList;

    internal bool stopRequested = false;
    
    internal ResourceConfiguration ResourceConfiguration { get; set; }

    public WindowRenderer(Window window)
    {
        Window = window;
        Resources = new WindowResourceContainer();
        ResourceConfiguration = new ResourceConfiguration();
        GraphicsDevice graphicsDevice;
        
        VeldridStartup.CreateWindowAndGraphicsDevice(
            new WindowCreateInfo(
                window.Position.Left, 
                window.Position.Top, 
                window.Size.Width, 
                window.Size.Height,
                WindowState.Normal,
                window.Title),
            new GraphicsDeviceOptions(
                true, null, true, ResourceBindingModel.Improved, true, true),
            out Sdl2Window,
            out graphicsDevice);

        Resources.GraphicsDevice = graphicsDevice;
        
        Sdl2Window.Resized += () =>
        {
            Resources.GraphicsDevice.MainSwapchain.Resize(
                (uint)window.Size.Width,
                (uint)window.Size.Height);
            Window.Events.InvokeWindowResized(
                new WindowResizedEventArgs(window.Size.Width, window.Size.Height));
        };
        
        CommandList = Resources.GraphicsDevice.ResourceFactory.CreateCommandList();
    }
    
    public void Dispose()
    {
        Resources.Dispose();
    }

    private void Update()
    {
        
    }

    public void RenderLoop()
    {
        Stopwatch stopwatch = Stopwatch.StartNew();
        float deltaTime = 0f;
        while (!stopRequested)
        {
            deltaTime += (float)stopwatch.ElapsedTicks / (float)Stopwatch.Frequency;
            stopwatch.Restart();
            InputSnapshot snapshot = Sdl2Window.PumpEvents();
            if (!Sdl2Window.Exists) break;
            
        }
    }
}
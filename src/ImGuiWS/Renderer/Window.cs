using System.Numerics;

namespace ImGuiWS.Renderer;

public class Window : IDisposable
{
    public WindowRenderer Renderer { get; internal set; }
    public WindowSize<int> Size { get; set; }
    public WindowPosition<int> Position { get; set; }
    public string Title { get; set; }
    public ModifierKeyState ModifierKeyState { get; internal set; }
    public Vector2 ScaleFactor { get; set; } = Vector2.One;
    
    public WindowEvents Events { get; internal set; }

    public Window(WindowSize<int> size, WindowPosition<int> position)
    {
        Size = size;
        Position = position;
        ModifierKeyState = new ModifierKeyState();
        Events = new();
        Renderer = new WindowRenderer(this);
    }
    
    public void Dispose()
    {
        Renderer.Dispose();
    }

    protected void RequestStop()
    {
        Renderer.stopRequested = true;
    }
}
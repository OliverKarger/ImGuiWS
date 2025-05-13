using System.Numerics;

namespace ImGuiWS.Renderer;

public class Window : IDisposable
{
    public WindowResourceContainer ResourceContainer { get; internal set; }
    public WindowSize<int> Size { get; internal set; }
    public WindowPosition<int> Position { get; internal set; }
    public ModifierKeyState ModifierKeyState { get; internal set; }
    public Vector2 ScaleFactor { get; internal set; } = Vector2.One;

    public Window(WindowSize<int> size, WindowPosition<int> position)
    {
        Size = size;
        Position = position;
        ModifierKeyState = new ModifierKeyState();
    }
    
    public void Dispose()
    {
        ResourceContainer.Dispose();
    }
}
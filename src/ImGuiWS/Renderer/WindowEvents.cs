namespace ImGuiWS.Renderer;

public class WindowResizedEventArgs(int width, int height) : EventArgs
{
    public readonly int Width = width;
    public readonly int Height = height;
}

public class WindowEvents
{
    public event Action<WindowResizedEventArgs> WindowResized;
    internal void InvokeWindowResized(WindowResizedEventArgs e) => WindowResized?.Invoke(e);
}
using System.Numerics;

namespace ImGuiWS.Renderer;

public class WindowResizedEventArgs(float width, float height) : EventArgs
{
    public readonly Vector2 Size = new Vector2(width, height);
}

/// <summary>
///     Collection of Events for <see cref="MainWindow"/>
/// </summary>
public class WindowEvents
{
    public event Action<WindowResizedEventArgs> WindowResized;   
    internal void InvokeWindowResized(float width, float height) => WindowResized?.Invoke(new WindowResizedEventArgs(width, height));
}
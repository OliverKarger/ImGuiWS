using System.Numerics;
using Veldrid;

namespace ImGuiWS.Renderer;

public class WindowState
{
    public bool FrameBegun { get; internal set; }
    
    public Vector2 WindowSize { get; internal set; }
    public Vector2 WindowPosition { get; internal set; }

    public Vector2 ScaleFactor = Vector2.One;

    public Vector4 ClearColor { get; internal set; } = Vector4.Zero;
    
    public IntPtr FontAtlasId { get; internal set; } = (IntPtr)1;
    public int LastAssignedId { get; internal set; } = 100;
}
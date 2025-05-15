using System.Numerics;
using Veldrid;

namespace ImGuiWS.Renderer;

/// <summary>
///     Represents the basic Window State
/// </summary>
public class WindowState
{
    internal bool FrameBegun { get; set; } = false;
    internal IntPtr FontAtlasId { get; set; } = (IntPtr)1;
    internal int LastAssignedId { get; set; } = 100;
    internal bool RenderingBegun { get; set; } = false;
    
    public Vector2 WindowSize { get; internal set; }
    public Vector2 WindowPosition { get; internal set; }
    public Vector2 ScaleFactor = Vector2.One;
    public Vector4 ClearColor { get; internal set; } = Vector4.Zero;
}
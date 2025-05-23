using System.Numerics;

namespace ImGuiWS.Renderer;

/// <summary>
///     Represents the basic Window State
/// </summary>
public class WindowState {
    public Vector2 ScaleFactor = Vector2.One;
    internal Boolean FrameBegun { get; set; } = false;
    internal IntPtr FontAtlasId { get; set; } = 1;
    internal IntPtr LastAssignedId { get; set; } = 100;
    internal Boolean RenderingBegun { get; set; } = false;

    public Vector2 WindowSize { get; internal set; }
    public Vector2 WindowPosition { get; internal set; }
    public Vector4 ClearColor { get; internal set; } = Vector4.Zero;
}
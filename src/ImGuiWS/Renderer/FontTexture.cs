using System.Numerics;
using ImGuiNET;

namespace ImGuiWS.Renderer;

/// <summary>
///     Represents a Font Texture
/// </summary>
public class FontTexture : Texture
{
    /// <summary>
    ///     ImGui Font Handle
    /// </summary>
    public ImFontPtr Handle { get; set; }
    
    /// <summary>
    ///     Display Name of the Font
    /// </summary>
    public string Name { get; set; }
    
    /// <summary>
    ///     Font Pixel Data
    /// </summary>
    public IntPtr Pixels { get; set; }
    
    /// <summary>
    ///     Font Size (X,Y)
    /// </summary>
    public Vector2 Size { get; set; }
    
    /// <summary>
    ///     Font Bytes (Size) per Pixel
    /// </summary>
    public int BytesPerPixel { get; set; }
}
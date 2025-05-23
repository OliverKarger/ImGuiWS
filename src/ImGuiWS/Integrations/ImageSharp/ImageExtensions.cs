using System.Numerics;
using System.Runtime.InteropServices;
using ImGuiWS.Renderer;
using ImGuiWS.Utils.Extensions;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace ImGuiWS.Integrations.ImageSharp;

/// <summary>
///     Extensions to <see cref="Image"/>
/// </summary>
public static class ImageExtensions
{
    /// <summary>
    ///     Converts <see cref="Image"/> to <see cref="Texture"/>
    /// </summary>
    /// <param name="image">
    ///     Image Instance
    /// </param>
    /// <param name="textureManager">
    ///     Texture Manager Instance
    /// </param>
    /// <param name="offset">
    ///     Texture update Offset
    /// </param>
    /// <typeparam name="TPixel">
    ///     Pixel Type
    /// </typeparam>
    /// <returns></returns>
    public static Texture AsTexture2D<TPixel>(
        this Image<TPixel> image,
        TextureManager textureManager,
        Vector3 offset = default) where TPixel : unmanaged, IPixel<TPixel>
    {
        TPixel[] pixels = new TPixel[image.Width * image.Height];
        image.CopyPixelDataTo(pixels);

        ReadOnlySpan<byte> pixelBytes = MemoryMarshal.AsBytes(pixels.AsSpan());
        Texture texture = textureManager.CreateTexture2D(pixelBytes, image.Size.ToVector2(), offset);
        
        textureManager.logger.Information("Created Texture (Size: {sizeX}x{sizeY}, Binding Id: {binding})", texture.Size.X, texture.Size.Y, texture.Id ?? IntPtr.Zero);
        return texture;
    }
}
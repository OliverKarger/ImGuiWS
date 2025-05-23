using System.Numerics;
using System.Runtime.InteropServices;
using Emgu.CV;
using ImGuiWS.Exceptions;
using ImGuiWS.Renderer;

namespace ImGuiWS.Integrations.EmguCV;

/// <summary>
///     Extensions for <see cref="Mat"/>
/// </summary>
public static class MatExtensions
{
    /// <summary>
    ///     Converts <see cref="Mat"/> to <see cref="Texture"/>
    /// </summary>
    /// <param name="mat">
    ///     Mat Instance
    /// </param>
    /// <param name="textureManager">
    ///     Instance of Texture Manager
    /// </param>
    /// <returns></returns>
    /// <exception cref="NoDataException">
    ///     Thrown if <paramref name="mat"/> is Empty
    /// </exception>
    /// <exception cref="NotSupportedException">
    ///     Thrown if <paramref name="mat"/> has an unsupported amount of Channels
    /// </exception>
    public static Texture AsTexture(this Mat mat, TextureManager textureManager)
    {
        if (mat.IsEmpty)
        {
            throw new NoDataException("Mat has no Data", string.Empty);
        }

        // Convert Mat to BGRA if necessary
       Mat convertedMat = new Mat();
        if (mat.NumberOfChannels == 3)
        {
            CvInvoke.CvtColor(mat, convertedMat, Emgu.CV.CvEnum.ColorConversion.Bgr2Bgra);
        }
        else if (mat.NumberOfChannels == 1)
        {
            CvInvoke.CvtColor(mat, convertedMat, Emgu.CV.CvEnum.ColorConversion.Gray2Bgra);
        }
        else if (mat.NumberOfChannels == 4)
        {
            convertedMat = mat;
        }
        else
        {
            throw new NotSupportedException("Unsupported number of channels: " + mat.NumberOfChannels);
        }

        int width = convertedMat.Width;
        int height = convertedMat.Height;
        int bytesPerPixel = 4; // BGRA
        int sizeInBytes = width * height * bytesPerPixel;

        byte[] pixelData = new byte[sizeInBytes];
        Marshal.Copy(convertedMat.DataPointer, pixelData, 0, sizeInBytes);

        // Use the existing CreateImageTexture2D method that accepts ReadOnlySpan<byte>
        Texture texture = textureManager.CreateTexture2D(
            pixelData,
            new Vector2(width, height),
            offset: Vector3.Zero);

        textureManager.backend.CreateImGuiBinding(ref texture);

        return texture;
    }
}
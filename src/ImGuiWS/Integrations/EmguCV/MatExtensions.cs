using System.Numerics;
using System.Runtime.InteropServices;
using Emgu.CV;
using Emgu.CV.CvEnum;
using ImGuiWS.Exceptions;
using ImGuiWS.Renderer;
using ImGuiWS.Utils.Extensions;

namespace ImGuiWS.Integrations.EmguCV;

/// <summary>
///     Extensions for <see cref="Mat" />
/// </summary>
public static class MatExtensions {
    internal static ReadOnlySpan<Byte> GetPixels(this Mat mat) {
        if(mat.IsEmpty) {
            throw new NoDataException("Mat has no Data", String.Empty);
        }

        // Convert Mat to BGRA if necessary
        Mat convertedMat = new();
        if(mat.NumberOfChannels == 3) {
            CvInvoke.CvtColor(mat, convertedMat, ColorConversion.Bgr2Bgra);
        }
        else if(mat.NumberOfChannels == 1) {
            CvInvoke.CvtColor(mat, convertedMat, ColorConversion.Gray2Bgra);
        }
        else if(mat.NumberOfChannels == 4) {
            convertedMat = mat;
        }
        else {
            throw new NotSupportedException("Unsupported number of channels: " + mat.NumberOfChannels);
        }

        Int32 width = convertedMat.Width;
        Int32 height = convertedMat.Height;
        Int32 bytesPerPixel = 4; // BGRA
        Int32 sizeInBytes = width * height * bytesPerPixel;

        Byte[] pixelData = new Byte[sizeInBytes];
        Marshal.Copy(convertedMat.DataPointer, pixelData, 0, sizeInBytes);
        return pixelData.AsSpan();
    }


    /// <summary>
    ///     Converts <see cref="Mat" /> to <see cref="Texture" />
    /// </summary>
    /// <param name="mat">
    ///     Mat Instance
    /// </param>
    /// <param name="textureManager">
    ///     Instance of Texture Manager
    /// </param>
    /// <returns></returns>
    /// <exception cref="NoDataException">
    ///     Thrown if <paramref name="mat" /> is Empty
    /// </exception>
    /// <exception cref="NotSupportedException">
    ///     Thrown if <paramref name="mat" /> has an unsupported amount of Channels
    /// </exception>
    public static Texture AsTexture(this Mat mat, TextureManager textureManager) {
        ReadOnlySpan<Byte> pixelData = GetPixels(mat);

        Texture texture = textureManager.CreateTexture2D(
            pixelData,
            mat.Size.ToVector2(),
            Vector3.Zero);

        textureManager.backend.CreateImGuiBinding(ref texture);

        return texture;
    }

    public static void AsTextureUpdate(this Mat mat, IntPtr id, TextureManager textureManager) {
        ReadOnlySpan<Byte> pixelData = GetPixels(mat);
        textureManager.UpdateTexture2D(id, pixelData, Vector3.Zero);
    }
}
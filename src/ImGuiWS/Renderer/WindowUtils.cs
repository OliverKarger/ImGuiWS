using System.Numerics;
using System.Runtime.InteropServices;
using ImGuiNET;
using ImGuiWS.Logging;
using Serilog;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using Veldrid;

namespace ImGuiWS.Renderer;

/// <summary>
///     Utility Methods for <see cref="MainWindow"/> and <see cref="WindowBackend"/>
/// </summary>
public class WindowUtils(WindowBackend backend, MainWindow window)
{
    private readonly MainWindow _mainWindow = window;
    private readonly WindowBackend _backend = backend;
    private readonly ILogger _logger = LoggerFactory.Create<WindowUtils>();

    /// <summary>
    ///     Loads a <c>*.ttf</c> Font from File
    /// </summary>
    /// <param name="path">
    ///     File Path
    /// </param>
    /// <param name="size">
    ///     Font Size
    /// </param>
    public void LoadFontFromFile(string path, int size)
    {
        if (_backend.State.RenderingBegun)
        {
            _logger.Error("Failed to load Font. Rendering has already begun!");
            throw new InvalidOperationException("Cannot add Font when rendering has already begun!");
        }

        unsafe
        {
            var io = ImGui.GetIO();

            ImFontConfig* config = ImGuiNative.ImFontConfig_ImFontConfig();
            config->OversampleH = 6;
            config->OversampleV = 4;
            config->PixelSnapH = 1;

            ushort* ranges = (ushort*)io.Fonts.GetGlyphRangesDefault();

            byte* nativePath = (byte*)Marshal.StringToHGlobalAnsi(path);

            ImGuiNative.ImFontAtlas_AddFontFromFileTTF(io.Fonts.NativePtr, nativePath, size, config, ranges);

            Marshal.FreeHGlobal((IntPtr)nativePath);

            IntPtr pixels;
            int width, height, bytesPerPixel;
            io.Fonts.GetTexDataAsRGBA32(out pixels, out width, out height, out bytesPerPixel);

            _backend.RecreateFontDeviceTexture();
        }
        
        _logger.Information("Loaded Font {path} ({size}px)", path, size);
    }

    /// <summary>
    ///     Creates new Image Texture
    /// </summary>
    /// <param name="path">
    ///     Path to Image File
    /// </param>
    /// <param name="size">
    ///     Size of the loaded Image
    /// </param>
    /// <returns>
    ///     ImGui Binding ID
    /// </returns>
    public IntPtr CreateImageTexture(string path, out Vector2 size)
    {
        // Load Image Data
        Image<Rgba32> image = Image.Load<Rgba32>(path);
        image.Mutate(x => x.Flip(FlipMode.Vertical));

        size = new Vector2(image.Width, image.Height);
        
        Rgba32[] pixels = new Rgba32[image.Width * image.Height];
        image.CopyPixelDataTo(pixels);
       
        // Create Texture
        Texture texture = _backend.Context.GraphicsDevice.ResourceFactory.CreateTexture(
            TextureDescription.Texture2D(
                (uint)image.Width, (uint)image.Height, mipLevels: 1, arrayLayers: 1,
                PixelFormat.B8_G8_R8_A8_UNorm,
                TextureUsage.Sampled)); 
        
        // Update Texture with Image Data
        _backend.Context.GraphicsDevice.UpdateTexture(
            texture,
            MemoryMarshal.AsBytes(pixels.AsSpan()),
            0, 0, 0,
            (uint)image.Width, (uint)image.Height,
            1, 0, 0);
        
        // Create TextureView
        TextureView textureView = _backend.Context.GraphicsDevice.ResourceFactory.CreateTextureView(texture);
        
        // Create Sampler
        SamplerDescription samplerDescription = new SamplerDescription(
            addressModeU: SamplerAddressMode.Clamp,
            addressModeV: SamplerAddressMode.Clamp,
            addressModeW: SamplerAddressMode.Clamp,
            filter: SamplerFilter.MinLinear_MagLinear_MipLinear,
            comparisonKind: null,
            maximumAnisotropy: 0,
            minimumLod: 0,
            maximumLod: uint.MaxValue,
            lodBias: 0,
            borderColor: SamplerBorderColor.TransparentBlack);
        Sampler sampler = _backend.Context.GraphicsDevice.ResourceFactory.CreateSampler(samplerDescription);
        
        // Create Binding
        IntPtr binding = _backend.GetOrCreateImGuiBinding(textureView);
        if (binding == IntPtr.Zero)
        {
            throw new NullReferenceException("Failed to create ImGui Binding.");
        }

        _logger.Information("Loaded Image {path} as Texture ({x}x{y})", path, size.X, size.Y);
        return binding;
    }
}
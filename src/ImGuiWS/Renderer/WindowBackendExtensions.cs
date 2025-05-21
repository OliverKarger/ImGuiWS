using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using ImGuiNET;
using ImGuiWS.Exceptions;
using ImGuiWS.Utils;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using Veldrid;
using Veldrid.Sdl2;
using Rectangle = Veldrid.Rectangle;

namespace ImGuiWS.Renderer;

public static class WindowBackendExtensions
{
    private static unsafe void _LoadFont(ref FontTexture texture, byte[] fontBytes, int size, ImFontConfig* config, ushort* ranges)
    {
        var io = ImGui.GetIO();

        fixed (byte* fontData = fontBytes)
        {
            ImFont* font = ImGuiNative.ImFontAtlas_AddFontFromMemoryTTF(io.Fonts.NativePtr, fontData, fontBytes.Length, size, config, ranges);
            texture.Handle = new ImFontPtr(font);
        }

        ImGuiNative.ImFontConfig_destroy(config);

        io.Fonts.GetTexDataAsRGBA32(out IntPtr pixels, out int width, out int height, out int bytesPerPixel);
        texture.Pixels = pixels;
        texture.Size = new Vector2(width, height);
        texture.BytesPerPixel = bytesPerPixel;
    }

    private static unsafe void _MergeFont(byte[] fontBytes, int size, ushort[] ranges)
    {
        var io = ImGui.GetIO();

        ImFontConfig* config = ImGuiNative.ImFontConfig_ImFontConfig();
        config->MergeMode = 1;
        config->PixelSnapH = 1;

        fixed (byte* fontData = fontBytes)
        fixed (ushort* glyphRanges = ranges)
        {
            ImGuiNative.ImFontAtlas_AddFontFromMemoryTTF(io.Fonts.NativePtr, fontData, fontBytes.Length, size, config, glyphRanges);
        }

        ImGuiNative.ImFontConfig_destroy(config);
    }

    /// <summary>
    ///     Load Font from Embedded Resources
    /// </summary>
    /// <param name="backend"></param>
    /// <param name="name"></param>
    /// <param name="size"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public static FontTexture LoadEmbeddedFont(this WindowBackend backend, string name, int size)
    {
        if (backend.State.RenderingBegun)
        {
            backend.logger.Error("Failed to load Font. Rendering has already begun!");
            throw new InvalidOperationException("Cannot add Font when rendering has already begun!");
        }

        var fontBytes = EmbeddedResourceManager.GetEmbeddedResourceBytes<WindowBackend>(name);
        if (fontBytes.Length == 0)
            throw new InvalidOperationException("Failed to load Font");

        size = (int)(size * backend.DpiScale);

        if (backend.Context.FontTexture == null)
            backend.Context.FontTexture = new();

        FontTexture fontTexture = backend.Context.FontTexture;

        unsafe
        {
            var io = ImGui.GetIO();

            ImFontConfig* config = ImGuiNative.ImFontConfig_ImFontConfig();
            config->OversampleH = 6;
            config->OversampleV = 4;
            config->PixelSnapH = 1;

            ushort* ranges = (ushort*)io.Fonts.GetGlyphRangesDefault();

            _LoadFont(ref fontTexture, fontBytes, size, config, ranges);
        }

        backend.RecreateFontDeviceTexture();
        backend.logger.Information("Loaded Font from memory: {name} ({size}px)", name, size);

        return fontTexture;
    }

    /// <summary>
    ///     Loads the default Font Setup
    /// </summary>
    /// <param name="backend"></param>
    /// <param name="size"></param>
    /// <exception cref="InvalidOperationException"></exception>
    public static void LoadDefaultFont(this WindowBackend backend, int size)
    {
        if (backend.State.RenderingBegun)
        {
            backend.logger.Error("Failed to load Font. Rendering has already begun!");
            throw new InvalidOperationException("Cannot add Font when rendering has already begun!");
        }

        var roboto = EmbeddedResourceManager.GetEmbeddedResourceBytes<WindowBackend>("Roboto");
        var faSolid = EmbeddedResourceManager.GetEmbeddedResourceBytes<WindowBackend>("FontAwesomeSolid");
        var faRegular = EmbeddedResourceManager.GetEmbeddedResourceBytes<WindowBackend>("FontAwesomeRegular");

        if (roboto.Length == 0 || faSolid.Length == 0 || faRegular.Length == 0)
            throw new InvalidOperationException("One or more required fonts failed to load.");

        size = (int)(size * backend.DpiScale);

        if (backend.Context.FontTexture == null)
            backend.Context.FontTexture = new();

        FontTexture fontTexture = backend.Context.FontTexture;

        unsafe
        {
            var io = ImGui.GetIO();

            // Load Roboto as base font
            ImFontConfig* baseConfig = ImGuiNative.ImFontConfig_ImFontConfig();
            baseConfig->OversampleH = 6;
            baseConfig->OversampleV = 4;
            baseConfig->PixelSnapH = 1;
            baseConfig->MergeMode = 0;

            ushort* baseRanges = (ushort*)io.Fonts.GetGlyphRangesDefault();

            fixed (byte* robotoData = roboto)
            {
                ImFont* robotoFont = ImGuiNative.ImFontAtlas_AddFontFromMemoryTTF(io.Fonts.NativePtr, robotoData, roboto.Length, size, baseConfig, baseRanges);
                fontTexture.Handle = new ImFontPtr(robotoFont);
                fontTexture.Name = "Roboto";
            }

            ImGuiNative.ImFontConfig_destroy(baseConfig);

            // Merge FontAwesome icons
            ushort[] iconRanges = new ushort[] { 0xf000, 0xf3ff, 0 };

            _MergeFont(faSolid, size, iconRanges);
            _MergeFont(faRegular, size, iconRanges);

            // Upload texture to GPU
            io.Fonts.GetTexDataAsRGBA32(out IntPtr pixels, out int width, out int height, out int bytesPerPixel);

            fontTexture.Pixels = pixels;
            fontTexture.Size = new Vector2(width, height);
            fontTexture.BytesPerPixel = bytesPerPixel;
        }

        backend.RecreateFontDeviceTexture();
        backend.logger.Information("Loaded Font with FontAwesome: Roboto + FaSolid900 + FaRegular400 ({size}px)", size);
    }



    /// <summary>
    ///     Returns the Screen Size
    /// </summary>
    /// <param name="backend">
    ///     Window Backend
    /// </param>
    /// <returns></returns>
    public static Vector2 GetScreenSize(this WindowBackend backend)
    {
        int result;
        Rectangle rect;

        unsafe
        {
            result = Sdl2Native.SDL_GetDisplayBounds(0, (Rectangle*)&rect);
        }

        if (result != 0)
        {
            throw new SdlException("Error while calling Sdl2Native.SDL_GetDisplayBounds");
        }

        return new Vector2(rect.Width, rect.Height);
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
    public static IntPtr CreateImageTexture(this WindowBackend backend, string path, out Vector2 size)
    {
        // Load Image Data
        Image<Rgba32> image = Image.Load<Rgba32>(path);
        image.Mutate(x => x.Flip(FlipMode.Vertical));

        size = new Vector2(image.Width, image.Height);
        
        Rgba32[] pixels = new Rgba32[image.Width * image.Height];
        image.CopyPixelDataTo(pixels);
       
        Texture texture = new();
        
        // Create Texture
        texture.Tex = backend.Context.GraphicsDevice.ResourceFactory.CreateTexture(
            TextureDescription.Texture2D(
                (uint)image.Width, (uint)image.Height, mipLevels: 1, arrayLayers: 1,
                PixelFormat.B8_G8_R8_A8_UNorm,
                TextureUsage.Sampled));

        
        
        // Update Texture with Image Data
        backend.Context.GraphicsDevice.UpdateTexture(
            texture.Tex,
            MemoryMarshal.AsBytes(pixels.AsSpan()),
            0, 0, 0,
            (uint)image.Width, (uint)image.Height,
            1, 0, 0);
        
        // Create TextureView
        texture.View = backend.Context.GraphicsDevice.ResourceFactory.CreateTextureView(texture.Tex);
        
        // Create Binding
        texture.ImGuiBinding = backend.GetOrCreateImGuiBinding(texture.View);
        if (texture.ImGuiBinding == IntPtr.Zero)
        {
            throw new NullReferenceException("Failed to create ImGui Binding.");
        }

        backend.Context.Textures.Add(texture);
        
        backend.logger.Information("Loaded Image {path} as Texture ({x}x{y})", path, size.X, size.Y);
        return (IntPtr)texture.ImGuiBinding;
    }

}
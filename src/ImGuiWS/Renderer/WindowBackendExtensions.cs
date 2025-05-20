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
    /// <summary>
    ///     Load TTF-Font from File
    /// </summary>
    /// <param name="backend">
    ///     Window Backend
    /// </param>
    /// <param name="path">
    ///     Path to File
    /// </param>
    /// <param name="size">
    ///     Font Size
    /// </param>
    /// <exception cref="InvalidOperationException">
    ///     Thrown if called after Rendering has Begun
    /// </exception>
    public static void LoadFontFromFile(this WindowBackend backend, string path, int size)
    {
        if (backend.State.RenderingBegun)
        {
            backend.logger.Error("Failed to load Font. Rendering has already begun!");
            throw new InvalidOperationException("Cannot add Font when rendering has already begun!");
        }

        size = (int)(size * backend.DpiScale);

        unsafe
        {
            var io = ImGui.GetIO();

            ImFontConfig* config = ImGuiNative.ImFontConfig_ImFontConfig();
            config->OversampleH = 6;
            config->OversampleV = 4;
            config->PixelSnapH = 1;

            // Allocate unmanaged memory for the font name
            // byte[] nameBytes = Encoding.UTF8.GetBytes(fontName + "\0");
            // IntPtr namePtr = Marshal.AllocHGlobal(nameBytes.Length);
            // Marshal.Copy(nameBytes, 0, namePtr, nameBytes.Length);
            // config->Name = (byte*)namePtr;

            ushort* ranges = (ushort*)io.Fonts.GetGlyphRangesDefault();

            byte* nativePath = (byte*)Marshal.StringToHGlobalAnsi(path);
            ImFont* handle = ImGuiNative.ImFontAtlas_AddFontFromFileTTF(io.Fonts.NativePtr, nativePath, size, config, ranges);

            Marshal.FreeHGlobal((IntPtr)nativePath);
            // Marshal.FreeHGlobal(namePtr); // Free the name memory
            ImGuiNative.ImFontConfig_destroy(config); // Clean up the config

            IntPtr pixels;
            int width, height, bytesPerPixel;
            io.Fonts.GetTexDataAsRGBA32(out pixels, out width, out height, out bytesPerPixel);

            backend.RecreateFontDeviceTexture();
            backend.Context.FontTexture.ImGuiBinding = (IntPtr)handle; 
        }

        backend.logger.Information("Loaded Font {path} ({size}px)", path, size);
    }

    /// <summary>
    ///     Load Font from Memory
    /// </summary>
    /// <param name="backend">Window Backend</param>
    /// <param name="name">Font Name</param>
    /// <param name="size">Font Size</param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException">
    ///     Thrown if called after Rendering has Begun
    /// </exception>
    public static Font LoadFontFromMemory(this WindowBackend backend, string name, int size)
    {
        if (backend.State.RenderingBegun)
        {
            backend.logger.Error("Failed to load Font. Rendering has already begun!");
            throw new InvalidOperationException("Cannot add Font when rendering has already begun!");
        }

        var bytes = EmbeddedResourceManager.GetEmbeddedResourceBytes<WindowBackend>(name);
        if (bytes.Length == 0)
        {
            throw new InvalidOperationException("Failed to load Font");
        }

        size = (int)(size * backend.DpiScale);
        
        Font? newFont;
        unsafe
        {
            var io = ImGui.GetIO();

            ImFontConfig* config = ImGuiNative.ImFontConfig_ImFontConfig();
            config->OversampleH = 6;
            config->OversampleV = 4;
            config->PixelSnapH = 1;

            ushort* ranges = (ushort*)io.Fonts.GetGlyphRangesDefault();

            fixed (byte* fontData = bytes)
            {
                ImFont* font = ImGuiNative.ImFontAtlas_AddFontFromMemoryTTF(io.Fonts.NativePtr, fontData, bytes.Length, size, config, ranges);
                ImFontPtr fontPtr = new ImFontPtr(font);
                newFont = new Font(name, fontPtr, size);
            }

            // Marshal.FreeHGlobal(namePtr); // Free the name memory
            ImGuiNative.ImFontConfig_destroy(config); // Clean up the config

            IntPtr pixels;
            int width, height, bytesPerPixel;
            io.Fonts.GetTexDataAsRGBA32(out pixels, out width, out height, out bytesPerPixel);

            backend.RecreateFontDeviceTexture();
        }

        backend.logger.Information("Loaded Font from memory: {name} ({size}px)", name, size);
        return newFont;
    }

    public static void LoadDefaultFont(this WindowBackend backend, int size)
    {
        if (backend.State.RenderingBegun)
        {
            backend.logger.Error("Failed to load Font. Rendering has already begun!");
            throw new InvalidOperationException("Cannot add Font when rendering has already begun!");
        }

        var robotoBytes = EmbeddedResourceManager.GetEmbeddedResourceBytes<WindowBackend>("Roboto");
        if (robotoBytes.Length == 0)
            throw new InvalidOperationException("Failed to load Font Roboto");

        var faSolidBytes = EmbeddedResourceManager.GetEmbeddedResourceBytes<WindowBackend>("FontAwesomeSolid");
        if (faSolidBytes.Length == 0)
            throw new InvalidOperationException("Failed to load Font FaSolid900");

        var faRegularBytes = EmbeddedResourceManager.GetEmbeddedResourceBytes<WindowBackend>("FontAwesomeRegular");
        if (faRegularBytes.Length == 0)
            throw new InvalidOperationException("Failed to load Font FaRegular400");

        size = (int)(size * backend.DpiScale);

        Font? newFont;
        unsafe
        {
            var io = ImGui.GetIO();

            // 1. Load the base Roboto font
            ImFontConfig* baseConfig = ImGuiNative.ImFontConfig_ImFontConfig();
            baseConfig->OversampleH = 6;
            baseConfig->OversampleV = 4;
            baseConfig->PixelSnapH = 1;
            baseConfig->MergeMode = 0;

            ushort* baseRanges = (ushort*)io.Fonts.GetGlyphRangesDefault();

            fixed (byte* robotoData = robotoBytes)
            {
                ImFont* robotoFont = ImGuiNative.ImFontAtlas_AddFontFromMemoryTTF(io.Fonts.NativePtr, robotoData,
                    robotoBytes.Length, size, baseConfig, baseRanges);
                ImFontPtr fontPtr = new ImFontPtr(robotoFont);
                newFont = new Font("Roboto", fontPtr, size);
            }

            ImGuiNative.ImFontConfig_destroy(baseConfig);

            // 2. Define FontAwesome icon range
            ushort[] iconRangesArray = new ushort[] { 0xf000, 0xf3ff, 0 };
            fixed (ushort* iconRanges = iconRangesArray)
            {
                ImFontConfig* faConfig = ImGuiNative.ImFontConfig_ImFontConfig();
                faConfig->MergeMode = 1;
                faConfig->PixelSnapH = 1;

                // Merge FontAwesome Solid
                fixed (byte* faSolidData = faSolidBytes)
                {
                    ImGuiNative.ImFontAtlas_AddFontFromMemoryTTF(io.Fonts.NativePtr, faSolidData, faSolidBytes.Length,
                        size, faConfig, iconRanges);
                }

                // Merge FontAwesome Regular
                fixed (byte* faRegularData = faRegularBytes)
                {
                    ImGuiNative.ImFontAtlas_AddFontFromMemoryTTF(io.Fonts.NativePtr, faRegularData,
                        faRegularBytes.Length, size, faConfig, iconRanges);
                }

                ImGuiNative.ImFontConfig_destroy(faConfig);
            }

            // Upload texture to GPU
            IntPtr pixels;
            int width, height, bytesPerPixel;
            io.Fonts.GetTexDataAsRGBA32(out pixels, out width, out height, out bytesPerPixel);
            backend.RecreateFontDeviceTexture();
        }

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
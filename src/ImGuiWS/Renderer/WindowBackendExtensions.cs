using System.Numerics;
using ImGuiNET;
using ImGuiWS.Exceptions;
using ImGuiWS.Utils;
using Veldrid.Sdl2;
using Rectangle = Veldrid.Rectangle;

namespace ImGuiWS.Renderer;

public static class WindowBackendExtensions {
    private static unsafe void _LoadFont(ref FontTexture texture, Byte[] fontBytes, Int32 size,
        ImFontConfig* config,
        UInt16* ranges) {
        ImGuiIOPtr io = ImGui.GetIO();

        fixed(Byte* fontData = fontBytes) {
            ImFont* font = ImGuiNative.ImFontAtlas_AddFontFromMemoryTTF(io.Fonts.NativePtr, fontData,
                fontBytes.Length, size, config, ranges);
            texture.Handle = new ImFontPtr(font);
        }

        ImGuiNative.ImFontConfig_destroy(config);

        io.Fonts.GetTexDataAsRGBA32(out IntPtr pixels, out Int32 width, out Int32 height, out Int32 bytesPerPixel);
        texture.Pixels = pixels;
        texture.Size = new Vector2(width, height);
        texture.BytesPerPixel = bytesPerPixel;
    }

    private static unsafe void _MergeFont(Byte[] fontBytes, Int32 size, UInt16[] ranges) {
        ImGuiIOPtr io = ImGui.GetIO();

        ImFontConfig* config = ImGuiNative.ImFontConfig_ImFontConfig();
        config -> MergeMode = 1;
        config -> PixelSnapH = 1;

        fixed(Byte* fontData = fontBytes)
        fixed(UInt16* glyphRanges = ranges) {
            ImGuiNative.ImFontAtlas_AddFontFromMemoryTTF(io.Fonts.NativePtr, fontData, fontBytes.Length, size, config,
                glyphRanges);
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
    public static FontTexture LoadEmbeddedFont(this WindowBackend backend, String name, Int32 size) {
        if(backend.State.RenderingBegun) {
            backend.logger.Error("Failed to load Font. Rendering has already begun!");
            throw new InvalidOperationException("Cannot add Font when rendering has already begun!");
        }

        Byte[] fontBytes = EmbeddedResourceManager.GetEmbeddedResourceBytes<WindowBackend>(name);
        if(fontBytes.Length == 0) {
            throw new InvalidOperationException("Failed to load Font");
        }

        size = (Int32)(size * backend.DpiScale);

        if(backend.Context.FontTexture == null) {
            backend.Context.FontTexture = new FontTexture();
        }

        FontTexture fontTexture = backend.Context.FontTexture;

        unsafe {
            ImGuiIOPtr io = ImGui.GetIO();

            ImFontConfig* config = ImGuiNative.ImFontConfig_ImFontConfig();
            config -> OversampleH = 6;
            config -> OversampleV = 4;
            config -> PixelSnapH = 1;

            UInt16* ranges = (UInt16*)io.Fonts.GetGlyphRangesDefault();

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
    public static void LoadDefaultFont(this WindowBackend backend, Int32 size) {
        if(backend.State.RenderingBegun) {
            backend.logger.Error("Failed to load Font. Rendering has already begun!");
            throw new InvalidOperationException("Cannot add Font when rendering has already begun!");
        }

        Byte[] roboto = EmbeddedResourceManager.GetEmbeddedResourceBytes<WindowBackend>("Roboto");
        Byte[] faSolid = EmbeddedResourceManager.GetEmbeddedResourceBytes<WindowBackend>("FontAwesomeSolid");
        Byte[] faRegular =
            EmbeddedResourceManager.GetEmbeddedResourceBytes<WindowBackend>("FontAwesomeRegular");

        if(roboto.Length == 0 || faSolid.Length == 0 || faRegular.Length == 0) {
            throw new InvalidOperationException("One or more required fonts failed to load.");
        }

        size = (Int32)(size * backend.DpiScale);

        if(backend.Context.FontTexture == null) {
            backend.Context.FontTexture = new FontTexture();
        }

        FontTexture fontTexture = backend.Context.FontTexture;

        unsafe {
            ImGuiIOPtr io = ImGui.GetIO();

            // Load Roboto as base font
            ImFontConfig* baseConfig = ImGuiNative.ImFontConfig_ImFontConfig();
            baseConfig -> OversampleH = 6;
            baseConfig -> OversampleV = 4;
            baseConfig -> PixelSnapH = 1;
            baseConfig -> MergeMode = 0;

            UInt16* baseRanges = (UInt16*)io.Fonts.GetGlyphRangesDefault();

            fixed(Byte* robotoData = roboto) {
                ImFont* robotoFont = ImGuiNative.ImFontAtlas_AddFontFromMemoryTTF(io.Fonts.NativePtr, robotoData,
                    roboto.Length, size, baseConfig, baseRanges);
                fontTexture.Handle = new ImFontPtr(robotoFont);
                fontTexture.Name = "Roboto";
            }

            ImGuiNative.ImFontConfig_destroy(baseConfig);

            // Merge FontAwesome icons
            UInt16[] iconRanges = new UInt16[] { 0xf000, 0xf3ff, 0 };

            _MergeFont(faSolid, size, iconRanges);
            _MergeFont(faRegular, size, iconRanges);

            // Upload texture to GPU
            io.Fonts.GetTexDataAsRGBA32(out IntPtr pixels, out Int32 width, out Int32 height,
                out Int32 bytesPerPixel);

            fontTexture.Pixels = pixels;
            fontTexture.Size = new Vector2(width, height);
            fontTexture.BytesPerPixel = bytesPerPixel;
        }

        backend.RecreateFontDeviceTexture();
        backend.logger.Information("Loaded Font with FontAwesome: Roboto + FaSolid900 + FaRegular400 ({size}px)",
            size);
    }


    /// <summary>
    ///     Returns the Screen Size
    /// </summary>
    /// <param name="backend">
    ///     Window Backend
    /// </param>
    /// <returns></returns>
    public static Vector2 GetScreenSize(this WindowBackend backend) {
        Int32 result;
        Rectangle rect;

        unsafe {
            result = Sdl2Native.SDL_GetDisplayBounds(0, &rect);
        }

        if(result != 0) {
            throw new SdlException("Error while calling Sdl2Native.SDL_GetDisplayBounds");
        }

        return new Vector2(rect.Width, rect.Height);
    }
}
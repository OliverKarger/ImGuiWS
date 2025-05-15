using System.Runtime.InteropServices;
using ImGuiNET;

namespace ImGuiWS.Renderer;

public class WindowUtils
{
    private readonly WindowBackend _backend;
    private readonly MainWindow _mainWindow;
    public WindowUtils(WindowBackend backend, MainWindow window)
    {
        _mainWindow = window;
        _backend = backend;
    }

    public void LoadFontFromFile(string path, int size)
    {
        if (_backend.State.RenderingBegun)
        {
            throw new Exception("Cannot add Font when rendering has already begun!");
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

            _backend.RecreateFontDeviceTexture(_backend.Context.GraphicsDevice);
        }
    }

}
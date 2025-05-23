using System.Numerics;
using System.Runtime.InteropServices;
using Veldrid;
using Veldrid.Sdl2;

namespace ImGuiWS.Renderer;

public class WindowSetupOptions {
    public WindowSetupOptions(String title, Vector2 size, Vector2 position) {
        this.Title = title;
        this.Size = size;
        this.Position = position;

        this.Flags = SDL_WindowFlags.AllowHighDpi | SDL_WindowFlags.Resizable;
        this.VSync = true;

        if(RuntimeInformation.IsOSPlatform(OSPlatform.OSX)) {
            this.Backend = GraphicsBackend.Metal;
        }
        else if(RuntimeInformation.IsOSPlatform(OSPlatform.Linux)) {
            this.Backend = GraphicsBackend.OpenGL;
        }
        else if(RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) {
            this.Backend = GraphicsBackend.Direct3D11;
        }
        else {
            this.Backend = GraphicsBackend.Vulkan;
        }
    }

    public String Title { get; set; }
    public Vector2 Size { get; set; }
    public Vector2 Position { get; set; }
    public SDL_WindowFlags Flags { get; set; }
    public Boolean VSync { get; set; } = true;
    public GraphicsBackend Backend { get; set; }
}
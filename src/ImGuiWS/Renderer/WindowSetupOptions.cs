using System.Numerics;
using System.Runtime.InteropServices;
using Veldrid;
using Veldrid.Sdl2;

namespace ImGuiWS.Renderer;

public class WindowSetupOptions
{
    public WindowSetupOptions(string title, Vector2 size, Vector2 position)
    {
        Title = title;
        Size = size;
        Position = position;

        Flags = SDL_WindowFlags.AllowHighDpi | SDL_WindowFlags.Resizable;
        VSync = true;

        if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            Backend = GraphicsBackend.Metal;
        } 
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            Backend = GraphicsBackend.OpenGL;
        } 
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            Backend = GraphicsBackend.Direct3D11;
        }
        else
        {
            Backend = GraphicsBackend.Vulkan;
        }
    }
    
    public string Title { get; set; }
    public Vector2 Size { get; set; }
    public Vector2 Position { get; set; }
    public SDL_WindowFlags Flags { get; set; }
    public bool VSync { get; set; } = true;
    public GraphicsBackend Backend { get; set; }
    
}
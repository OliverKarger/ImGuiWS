using System.Numerics;
using ImGuiNET;
using ImGuiWS.Utils.Extensions;
using Veldrid;
using Veldrid.Sdl2;

namespace ImGuiWS.Renderer;

public class Font(string name, ImFontPtr fontPtr, int size)
{
    public readonly string Name = name;
    public readonly ImFontPtr FontPtr = fontPtr;
    public readonly int FontSize = size;
}

/// <summary>
///     Resource/Object Context for Window Backend
/// </summary>
public class WindowBackendContext(WindowBackend _backend) : IDisposable
{
    public GraphicsDevice GraphicsDevice { get; internal set; }
    public Sdl2Window Window { get; internal set; }
    public CommandList CommandList { get; internal set; }
    public DeviceBuffer VertexBuffer { get; internal set; }
    public DeviceBuffer IndexBuffer { get; internal set; }
    public DeviceBuffer ProjectionBuffer { get; internal set; }
    public FontTexture? FontTexture { get; internal set; }
    public Shader VertexShader { get; internal set; }
    public Shader FragmentShader { get; internal set; }
    public ResourceLayout ResLayout { get; internal set; }
    public ResourceLayout TexLayout { get; internal set; }
    public Pipeline Pipeline { get; internal set; }
    public ResourceSet MainResourceSet { get; internal set; }
    public ResourceSet FontTextureResourceSet { get; internal set; }
    public TextureManager Textures { get; internal set; } = new(_backend);

    public void Dispose()
    {   
        VertexBuffer?.Dispose();
        IndexBuffer?.Dispose();
        ProjectionBuffer?.Dispose();
        FontTexture?.Dispose();
        VertexShader?.Dispose();
        FragmentShader?.Dispose();
        ResLayout?.Dispose();
        TexLayout?.Dispose();
        Pipeline?.Dispose();
        MainResourceSet?.Dispose();
        FontTextureResourceSet?.Dispose();
        Textures.Dispose();
    }
}
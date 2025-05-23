using ImGuiNET;
using Veldrid;
using Veldrid.Sdl2;

namespace ImGuiWS.Renderer;

public class Font(String name, ImFontPtr fontPtr, Int32 size) {
    public readonly ImFontPtr FontPtr = fontPtr;
    public readonly Int32 FontSize = size;
    public readonly String Name = name;
}

/// <summary>
///     Resource/Object Context for Window Backend
/// </summary>
public class WindowBackendContext(WindowBackend _backend) : IDisposable {
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

    public void Dispose() {
        this.VertexBuffer?.Dispose();
        this.IndexBuffer?.Dispose();
        this.ProjectionBuffer?.Dispose();
        this.FontTexture?.Dispose();
        this.VertexShader?.Dispose();
        this.FragmentShader?.Dispose();
        this.ResLayout?.Dispose();
        this.TexLayout?.Dispose();
        this.Pipeline?.Dispose();
        this.MainResourceSet?.Dispose();
        this.FontTextureResourceSet?.Dispose();
        this.Textures.Dispose();
    }
}
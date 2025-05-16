using System.Numerics;
using ImGuiWS.Utils.Extensions;
using Veldrid;
using Veldrid.Sdl2;

namespace ImGuiWS.Renderer;

/// <summary>
///     Resource/Object Context for Window Backend
/// </summary>
public class WindowBackendContext : IDisposable
{
    public GraphicsDevice GraphicsDevice { get; internal set; }
    public Sdl2Window Window { get; internal set; }
    public CommandList CommandList { get; internal set; }
    public DeviceBuffer VertexBuffer { get; internal set; }
    public DeviceBuffer IndexBuffer { get; internal set; }
    public DeviceBuffer ProjectionBuffer { get; internal set; }
    public Texture FontTexture { get; internal set; }
    public TextureView FontTextureView { get; internal set; }
    public Shader VertexShader { get; internal set; }
    public Shader FragmentShader { get; internal set; }
    public ResourceLayout ResLayout { get; internal set; }
    public ResourceLayout TexLayout { get; internal set; }
    public Pipeline Pipeline { get; internal set; }
    public ResourceSet MainResourceSet { get; internal set; }
    public ResourceSet FontTextureResourceSet { get; internal set; }
    
    public ICollection<IDisposable> OwnedResources { get; internal set; }
    
    public Dictionary<TextureView, ResourceSetInfo> SetsByView { get; internal set; } = new();
    public Dictionary<Texture, TextureView> AutoViewsByTexture { get; internal set; } = new();
    public Dictionary<IntPtr, ResourceSetInfo> ViewsById { get; internal set; } = new();

    
    public void Dispose()
    {   
        VertexBuffer?.Dispose();
        IndexBuffer?.Dispose();
        ProjectionBuffer?.Dispose();
        FontTexture?.Dispose();
        FontTextureView?.Dispose();
        VertexShader?.Dispose();
        FragmentShader?.Dispose();
        ResLayout?.Dispose();
        TexLayout?.Dispose();
        Pipeline?.Dispose();
        MainResourceSet?.Dispose();
        FontTextureResourceSet?.Dispose();
        
        ClearCachedResources();
    }

    public void ClearCachedResources()
    {
        OwnedResources?.ForEach(e => e.Dispose());
        OwnedResources?.Clear();
        
        SetsByView?.Clear();
        ViewsById?.Clear();
        AutoViewsByTexture?.Clear();
        ViewsById?.Clear();
    }
}
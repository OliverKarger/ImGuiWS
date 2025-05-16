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
    public readonly int Size = size;
}

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

    public HashSet<IDisposable> OwnedResources { get; internal set; } = new();
    
    public Dictionary<TextureView, ResourceSetInfo> SetsByView { get; internal set; } = new();
    public Dictionary<Texture, TextureView> AutoViewsByTexture { get; internal set; } = new();
    public Dictionary<IntPtr, ResourceSetInfo> ViewsById { get; internal set; } = new();
    
    public HashSet<Font> Fonts { get; internal set; } = new();
    
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
        Fonts.Clear();
    }
}
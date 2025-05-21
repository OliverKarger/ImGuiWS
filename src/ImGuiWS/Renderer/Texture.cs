using Veldrid;

namespace ImGuiWS.Renderer;

/// <summary>
///     Texture
/// </summary>
public class Texture : IDisposable
{
    /// <summary>
    ///     Associated Texture View
    /// </summary>
    public TextureView View { get; set; } 
    
    /// <summary>
    ///     Raw Veldrid Texture
    /// </summary>
    public Veldrid.Texture Tex { get; set; }
    
    /// <summary>
    ///     Associated Resource Set
    /// </summary>
    public ResourceSet ResourceSet { get; set; }
    
    /// <summary>
    ///     ImGui Binding ID if available
    /// </summary>
    public IntPtr? ImGuiBinding { get; set; }
    
    /// <summary>
    ///     Display Name of the Texture
    /// </summary>
    public string? Name { get; set; }

    public Texture(TextureView view, Veldrid.Texture texture, ResourceSet resourceSet)
    {
        View = view;
        Tex = texture;
        ResourceSet = resourceSet;
    }

    public Texture(TextureView view, Veldrid.Texture texture, ResourceSet resourceSet, IntPtr imGuiHandle)
    {
        View = view;
        Tex = texture;
        ResourceSet = resourceSet;
        ImGuiBinding = imGuiHandle;
    }
    
    public Texture() {}

    public void Dispose()
    {
        View.Dispose();
        Tex.Dispose();
    }
}
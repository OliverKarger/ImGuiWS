using Veldrid;

namespace ImGuiWS.Renderer;

public class Texture : IDisposable
{
    public TextureView View { get; set; } 
    public Veldrid.Texture Tex { get; set; }
    public ResourceSet ResourceSet { get; set; }
    public IntPtr? ImGuiBinding { get; set; }
    
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
using System.Numerics;
using Veldrid;

namespace ImGuiWS.Renderer;

/// <summary>
///     Texture
/// </summary>
public class Texture : IDisposable {
    public Texture(TextureView view, Veldrid.Texture texture, ResourceSet resourceSet) {
        this.View = view;
        this.Tex = texture;
        this.ResourceSet = resourceSet;
    }

    public Texture(TextureView view, Veldrid.Texture texture, ResourceSet resourceSet, IntPtr imGuiHandle) {
        this.View = view;
        this.Tex = texture;
        this.ResourceSet = resourceSet;
        this.Id = imGuiHandle;
    }

    public Texture() { }

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
    public IntPtr? Id { get; set; }

    /// <summary>
    ///     Display Name of the Texture
    /// </summary>
    public String? Name { get; set; }

    public Vector2 Size => new(this.Tex.Width, this.Tex.Height);

    public void Dispose() {
        this.View.Dispose();
        this.Tex.Dispose();
    }
}
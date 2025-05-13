using Veldrid;

namespace ImGuiWS.Renderer;

public class TextureContainer(Texture texture, TextureView textureView) : IDisposable
{
    public Texture Texture { get; set; } = texture;
    public TextureView TextureView { get; set; } = textureView;

    public void Dispose()
    {
        Texture.Dispose();
        TextureView.Dispose();
    }
}
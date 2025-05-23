using System.Numerics;
using ImGuiWS.Logging;
using ImGuiWS.Utils.Extensions;
using Serilog;
using Veldrid;

namespace ImGuiWS.Renderer;

public class TextureManager : IDisposable {
    private readonly HashSet<Texture> _textures = new();
    internal readonly WindowBackend backend;
    internal readonly ILogger logger;

    public TextureManager(WindowBackend backend) {
        this.backend = backend;
        this.logger = LoggerFactory.Create<TextureManager>(backend.MainWindow.Id);
    }

    public void Dispose() {
        this._textures.ForEach(e => e.Dispose());
        this._textures.Clear();
    }

    public Texture? GetByBindingId(IntPtr bindingId) {
        return this._textures.FirstOrDefault(e => e.Id == bindingId);
    }

    private void UpdateTexture(Veldrid.Texture texture, ReadOnlySpan<Byte> pixels, Vector2 size, Vector3 offset) {
        this.backend.Context.GraphicsDevice.UpdateTexture(
            texture,
            pixels,
            (UInt32)offset.X, (UInt32)offset.Y, (UInt32)offset.Z,
            (UInt32)size.X, (UInt32)size.Y,
            1,
            0,
            0);
    }

    public Texture CreateTexture2D(
        ReadOnlySpan<Byte> pixels,
        Vector2 size,
        Vector3 offset = default) {
        Texture texture = new();
        TextureDescription textureDesc = TextureDescription.Texture2D(
            (UInt32)size.X, (UInt32)size.Y,
            1, 1, PixelFormat.B8_G8_R8_A8_UNorm, TextureUsage.Sampled);

        texture.Tex = this.backend.Context.GraphicsDevice.ResourceFactory.CreateTexture(textureDesc);
        texture.View = this.backend.Context.GraphicsDevice.ResourceFactory.CreateTextureView(texture.Tex);
        texture.Id = this.backend.CreateImGuiBinding(ref texture);

        this.UpdateTexture(texture.Tex, pixels, size, offset);

        if(texture.Id == IntPtr.Zero) {
            throw new Exception("Failed to create ImGui Binding for Texture");
        }

        this.logger.Debug("Created Texture");
        this._textures.Add(texture);
        return texture;
    }

    public void UpdateTexture2D(IntPtr id, ReadOnlySpan<Byte> pixels, Vector3 offset = default) {
        Texture? texture = this.GetByBindingId(id);
        if(texture == null) {
            this.logger.Error("Could not Update Texture. Id {id} does not exist", id);
            return;
        }

        this.UpdateTexture(texture.Tex, pixels, texture.Size, offset);
    }
}
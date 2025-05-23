using System.Numerics;
using System.Runtime.InteropServices;
using Emgu.CV;
using ImGuiWS.Logging;
using ImGuiWS.Utils.Extensions;
using Serilog;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using Veldrid;

namespace ImGuiWS.Renderer;

public class TextureManager : IDisposable
{
    private HashSet<Texture> _textures = new();
    internal readonly WindowBackend backend;
    internal readonly ILogger logger;
    
    public TextureManager(WindowBackend backend)
    {
        this.backend = backend;
        logger = LoggerFactory.Create<TextureManager>(backend.MainWindow.Id);
    }

    public Texture? GetByBindingId(IntPtr bindingId)
    {
        return _textures.FirstOrDefault(e => e.Id == bindingId);
    }

    private void UpdateTexture(Veldrid.Texture texture, ReadOnlySpan<byte> pixels, Vector2 size, Vector3 offset)
    {
        backend.Context.GraphicsDevice.UpdateTexture(
            texture,
            pixels,
            (uint)offset.X, (uint)offset.Y, (uint)offset.Z,
            (uint)size.X, (uint)size.Y, 
            1, 
            0, 
            0);
    }
    
    public Texture CreateTexture2D(
        ReadOnlySpan<byte> pixels,
        Vector2 size,
        Vector3 offset = default)
    {
        Texture texture = new();
        TextureDescription textureDesc = TextureDescription.Texture2D(
            (uint)size.X, (uint)size.Y,
            1,1,PixelFormat.B8_G8_R8_A8_UNorm,TextureUsage.Sampled);
        
        texture.Tex = backend.Context.GraphicsDevice.ResourceFactory.CreateTexture(textureDesc);
        texture.View = backend.Context.GraphicsDevice.ResourceFactory.CreateTextureView(texture.Tex);
        texture.Id = backend.CreateImGuiBinding(ref texture);
        
        UpdateTexture(texture.Tex, pixels, size, offset);
        
        if (texture.Id == IntPtr.Zero)
        {
            throw new Exception("Failed to create ImGui Binding for Texture");
        }
        
        logger.Debug("Created Texture");
        _textures.Add(texture);
        return texture;
    }

    public void UpdateTexture2D(IntPtr id, ReadOnlySpan<byte> pixels, Vector3 offset = default)
    {
        Texture? texture = GetByBindingId(id);
        if (texture == null)
        {
            logger.Error("Could not Update Texture. Id {id} does not exist", id);
            return;
        }
        
        UpdateTexture(texture.Tex, pixels, texture.Size, offset);
    }
    
    public void Dispose()
    {
        _textures.ForEach(e => e.Dispose());
        _textures.Clear();
    }
}
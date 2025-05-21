using ImGuiWS.Utils.Extensions;
using Veldrid;

namespace ImGuiWS.Renderer;

/// <summary>
///     Managed Collection of <see cref="Texture"/>
/// </summary>
public class TextureCollection : HashSet<Texture>, IDisposable
{
    /// <summary>
    ///     Returns <see cref="Texture"/> by <see cref="TextureView"/>
    /// </summary>
    /// <param name="view"></param>
    /// <param name="texture"></param>
    /// <returns></returns>
    public bool GetByView(TextureView view, out Texture? texture)
    {
     texture = this.FirstOrDefault(e => e.View == view);
     return texture != null;
    }

    /// <summary>
    ///      Gets <see cref="Texture"/> by <see cref="Veldrid.Texture"/>
    /// </summary>
    /// <param name="veldridTexture"></param>
    /// <param name="texture"></param>
    /// <returns></returns>
    public bool GetByTexture(Veldrid.Texture veldridTexture, out Texture? texture)
    {
     texture = this.FirstOrDefault(e => e.Tex == veldridTexture);
     return texture != null;
    }

    /// <summary>
    ///      Gets Texture by ImGui Binding Id
    /// </summary>
    /// <param name="bindingId"></param>
    /// <param name="texture"></param>
    /// <returns></returns>
    public bool GetByBindingId(IntPtr bindingId, out Texture? texture)
    {
     texture = this.FirstOrDefault(e => e.ImGuiBinding == bindingId);
     return texture != null;
    }

    public void Dispose()
    {
     this.ForEach(e => e.Dispose());
     Clear();
    }
}
using ImGuiWS.Utils.Extensions;
using Veldrid;

namespace ImGuiWS.Renderer;

public class TextureCollection : HashSet<Texture>, IDisposable
{
     public bool GetByView(TextureView view, out Texture? texture)
     {
         texture = this.FirstOrDefault(e => e.View == view);
         return texture != null;
     }

     public bool GetByTexture(Veldrid.Texture texture1, out Texture? texture)
     {
         texture = this.FirstOrDefault(e => e.Tex == texture1);
         return texture != null;
     }

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
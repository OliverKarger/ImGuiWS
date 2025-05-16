using System.Numerics;

namespace ImGuiWS.Controls;

public class Image(IntPtr bindingId, string id) : ControlBase(id)
{
    public IntPtr BindingId { get; set; } = bindingId;
    public Vector2 ImageSize { get; set; } = Vector2.Zero;
    
    public override void Start()
    {
    }

    public override void Update()
    {
        throw new NotImplementedException();
    }

    public override void Shutdown()
    {
        throw new NotImplementedException();
    }
}
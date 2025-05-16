using System.Numerics;
using ImGuiNET;

namespace ImGuiWS.Controls;

public class Image(string id) : ControlBase(id)
{
    /// <summary>
    ///     ImGui Binding Id
    /// </summary>
    public IntPtr BindingId { get; set; } = IntPtr.Zero;
    
    /// <summary>
    ///     Size of the Image
    /// </summary>
    public Vector2 ImageSize { get; set; } = Vector2.Zero;
    
    /// <summary>
    ///     Path to the Image
    /// </summary>
    public string ImagePath { get; set; } = string.Empty;
    
    /// <summary>
    ///     Loads the Image if <c>BindingId</c> is <see cref="IntPtr.Zero"/>
    /// </summary>
    public override void Start()
    {
        if (BindingId == IntPtr.Zero)
        {
            var imageSize = ImageSize;
            BindingId = RootWindow.Utils.CreateImageTexture(ImagePath, out imageSize);
            ImageSize = imageSize;
        }
    }

    public override void Update()
    {
        if (BindingId != IntPtr.Zero)
        {
            ImGui.Image(BindingId!, ImageSize);
        }
    }

    public override void Shutdown()
    {
        throw new NotImplementedException();
    }
}
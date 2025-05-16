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
    ///     Size of the Original Image
    /// </summary>
    public Vector2 SourceImageSize { get; private set; } = Vector2.Zero;

    private float _scaleFactor = 1.0f;
    
    /// <summary>
    ///     Scale Factor
    /// </summary>
    public float ScaleFactor
    {
        get => _scaleFactor;
        set
        {
            _scaleFactor = value;
            ScaledImageSize = SourceImageSize * value;
        }
    }

    private Vector2 ScaledImageSize { get; set; } = Vector2.Zero; 
    
    /// <summary>
    ///     Path to the Image
    /// </summary>
    public string ImagePath { get; set; } = string.Empty;

    private bool _errorShown = false;
    
    /// <summary>
    ///     Loads the Image if <c>BindingId</c> is <see cref="IntPtr.Zero"/>
    /// </summary>
    public override void Start()
    {
        if (BindingId == IntPtr.Zero)
        {
            var imageSize = SourceImageSize;
            BindingId = RootWindow.Utils.CreateImageTexture(ImagePath, out imageSize);
            SourceImageSize = imageSize;
            ScaledImageSize = SourceImageSize * _scaleFactor;
        }
    }

    public override void Update()
    {
        if (BindingId == IntPtr.Zero)
        {
            if (!_errorShown)
            {
                Console.WriteLine("WARNING: BindingId is null!");
                _errorShown = true;
            }
        }
        
        ImGui.Image(BindingId!, ScaledImageSize);
    }

    public override void Shutdown()
    {
        throw new NotImplementedException();
    }
}
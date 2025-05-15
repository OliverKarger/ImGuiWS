namespace ImGuiWS.Renderer;

/// <summary>
///     State of Modifier Keys
/// </summary>
public class WindowModifierKeyState
{
    public bool ControlDown { get; internal set; }
    public bool ShiftDown { get; internal set; }
    public bool AltDown { get; internal set; }
    public bool WindowKeyDown { get; internal set; }
}
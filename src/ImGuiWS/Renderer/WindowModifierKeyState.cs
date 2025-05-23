namespace ImGuiWS.Renderer;

/// <summary>
///     State of Modifier Keys
/// </summary>
public class WindowModifierKeyState {
    public Boolean ControlDown { get; internal set; }
    public Boolean ShiftDown { get; internal set; }
    public Boolean AltDown { get; internal set; }
    public Boolean WindowKeyDown { get; internal set; }
}
using ImGuiWS.Controls;

namespace ImGuiWS.Events;

/// <summary>
///     Event Args for <see cref="ClickableControl"/> OnClick Event
/// </summary>
public class OnClickEventArgs(string controlId) : EventArgs
{
    /// <summary>
    ///     Id of the clicked Control
    /// </summary>
    public string ControlId { get; set; } = controlId;

    /// <summary>
    ///     Handle to the Actor
    /// </summary>
    public IntPtr Actor { get; set; }
}
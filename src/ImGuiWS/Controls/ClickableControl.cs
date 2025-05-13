using ImGuiWS.Events;

namespace ImGuiWS.Controls;

/// <summary>
///     Base Class for all Controls that provide a
///     "clickable" Interaction
/// </summary>
/// <param name="id">
///     <inheritdoc cref="Control"/>
/// </param>
public class ClickableControl(string id) : Control(id)
{
    public event Action<OnClickEventArgs>? OnClick;

    internal virtual void SimulateClick()
    {
        OnClick?.Invoke(new OnClickEventArgs(Id));
    }

    public void AddOnClickEventListener(Action<OnClickEventArgs> action)
    {
        OnClick += action;
    }

    public void RemoveOnClickEventListener(Action<OnClickEventArgs> action)
    {
        OnClick -= action;
    }
}
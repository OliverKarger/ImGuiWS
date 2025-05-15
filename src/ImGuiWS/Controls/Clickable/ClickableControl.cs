namespace ImGuiWS.Controls;

public abstract class ClickableControl(string id) : ControlBase(id)
{
    public event Action? OnClick;
    internal void Click() => OnClick?.Invoke();
}
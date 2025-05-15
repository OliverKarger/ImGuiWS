namespace ImGuiWS.Controls;

public abstract class ControlBase(string id)
{
    public readonly string Id = id;
    public abstract void Render();
}
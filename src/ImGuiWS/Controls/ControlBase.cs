namespace ImGuiWS.Controls;

public abstract class ControlBase(string id)
{
    public readonly string Id = id;
    public abstract void Render();

    public override bool Equals(object obj)
    {
        if (obj is ControlBase control)
        {
            return Id == control.Id;
        }

        return false;
    }

    public override int GetHashCode()
    {
        return Id?.GetHashCode() ?? 0;
    }
}
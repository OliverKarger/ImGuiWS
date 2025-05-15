using ImGuiWS.Controls;

namespace ImGuiWS;

public class WindowControlsCollection
{
    internal HashSet<ControlBase> Controls = new HashSet<ControlBase>();

    public WindowControlsCollection AddControl(ControlBase control)
    {
        Controls.Add(control);
        return this;
    }

    public WindowControlsCollection AddControl<T>(Func<T> factory, Action<T>? configure) where T : ControlBase
    {
        T control = factory();
        configure?.Invoke(control);

        Controls.Add(control);
        return this;
    }
    
    internal void Render()
    {
        foreach (var control in Controls)
        {
            control.Render();
        }
    }
}
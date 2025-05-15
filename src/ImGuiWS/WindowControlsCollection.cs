using System.Data;
using ImGuiNET;
using ImGuiWS.Controls;
using ImGuiWS.Controls.Utils;

namespace ImGuiWS;

public class WindowControlsCollection
{
    private readonly HashSet<ControlBase> Controls = new HashSet<ControlBase>();

    public WindowControlsCollection Add(ControlBase control)
    {
        if (Controls.Any(e => e.Id == control.Id))
        {
           throw new DuplicateNameException("Duplicate control name/id"); 
        }
        
        Controls.Add(control);
        return this;
    }

    public WindowControlsCollection Add<T>(Func<T> factory, Action<T>? configure) where T : ControlBase
    {
        T control = factory();
        configure?.Invoke(control);

        return Add(control);
    }

    public T GetById<T>(string id) where T : ControlBase
    {
        var found = Controls.FirstOrDefault(e => e.Id == id);

        if (found == null)
        {
            throw new KeyNotFoundException($"Control with id {id} not found");
        }
        
        if (typeof(T) != found.GetType())
        {
            throw new Exception($"Type {found.GetType().Name} of Control {id} does not match with {typeof(T).Name}");
        }

        return (found as T)!;
    }

    public T GetByName<T>(string name) where T : ControlBase
    {
        return GetById<T>(name.ToControlId());
    }

    internal void Render()
    {
        foreach (var control in Controls)
        {
            ImGui.PushID(control.Id);
            control.Render();
            ImGui.PopID();
        }
    }
}
using ImGuiNET;
using ImGuiWS.Utils;

namespace ImGuiWS.Components.Navigation;

public class MenuItem : RenderableComponent
{
    public string Label { get; set; }
    public string Shortcut { get; set; } = string.Empty;
    public bool Selected { get; set; } = false;
    public bool Enabled { get; set; } = true;

    public event Action? OnClick;
    public event Action? OnSelected;
    public event Action? OnDeselected;
    
    public MenuItem(string label) : base(label.ToControlId())
    {
        Label = label;
    }

    public MenuItem(string label, Window? parentWindow = null) : base(label.ToControlId(), parentWindow)
    {
        Label = label;
    }

    public override void Update(float delta)
    {
        bool selected = Selected;
        
        if (ImGui.MenuItem(Label, Shortcut, ref selected, Enabled))
        {
            OnClick?.Invoke(); 
        }

        if (Selected == selected) return;
        
        Selected = selected;
        if (Selected)
        {
            OnSelected?.Invoke();
        }
        else
        {
            OnDeselected?.Invoke();
        }
    }
}
using ImGuiWS.Utils;

namespace ImGuiWS.Components.Controls;

public class DelegateControl : Control
{
    public Action? Delegate { get; set; }
    
    public DelegateControl(string id, Window window) : base(id.ToControlId(), window)
    {
    }

    public DelegateControl(string id) : base(id.ToControlId())
    {
    }

    public override void Update(float delta)
    {
        if (!Visible) return;
        Delegate?.Invoke();
    }
}
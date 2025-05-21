using ImGuiWS.Utils;

namespace ImGuiWS.Components.Controls;

/// <summary>
///     Provides a Callback-based Control which
///     can be customized by the implementor
/// </summary>
public class DelegateControl : Control
{
    /// <summary>
    ///     Callback Delegate
    /// </summary>
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
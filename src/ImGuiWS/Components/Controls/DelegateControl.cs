using ImGuiWS.Utils;

namespace ImGuiWS.Components.Controls;

/// <summary>
///     Provides a Callback-based Control which
///     can be customized by the implementor
/// </summary>
public class DelegateControl : Control {
    public DelegateControl(String id, Window window) : base(id.ToControlId(), window) { }

    public DelegateControl(String id) : base(id.ToControlId()) { }

    /// <summary>
    ///     Callback Delegate
    /// </summary>
    public Action? Delegate { get; set; }

    public override void Update(Single delta) {
        if(!this.Visible) {
            return;
        }

        this.Delegate?.Invoke();
    }
}
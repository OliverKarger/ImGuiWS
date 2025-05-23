using ImGuiNET;
using ImGuiWS.Utils;

namespace ImGuiWS.Components.Navigation;

/// <summary>
///     Menu Item
/// </summary>
public class MenuItem : RenderableComponent {
    public MenuItem(String label) : base(label.ToControlId()) {
        this.Label = label;
    }

    public MenuItem(String label, Window? parentWindow = null) : base(label.ToControlId(), parentWindow) {
        this.Label = label;
    }

    public override void Update(Single delta) {
        Boolean selected = this.Selected;

        if(ImGui.MenuItem(this.Label, this.Shortcut, ref selected, this.Enabled)) {
            this.OnClick?.Invoke();
        }

        if(this.Selected == selected) {
            return;
        }

        this.Selected = selected;
        if(this.Selected) {
            this.OnSelected?.Invoke();
        }
        else {
            this.OnDeselected?.Invoke();
        }
    }

    #region "Properties"

    /// <summary>
    ///     Menu Item Label
    /// </summary>
    public String Label { get; set; }

    /// <summary>
    ///     Menu Item Shortcut
    /// </summary>
    /// <example>
    ///     ALT+F4
    /// </example>
    public String Shortcut { get; set; } = String.Empty;

    /// <summary>
    ///     Selected / Active Flag
    /// </summary>
    public Boolean Selected { get; set; }

    /// <summary>
    ///     Enabled Flag
    /// </summary>
    public Boolean Enabled { get; set; } = true;

    #endregion

    #region Events

    /// <summary>
    ///     Invoked when Menu Item is clicked
    /// </summary>
    public event Action? OnClick;

    /// <summary>
    ///     Invoked when Item is activated / selected
    /// </summary>
    public event Action? OnSelected;

    /// <summary>
    ///     Invoked when Item is deactivated / deselected
    /// </summary>
    public event Action? OnDeselected;

    #endregion
}
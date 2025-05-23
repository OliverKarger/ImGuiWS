using ImGuiNET;
using ImGuiWS.Components.Controls;
using ImGuiWS.Utils;

namespace ImGuiWS.Components.Modals;

/// <summary>
///     Popup Modal Control
/// </summary>
public class Modal : Control {
    public Modal(String title) : base(title.ToControlId()) {
        this.Label = title;
        this.Visible = false;
    }

    public Modal(String title, Window? parentWindow = null) : base(title.ToControlId(), parentWindow) {
        this.Label = title;
        this.Visible = false;
    }

    public override void Startup() {
        this.Controls.Startup();
    }

    public override void Update(Single delta) {
        if(!this.Visible) {
            return;
        }

        ImGui.OpenPopup(this.Label);
        if(ImGui.BeginPopupModal(this.Label)) {
            this.Controls.Update(delta);
            ImGui.EndPopup();
        }
    }

    public override void Shutdown() {
        this.Controls.Shutdown();
    }

    #region Properties

    /// <summary>
    ///     Nested Controls
    /// </summary>
    public WindowComponentCollection<Control> Controls { get; set; }

    /// <summary>
    ///     Label of the Control
    /// </summary>
    public String Label { get; set; }

    #endregion
}
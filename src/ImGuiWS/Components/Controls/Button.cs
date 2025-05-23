using ImGuiNET;
using ImGuiWS.Utils;

namespace ImGuiWS.Components.Controls;

public class Button : Control {
    public Button(String label) : base(label.ToControlId()) {
        this.Label = label;
    }

    public Button(String label, Window window) : base(label.ToControlId(), window) {
        this.Label = label;
    }

    #region Properties

    /// <summary>
    ///     Label of the Button
    /// </summary>
    public String Label { get; set; }

    #endregion

    #region Events

    /// <summary>
    ///     Invoked when Button is clicked
    /// </summary>
    public event Action OnClick;

    #endregion

    public override void Update(Single delta) {
        if(ImGui.Button(this.Label, this.Size)) {
            this.OnClick?.Invoke();
        }
    }
}
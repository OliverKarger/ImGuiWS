using ImGuiNET;
using ImGuiWS.Components.Controls;
using ImGuiWS.Logging;

namespace ImGuiWS.Components.BuiltIn;

public class SerilogWindow() : Window("Serilog Logs") {
    private ListBox _lbCompRef;

    public override void Startup() {
        this._lbCompRef = this.Controls.Add<ListBox>(() => new ListBox("Logs"))!;
        base.Startup();
    }

    public override void Update(Single delta) {
        this._lbCompRef.Items = LogBuffer.RenderedBuffer.ToList();
        this._lbCompRef.CurrentIndex = this._lbCompRef.Items.Count - 1;
        this._lbCompRef.Size = ImGui.GetWindowSize();
        base.Update(delta);
    }
}
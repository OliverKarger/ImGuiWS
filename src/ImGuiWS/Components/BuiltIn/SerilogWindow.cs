using ImGuiNET;
using ImGuiWS.Components.Controls;
using ImGuiWS.Logging;

namespace ImGuiWS.Components.BuiltIn;

public class SerilogWindow() : Window("Serilog Logs")
{
    private ListBox _lbCompRef;
    public override void Startup()
    {
        _lbCompRef = Controls.Add<ListBox>(() => new ListBox("Logs"))!;
        base.Startup();
    }
    
    public override void Update(float delta)
    {
        _lbCompRef.Items = LogBuffer.RenderedBuffer.ToList(); 
        _lbCompRef.CurrentIndex = _lbCompRef.Items.Count - 1;
        _lbCompRef.Size = ImGui.GetWindowSize();
        base.Update(delta);
    }
}
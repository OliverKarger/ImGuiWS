using ImGuiNET;
using ImGuiWS.Components.Controls;
using ImGuiWS.Utils;

namespace ImGuiWS.Components.Modals;

public class Modal : Control
{
    public WindowComponentCollection<Control> Controls { get; set; }
    
    public string Label { get; set; }
    
    public Modal(string title) : base(title.ToControlId())
    {
        Label = title;
        Visible = false;
    }

    public Modal(string title, Window? parentWindow = null) : base(title.ToControlId(), parentWindow)
    {
        Label = title;
        Visible = false;
    }

    public override void Startup()
    {
        Controls.Startup();
    }
    
    public override void Update(float delta)
    {
        if (!Visible) return;
        
        ImGui.OpenPopup(Label);
        if (ImGui.BeginPopupModal(Label))
        {
            Controls.Update(delta); 
            ImGui.EndPopup();
        }
    }

    public override void Shutdown()
    {
        Controls.Shutdown();
    }
}
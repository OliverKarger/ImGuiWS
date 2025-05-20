using ImGuiNET;
using ImGuiWS.Components.Controls;
using ImGuiWS.Utils;

namespace ImGuiWS.Components.Navigation;

public class Menu : Control
{
    public WindowComponentCollection<MenuItem> Items { get; protected internal set; }
    public WindowComponentCollection<Menu> SubMenues { get; protected internal set; }
   
    public string Label { get; set; }
    public bool Enabled { get; set; } = true;
    
    public Menu(string label) : base(label.ToControlId())
    {
        Label = label;
    }

    public Menu(string label, Window? parentWindow = null) : base(label, parentWindow)
    {
        Label = label;
    }

    public override void Startup()
    {
        Items.Startup();
        SubMenues.Startup();
    }

    public override void Update(float delta)
    {
        if (ImGui.BeginMenu(Label, Enabled))
        {
            Items.Update(delta);
            SubMenues.Update(delta);
            ImGui.EndMenu();
        }
    }
    
    public override void Shutdown()
    {
        Items.Shutdown();
        SubMenues.Shutdown();
    }
}
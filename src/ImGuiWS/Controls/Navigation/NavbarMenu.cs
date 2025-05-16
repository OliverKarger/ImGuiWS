using ImGuiNET;
using ImGuiWS.Controls.Utils;
using ImGuiWS.Renderer;

namespace ImGuiWS.Controls.Navigation;

public class NavbarMenu : ControlBase
{
    public NavbarItemCollection Items { get; internal set; }
    public NavbarMenuCollection Menus { get; internal set; }
    public NavbarMenu? ParentMenu { get; internal set; }
    public Navbar Navbar { get; internal set; }
    
    public string Label { get; set; }
    public bool Enabled { get; set; } = true;

    public NavbarMenu(string label) : base(label.ToControlId())
    {
        Label = label;
    }

    public override void Start()
    {
        Items.Start();
        Menus.Start();
    }

    public override void Update()
    {
        if (!Visible) return;
        if (ImGui.BeginMenu(Label, Enabled))
        {
            Items.Update();
            Menus.Update();

            ImGui.EndMenu();
        }
    }

    public override void Shutdown()
    {
        Items.Shutdown();
        Menus.Shutdown();
    }
}
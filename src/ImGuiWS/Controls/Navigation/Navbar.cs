using ImGuiNET;
using ImGuiWS.Renderer;

namespace ImGuiWS.Controls.Navigation;

public class Navbar : ControlBase
{
    public NavbarItemCollection Items { get; internal set; }
    public NavbarMenuCollection Menus { get; internal set; }

    public Navbar(string id) : base(id)
    {
        Items = new(RootWindow, this, null, DirectParent);
        Menus = new(RootWindow, this, null, DirectParent);
    }

    public override void Start()
    {
        Menus.Start();
        Items.Start();
    }

    public override void Update()
    {
        if (!Visible) return;
        if (ImGui.BeginMainMenuBar())
        {
            Menus.Update();
            Items.Update();
            ImGui.EndMainMenuBar();
        }
    }

    public override void Shutdown()
    {
        Menus.Shutdown();
        Items.Shutdown();
    }
}
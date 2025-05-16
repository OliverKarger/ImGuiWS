using ImGuiNET;
using ImGuiWS.Controls.Utils;
using ImGuiWS.Renderer;

namespace ImGuiWS.Controls.Navigation;

public class NavbarItem : ClickableControl
{
    public NavbarMenu? ParentMenu { get; internal set; }
    public Navbar Navbar { get; internal set; }
    
    public string Label { get; set; }
    public string? Shortcut { get; set; }
    public bool Selected { get; set; } = false;
    public bool Enabled { get; set; } = true;

    public NavbarItem(string label, string? shortcut = null) : base(label.ToControlId())
    {
        Label = label;
        Shortcut = shortcut;

        OnClick += () => { Selected = !Selected; };
    }
    
    public override void Start()
    {
    }

    public override void Update()
    {
        if (ImGui.MenuItem(Label, Shortcut, Selected, Enabled))
        {
            Click(); 
        }
    }

    public override void Shutdown()
    {
    }
}
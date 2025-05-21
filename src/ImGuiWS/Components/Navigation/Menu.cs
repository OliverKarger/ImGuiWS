using ImGuiNET;
using ImGuiWS.Components.Controls;
using ImGuiWS.Utils;

namespace ImGuiWS.Components.Navigation;

/// <summary>
///     Menu as Part of <see cref="MenuBar"/> or <see cref="MainMenuBar"/>
/// </summary>
public class Menu : Control
{
    #region Properties
    /// <summary>
    ///     Menu Items
    /// </summary>
    public WindowComponentCollection<MenuItem> Items { get; protected internal set; }
    
    /// <summary>
    ///     Menu Sub Menues
    /// </summary>
    public WindowComponentCollection<Menu> SubMenues { get; protected internal set; }
   
    /// <summary>
    ///     Menu Labels
    /// </summary>
    public string Label { get; set; }
    
    /// <summary>
    ///     Menu Enabled Flag
    /// </summary>
    public bool Enabled { get; set; } = true;
    #endregion 
    
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
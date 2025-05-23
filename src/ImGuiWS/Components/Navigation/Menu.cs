using ImGuiNET;
using ImGuiWS.Components.Controls;
using ImGuiWS.Utils;

namespace ImGuiWS.Components.Navigation;

/// <summary>
///     Menu as Part of <see cref="MenuBar" /> or <see cref="MainMenuBar" />
/// </summary>
public class Menu : Control {
    public Menu(String label) : base(label.ToControlId()) {
        this.Label = label;
    }

    public Menu(String label, Window? parentWindow = null) : base(label, parentWindow) {
        this.Label = label;
    }

    public override void Startup() {
        this.Items.Startup();
        this.SubMenues.Startup();
    }

    public override void Update(Single delta) {
        if(ImGui.BeginMenu(this.Label, this.Enabled)) {
            this.Items.Update(delta);
            this.SubMenues.Update(delta);
            ImGui.EndMenu();
        }
    }

    public override void Shutdown() {
        this.Items.Shutdown();
        this.SubMenues.Shutdown();
    }

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
    public String Label { get; set; }

    /// <summary>
    ///     Menu Enabled Flag
    /// </summary>
    public Boolean Enabled { get; set; } = true;

    #endregion
}
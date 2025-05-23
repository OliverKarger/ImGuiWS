using ImGuiNET;

namespace ImGuiWS.Components.Navigation;

/// <summary>
///     Main Menu Bar
/// </summary>
/// <remarks>
///     Only to be used on <see cref="MainWindow" />
///     Use <see cref="MenuBar" /> for ordinary <see cref="Window" />
/// </remarks>
public class MainMenuBar : Menu {
    public MainMenuBar(String id) : base(id) { }

    public MainMenuBar(String id, Window? parentWindow = null) : base(id, parentWindow) { }

    public override void Update(Single delta) {
        if(ImGui.BeginMainMenuBar()) {
            this.Items.Update(delta);
            this.SubMenues.Update(delta);

            ImGui.EndMainMenuBar();
        }
    }
}
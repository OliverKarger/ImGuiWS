using ImGuiNET;

namespace ImGuiWS.Components.Navigation;

/// <summary>
///     Main Menu Bar
/// </summary>
/// <remarks>
///     Only to be used on <see cref="MainWindow"/>
///     Use <see cref="MenuBar"/> for ordinary <see cref="Window"/>
/// </remarks>
public class MainMenuBar : Menu
{
    public MainMenuBar(string id) : base(id)
    {
    }

    public MainMenuBar(string id, Window? parentWindow = null) : base(id, parentWindow)
    {
    }

    public override void Update(float delta)
    {
        if (ImGui.BeginMainMenuBar())
        {
            Items.Update(delta);
            SubMenues.Update(delta);
            
            ImGui.EndMainMenuBar();
        }
    }
}

// See https://aka.ms/new-console-template for more information

using System.Net.Mime;
using System.Numerics;
using ImGuiNET;
using ImGuiWS.Controls;
using ImGuiWS.Controls.Input;
using ImGuiWS.Controls.Navigation;
using Veldrid.StartupUtilities;
using WindowState = Veldrid.WindowState;


namespace ImGuiWS.Demo;

class MyMainWindow() : MainWindow(new WindowCreateInfo(50, 50, 1680, 1024, WindowState.Normal, "Sample Window"))
{
    protected override void UserStart()
    {
        Controls.Add<Navbar>(() => new Navbar("Main Navbar"), navbar =>
        {
            navbar.Items.Add(() => new NavbarItem("Item 1"), item => { });
            navbar.Menus.Add(() => new NavbarMenu("Application"), menu =>
            {
                menu.Items.Add(() => new NavbarItem("Exit", "Alt+F4"), item =>
                {
                    item.OnClick += () => Environment.Exit(0);
                });

                menu.Items.Add(() => new NavbarItem("Item 2"), item => { });
            });
        });
        
        Windows.Add<Window>(() => new Window("Window 1"), window =>
        {
            window.Options.Position = new Vector2(250, 100);
            window.Options.Size = new Vector2(175, 175);
            window.Options.Collapsed = true;
            window.Controls.Add<Button>(() => new Button("Test"), button =>
            {
                button.OnClick += () => Console.WriteLine("Button clicked!");
            });
        });

        Windows.Add(() => new Window("Window 2"), window =>
        {
            window.Options.Position = new Vector2(500, 100);
            window.Options.Size = new Vector2(175, 175);
            window.Controls.Add<Checkbox>(() => new Checkbox("Test"), checkbox=>
            {
                checkbox.OnValueChanged += value => Console.WriteLine($"Checkbox Value: {value}");
            });
        });
    }

    protected override void UserUpdate()
    {
        ImGui.ShowDemoWindow();
    }
}

public static class Program
{
    public static void Main(string[] args)
    {
        string imguiIniPath = Path.Join(Environment.CurrentDirectory, "imgui.ini");
        if (File.Exists(imguiIniPath))
        {
            File.Delete(imguiIniPath);
        }
        var window = new MyMainWindow();
        window.RenderLoop();
    }
}
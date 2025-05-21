using System.Numerics;
using System.Runtime.CompilerServices;
using ImGuiNET;
using ImGuiWS.Components;
using ImGuiWS.Components.BuiltIn;
using ImGuiWS.Components.Controls;
using ImGuiWS.Components.Modals;
using ImGuiWS.Components.Navigation;
using ImGuiWS.Renderer;
using ImGuiWS.Utils;
using Veldrid;
using Veldrid.StartupUtilities;
using Vortice.Direct3D11;
using WindowState = Veldrid.WindowState;

namespace ImGuiWS.Demo;

public static class Program
{
    public static void Main(string[] args)
    {
        MainWindow window =
            new MainWindow(new WindowSetupOptions("Example Window", new Vector2(1680, 1240), new Vector2(100, 100)));

        window.SubWindows.Add(() => new Window("Draw List Example"), window =>
        {
            window.Size = new Vector2(512, 512);
            window.Position = new Vector2(50, 50);
            window.FixedSize = true;
            window.FixedPosition = true;
            window.Controls.Add(() => new DelegateControl("Draw List 1"), control =>
            {
                control.Visible = true;
                control.Delegate += () =>
                {
                    var drawList = ImGui.GetWindowDrawList();

                    Vector2 rectOrigin = window.ContentOrigin + new Vector2(100, 100); 
                    Vector2 rectTarget = window.ContentOrigin + new Vector2(200, 200); 
                
                    drawList.AddRectFilled(rectOrigin, rectTarget, 0xFFFFFFFF);
                    drawList.AddText(rectOrigin, 0xBBBBBBBB, "Hello!");
                };
            });
        });
        
        window.Controls.Add(() => new DelegateControl("ImGui Demo Window"), control =>
        {
            control.Visible = false;
            control.Delegate = () =>
            {
                bool visible = control.Visible;
                ImGui.ShowDemoWindow(ref visible);
                control.Visible = visible;
            };
        });

        window.Controls.Add(() => new Modal("Hello World"), modal =>
        {
            modal.Controls.Add(() => new Button("Test"), button =>
            {
                button.OnClick += () => modal.Visible = false;
            });
        });
        
        window.Controls.Add(() => new MainMenuBar("main_mmb"), menuBar =>
        {
            menuBar.SubMenues.Add(() => new Menu("Application"), menu =>
            {
                menu.Items.Add(() => new MenuItem("Exit"), item =>
                {
                    item.OnClick += () => Environment.Exit(0);
                });
            });

            menuBar.SubMenues.Add(() => new Menu("Developer"), menu =>
            {
                menu.SubMenues.Add(() => new Menu("ImGUI"), imguiMenu =>
                {
                    imguiMenu.Items.Add(() => new MenuItem("Demo Window"), item =>
                    {
                        item.OnClick += () =>
                            item.MainWindow.FindComponent<DelegateControl>(e => e.Id == "imgui_demo_window")
                                .ToggleVisibility();
                    });
                });

                menu.Items.Add(() => new MenuItem("Test Modal"), menuItem =>
                {
                    menuItem.OnClick += () =>
                    {
                        menuItem.MainWindow.FindComponent<Modal>(m => m.Id == "hello_world").Visible = true;
                    };
                });
            });
        });
        
        window.Startup();
        while (window.WindowExists)
        {
            window.Update(.0f);
        }
        window.Shutdown();
    }
}
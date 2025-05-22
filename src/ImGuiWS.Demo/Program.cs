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
            window.Position = new Vector2(50, 50);
            window.FixedSize = true;
            window.FixedPosition = false;
            window.Controls.Add(() => new DelegateControl("Draw List 1"), control =>
            {
                control.Visible = true;

                Vector2 boxSize = new Vector2(64, 64);
                const int numBoxes = 32;
                const int numBoxesPerRow = 8;
                Vector2 offset = new Vector2(5, 5);

                const uint textColor = 0xFF000000;
                const uint boxColor = 0xFFAAAAAA;
                const uint selectedBoxColor = 0xFF000000;
                const uint selectedTextColor = 0xFFAAAAAA;

                int selectedBoxIndex = 0;
                
                int numRows = (int)Math.Ceiling(numBoxes / (float)numBoxesPerRow);

                float requiredWidth = offset.X + numBoxesPerRow * boxSize.X + (numBoxesPerRow - 1) * offset.X;
                float requiredHeight = offset.Y + numRows * boxSize.Y + (numRows - 1) * offset.Y;

                window.Size = new Vector2(requiredWidth, requiredHeight);
                
                control.Delegate += () =>
                {
                    ImDrawListPtr drawList = ImGui.GetWindowDrawList();
                    Vector2 mousePos = ImGui.GetMousePos();
                    bool mouseClicked = ImGui.IsMouseClicked(ImGuiMouseButton.Left);

                    for (int box = 0; box < numBoxes; box++)
                    {
                        int row = box / numBoxesPerRow;
                        int col = box % numBoxesPerRow;

                        float x = window.ContentOrigin.X + offset.X + col * (boxSize.X + offset.X);
                        float y = window.ContentOrigin.Y + offset.Y + row * (boxSize.Y + offset.Y);

                        Vector2 topLeft = new Vector2(x, y);
                        Vector2 bottomRight = topLeft + boxSize;

                        bool isHovered =
                            mousePos.X >= topLeft.X && mousePos.X <= bottomRight.X &&
                            mousePos.Y >= topLeft.Y && mousePos.Y <= bottomRight.Y;

                        if (isHovered && mouseClicked)
                        {
                            selectedBoxIndex = box;
                        }

                        uint actualBoxColor = (box == selectedBoxIndex) ? selectedBoxColor : boxColor;
                        uint actualTextColor = (box == selectedBoxIndex) ? selectedTextColor : textColor;

                        drawList.AddRectFilled(topLeft, bottomRight, actualBoxColor);
                        drawList.AddText(topLeft + offset, actualTextColor, $"Box {box}");
                    }
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
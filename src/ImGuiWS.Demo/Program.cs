using System.Numerics;
using Emgu.CV;
using ImGuiNET;
using ImGuiWS.Components;
using ImGuiWS.Components.Controls;
using ImGuiWS.Components.Modals;
using ImGuiWS.Components.Navigation;
using ImGuiWS.Integrations.EmguCV;
using ImGuiWS.Renderer;
using Texture = ImGuiWS.Renderer.Texture;

namespace ImGuiWS.Demo;

public static class Program {
    public static void Main(String[] args) {
        MainWindow window =
            new(new WindowSetupOptions("Example Window", new Vector2(1680, 1240), new Vector2(100, 100)));

        window.SubWindows.Add(() => new Window("Image Example"), window => {
            TextureManager textureManager = window.MainWindow.Backend.Context.Textures;
            Texture tex = CvInvoke.Imread("Example.png").AsTexture(textureManager);

            window.Controls.Add(() => new Button("Update Image"), button => {
                button.OnClick += () => {
                    if(tex.Id != IntPtr.Zero) {
                        CvInvoke.Imread("Example1.png").AsTextureUpdate((IntPtr)tex.Id, textureManager);
                    }
                };
            });

            window.Controls.Add(() => new DelegateControl("Preview Window"), control => {
                control.Delegate += () => {
                    if(tex.Id != IntPtr.Zero) {
                        ImGui.Image((IntPtr)tex.Id, tex.Size);
                    }
                };
            });
        });

        window.SubWindows.Add(() => new Window("Draw List Example"), window => {
            window.Position = new Vector2(50, 50);
            window.FixedSize = true;
            window.FixedPosition = false;
            window.Controls.Add(() => new DelegateControl("Draw List 1"), control => {
                control.Visible = true;

                Vector2 boxSize = new(64, 64);
                const Int32 numBoxes = 32;
                const Int32 numBoxesPerRow = 8;
                Vector2 offset = new(5, 5);

                const UInt32 textColor = 0xFF000000;
                const UInt32 boxColor = 0xFFAAAAAA;
                const UInt32 selectedBoxColor = 0xFF000000;
                const UInt32 selectedTextColor = 0xFFAAAAAA;

                Int32 selectedBoxIndex = 0;

                Int32 numRows = (Int32)Math.Ceiling(numBoxes / (Single)numBoxesPerRow);

                Single requiredWidth = offset.X + numBoxesPerRow * boxSize.X + (numBoxesPerRow - 1) * offset.X;
                Single requiredHeight = offset.Y + numRows * boxSize.Y + (numRows - 1) * offset.Y;

                window.Size = new Vector2(requiredWidth, requiredHeight);

                control.Delegate += () => {
                    ImDrawListPtr drawList = ImGui.GetWindowDrawList();
                    Vector2 mousePos = ImGui.GetMousePos();
                    Boolean mouseClicked = ImGui.IsMouseClicked(ImGuiMouseButton.Left);

                    for(Int32 box = 0; box < numBoxes; box++) {
                        Int32 row = box / numBoxesPerRow;
                        Int32 col = box % numBoxesPerRow;

                        Single x = window.ContentOrigin.X + offset.X + col * (boxSize.X + offset.X);
                        Single y = window.ContentOrigin.Y + offset.Y + row * (boxSize.Y + offset.Y);

                        Vector2 topLeft = new(x, y);
                        Vector2 bottomRight = topLeft + boxSize;

                        Boolean isHovered =
                            mousePos.X >= topLeft.X && mousePos.X <= bottomRight.X &&
                            mousePos.Y >= topLeft.Y && mousePos.Y <= bottomRight.Y;

                        if(isHovered && mouseClicked) {
                            selectedBoxIndex = box;
                        }

                        UInt32 actualBoxColor = box == selectedBoxIndex ? selectedBoxColor : boxColor;
                        UInt32 actualTextColor = box == selectedBoxIndex ? selectedTextColor : textColor;

                        drawList.AddRectFilled(topLeft, bottomRight, actualBoxColor);
                        drawList.AddText(topLeft + offset, actualTextColor, $"Box {box}");
                    }
                };
            });
        });

        window.Controls.Add(() => new DelegateControl("ImGui Demo Window"), control => {
            control.Visible = false;
            control.Delegate = () => {
                Boolean visible = control.Visible;
                ImGui.ShowDemoWindow(ref visible);
                control.Visible = visible;
            };
        });

        window.Controls.Add(() => new Modal("Hello World"),
            modal => {
                modal.Controls.Add(() => new Button("Test"),
                    button => { button.OnClick += () => modal.Visible = false; });
            });

        window.Controls.Add(() => new MainMenuBar("main_mmb"), menuBar => {
            menuBar.SubMenues.Add(() => new Menu("Application"),
                menu => {
                    menu.Items.Add(() => new MenuItem("Exit"), item => { item.OnClick += () => Environment.Exit(0); });
                });

            menuBar.SubMenues.Add(() => new Menu("Developer"), menu => {
                menu.SubMenues.Add(() => new Menu("ImGUI"), imguiMenu => {
                    imguiMenu.Items.Add(() => new MenuItem("Demo Window"), item => {
                        item.OnClick += () =>
                            item.MainWindow.FindComponent<DelegateControl>(e => e.Id == "imgui_demo_window")
                                .ToggleVisibility();
                    });
                });

                menu.Items.Add(() => new MenuItem("Test Modal"),
                    menuItem => {
                        menuItem.OnClick += () => {
                            menuItem.MainWindow.FindComponent<Modal>(m => m.Id == "hello_world").Visible = true;
                        };
                    });
            });
        });

        window.Startup();

        while(window.WindowExists) {
            window.Update(.0f);
        }

        window.Shutdown();
    }
}
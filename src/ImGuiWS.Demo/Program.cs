// See https://aka.ms/new-console-template for more information

using System.Numerics;
using ImGuiNET;
using ImGuiWS.Controls;
using ImGuiWS.Controls.Input;
using Veldrid.StartupUtilities;
using WindowState = Veldrid.WindowState;


namespace ImGuiWS.Demo;

class MyMainWindow() : MainWindow(new WindowCreateInfo(50, 50, 1680, 1024, WindowState.Normal, "Sample Window"))
{
    protected override void UserStart()
    {
        Utils.LoadFontFromFile("C:\\Fonts\\Aptos.ttf",20);

        Windows.Add<Window>(() => new Window("Window 1"), window =>
        {
            window.Position = new Vector2(250, 100);
            window.Size = new Vector2(175, 175);
            window.Controls.Add<Button>(() => new Button("Test"), button =>
            {
                button.OnClick += () => Console.WriteLine("Button clicked!");
            });
        });

        Windows.Add(() => new Window("Window 2"), window =>
        {
            window.Position = new Vector2(500, 100);
            window.Size = new Vector2(175, 175);
            window.Controls.Add<Checkbox>(() => new Checkbox("Test"), checkbox=>
            {
                checkbox.OnValueChanged += value => Console.WriteLine($"Checkbox Value: {value}");
            });
        });

        Windows.Add(() => new Window("Window 3"), window =>
        {
            window.Position = new Vector2(750, 100);
            window.Size = new Vector2(175, 175);
            window.FixedSize = false;
            window.FixedPosition = false;
            window.Controls.Add<Image>(() => new Image("image_1"), image =>
            {
                image.ImagePath = Path.Join(Environment.CurrentDirectory, "azure.png");
                image.ScaleFactor = 0.1f;
            });
        });
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
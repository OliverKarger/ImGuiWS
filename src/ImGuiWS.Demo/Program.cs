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
            window.Controls.Add<Button>(() => new Button("Test"), button =>
            {
                button.OnClick += () => Console.WriteLine("Button clicked!");
            });
        });

        Windows.Add(() => new Window("Window 2"), window =>
        {
            window.Controls.Add<Checkbox>(() => new Checkbox("Test"), checkbox=>
            {
                checkbox.OnValueChanged += value => Console.WriteLine($"Checkbox Value: {value}");
            });
        });

        Windows.Add(() => new Window("Window 3"), window =>
        {
            window.Controls.Add<Image>(() => new Image("image_1"), image =>
            {
                image.ImagePath = Path.Join(Environment.CurrentDirectory, "azure.png");
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
        var window = new MyMainWindow();
        window.RenderLoop();
    }
}
// See https://aka.ms/new-console-template for more information

using System.Numerics;
using ImGuiNET;
using ImGuiWS.Controls;
using Veldrid.StartupUtilities;
using WindowState = Veldrid.WindowState;


namespace ImGuiWS.Demo;

class MyWindow() : ImGuiWS.Window(new WindowCreateInfo(50, 50, 1680, 1024, WindowState.Normal, "Sample Window"))
{
    protected override void Start()
    {
        Controls.AddControl<Button>(() => new Button("Hello!"), button =>
        {
            button.OnClick += () => { Console.WriteLine("Button 1 clicked!"); };
            button.Size = new Vector2(128, 512);
        });
    }
    
    protected override void Update()
    {
        ImGui.ShowDemoWindow();
    }
}

public static class Program
{
    public static void Main(string[] args)
    {
        var window = new MyWindow();
        window.RunLoop();
    }
}
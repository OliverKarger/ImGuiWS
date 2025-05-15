// See https://aka.ms/new-console-template for more information

using System.Numerics;
using ImGuiNET;
using ImGuiWS.Controls;
using Veldrid.StartupUtilities;
using WindowState = Veldrid.WindowState;


namespace ImGuiWS.Demo;

class MyMainWindow() : ImGuiWS.MainWindow(new WindowCreateInfo(50, 50, 1680, 1024, WindowState.Normal, "Sample Window"))
{
    protected override void Start()
    {
        Windows.Add("Test Window", window =>
        {
            window.Controls.Add<Button>(() => new Button("Test"), button => { });
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
        var window = new MyMainWindow();
        window.Render();
    }
}
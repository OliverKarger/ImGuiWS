// See https://aka.ms/new-console-template for more information

using ImGuiNET;
using Veldrid.StartupUtilities;
using WindowState = Veldrid.WindowState;


namespace ImGuiWS.Demo;

class MyWindow() : ImGuiWS.Window(new WindowCreateInfo(50, 50, 512, 512, WindowState.Normal, "Sample Window"))
{
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
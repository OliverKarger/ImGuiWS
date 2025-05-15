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
        Controls.Add<Button>(() => new Button("Hello!"), control =>
        {
            control.OnClick += () => { Console.WriteLine("Button 1 clicked!"); };
            control.Size = new Vector2(128, 512);
            control.OnClick += () =>
            {
                var inputControl = Controls.GetByName<InputBox<string>>("String Input");
                inputControl.Value = string.Empty;
            };
        });

        Controls.Add<InputBox<string>>(() => new InputBox<string>("String Input"), control =>
        {
            control.MaxLength = 32;
            control.Value = "Currently no Value!";
        });

        Controls.Add<InputBox<int>>(() => new InputBox<int>("Int Input"), control =>
        {

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
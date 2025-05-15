using ImGuiNET;
using ImGuiWS.Controls.Utils;

namespace ImGuiWS.Controls;

public enum WindowRenderMode
{
    ControlsFirst,
    SubWindowsFirst
}

public class Window
{
    public string Id { get; set; }
    public WindowControlsCollection Controls { get; internal set; }
    public WindowCollection Windows { get; internal set; }
    public Window? Parent { get; internal set; }
    public WindowRenderMode RenderMode { get; set; } = WindowRenderMode.ControlsFirst;
    public bool Open { get; set; } = true;
    public event Action OnOpened;
    public event Action OnClosed;
    public string Label { get; set; }

    public Window(string label)
    {
        Id = label.ToControlId();
        Label = label;
        Controls = new WindowControlsCollection(this);
        Windows = new WindowCollection(this);
    }

    protected internal virtual void Start(){}

    protected internal virtual void Update()
    {
        bool open = Open;
        bool beginSucceeded = ImGui.Begin(Label, ref open);

        if (open != Open)
        {
            Open = open;
            if (Open)
                OnOpened?.Invoke();
            else
                OnClosed?.Invoke();
        }

        if (beginSucceeded)
        {
            if (RenderMode == WindowRenderMode.ControlsFirst)
            {
                Controls.Render();
                Windows.Render();
            }

            if (RenderMode == WindowRenderMode.SubWindowsFirst)
            {
                Windows.Render();
                Controls.Render();
            }
        }

        ImGui.End(); // Always call End if Begin was called
    }
    
    protected internal virtual void Shutdown(){}
}
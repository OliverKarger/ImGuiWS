using System.Numerics;
using System.Reflection.Emit;
using ImGuiNET;
using ImGuiWS.Components.Style;
using ImGuiWS.Utils;

namespace ImGuiWS.Components.Controls;

public class Button : Control
{
    public event Action OnClick;
    public string Label { get; set; }

    public Button(string label) : base(label.ToControlId())
    {
        Label = label;
    }

    public Button(string label, Window window) : base(label.ToControlId(), window)
    {
        Label = label;
    }

    public override void Update(float delta)
    {
        if (ImGui.Button(Label, Size))
        {
            OnClick?.Invoke();
        }
    }
}
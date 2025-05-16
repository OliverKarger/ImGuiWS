using ImGuiNET;
using ImGuiWS.Controls.Utils;

namespace ImGuiWS.Controls.Input;

public class Checkbox : ValueControl<bool>
{
    public string Label { get; set; }
    
    public Checkbox(string label) : base(label.ToControlId())
    {
        Label = label;
    }

    public Checkbox(bool initialValue, string label) : base(initialValue, label.ToControlId())
    {
        Label = label;
    }

    public override void Start()
    {
    }

    public override void Update()
    {
        bool temp = Value;
        ImGui.Checkbox(Label.ToControlId(), ref temp);
        if (temp == Value) return;
        OnValueChanged?.Invoke(temp);
        Value = temp;
    }

    public override void Shutdown(){}
}
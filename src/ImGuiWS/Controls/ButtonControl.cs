namespace ImGuiWS.Controls;

public class ButtonControl(string label, string id) : ClickableControl(id)
{
    public string Label { get; set; } = label;

    internal override void Render()
    {
        
    }
    
}
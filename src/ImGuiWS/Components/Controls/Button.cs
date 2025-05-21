using System.Numerics;
using System.Reflection.Emit;
using ImGuiNET;
using ImGuiWS.Utils;

namespace ImGuiWS.Components.Controls;

public class Button : Control
{
    #region Events
    /// <summary>
    ///     Invoked when Button is clicked
    /// </summary>
    public event Action OnClick;

    #endregion
    
    #region Properties
    /// <summary>
    ///     Label of the Button 
    /// </summary>
    public string Label { get; set; }
    #endregion

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
using ImGuiNET;
using ImGuiWS.Utils;

namespace ImGuiWS.Components.Navigation;

/// <summary>
///     Menu Item
/// </summary>
public class MenuItem : RenderableComponent
{
    #region "Properties"
    /// <summary>
    ///     Menu Item Label
    /// </summary>
    public string Label { get; set; }
    
    /// <summary>
    ///     Menu Item Shortcut
    /// </summary>
    /// <example>
    ///     ALT+F4
    /// </example>
    public string Shortcut { get; set; } = string.Empty;
    
    /// <summary>
    ///     Selected / Active Flag
    /// </summary>
    public bool Selected { get; set; } = false;
    
    /// <summary>
    ///     Enabled Flag
    /// </summary>
    public bool Enabled { get; set; } = true;
    #endregion
    
    #region Events
    /// <summary>
    ///     Invoked when Menu Item is clicked
    /// </summary>
    public event Action? OnClick;
    
    /// <summary>
    ///     Invoked when Item is activated / selected
    /// </summary>
    public event Action? OnSelected;
    
    /// <summary>
    ///     Invoked when Item is deactivated / deselected
    /// </summary>
    public event Action? OnDeselected;
    #endregion
    
    public MenuItem(string label) : base(label.ToControlId())
    {
        Label = label;
    }

    public MenuItem(string label, Window? parentWindow = null) : base(label.ToControlId(), parentWindow)
    {
        Label = label;
    }

    public override void Update(float delta)
    {
        bool selected = Selected;
        
        if (ImGui.MenuItem(Label, Shortcut, ref selected, Enabled))
        {
            OnClick?.Invoke(); 
        }

        if (Selected == selected) return;
        
        Selected = selected;
        if (Selected)
        {
            OnSelected?.Invoke();
        }
        else
        {
            OnDeselected?.Invoke();
        }
    }
}
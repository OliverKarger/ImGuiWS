using System.Numerics;
using ImGuiNET;
using ImGuiWS.Utils;

namespace ImGuiWS.Components.Controls;

public class ListBox : Control
{
    #region Properties
    /// <summary>
    ///     Current Item List
    /// </summary>
    public List<string> Items { get; internal set; } = new();
    /// <summary>
    ///     Index of the current Item
    /// </summary>
    public int CurrentIndex { get; set; } = 0;
    /// <summary>
    ///     Label of the ListBox
    /// </summary>
    public string Label { get; set; }
    #endregion
    
    public ListBox(string label, Window window) : base(label.ToControlId(), window)
    {
        Label = label;
    }

    public ListBox(string label) : base(label.ToControlId())
    {
        Label = label;
    }

    public override void Update(float delta)
    {
        if (CurrentIndex >= Items.Count)
        {
            CurrentIndex = 0;
        }
        
        if (ImGui.BeginListBox(Label, Size))
        {
            for (int i = 0; i < Items.Count; i++)
            {
                bool isSelected = (i == CurrentIndex);
                if (ImGui.Selectable(Items[i], isSelected))
                {
                    CurrentIndex = i;
                }

                if (isSelected)
                {
                    ImGui.SetItemDefaultFocus();
                }
            }

            ImGui.EndListBox();
        }
    }
}
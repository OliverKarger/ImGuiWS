using ImGuiNET;
using ImGuiWS.Utils;

namespace ImGuiWS.Components.Controls;

public class ListBox : Control {
    public ListBox(String label, Window window) : base(label.ToControlId(), window) {
        this.Label = label;
    }

    public ListBox(String label) : base(label.ToControlId()) {
        this.Label = label;
    }

    public override void Update(Single delta) {
        if(this.CurrentIndex >= this.Items.Count) {
            this.CurrentIndex = 0;
        }

        if(ImGui.BeginListBox(this.Label, this.Size)) {
            for(Int32 i = 0; i < this.Items.Count; i++) {
                Boolean isSelected = i == this.CurrentIndex;
                if(ImGui.Selectable(this.Items[i], isSelected)) {
                    this.CurrentIndex = i;
                }

                if(isSelected) {
                    ImGui.SetItemDefaultFocus();
                }
            }

            ImGui.EndListBox();
        }
    }

    #region Properties

    /// <summary>
    ///     Current Item List
    /// </summary>
    public List<String> Items { get; internal set; } = new();

    /// <summary>
    ///     Index of the current Item
    /// </summary>
    public Int32 CurrentIndex { get; set; }

    /// <summary>
    ///     Label of the ListBox
    /// </summary>
    public String Label { get; set; }

    #endregion
}
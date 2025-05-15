using System.Numerics;
using System.Runtime.CompilerServices;
using ImGuiNET;
using ImGuiWS.Controls.Utils;

namespace ImGuiWS.Controls;

public class Button(string label) : ClickableControl(label.ToControlId())
{
   public string Label { get; set; } = label;
   public Vector2 Size { get; set; } = Vector2.Zero;
   
   public override void Render()
   {
      if (ImGui.Button(Label, Size))
      {
         Click();
      }
   }
}
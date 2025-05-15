using System.Numerics;
using System.Runtime.CompilerServices;
using ImGuiNET;

namespace ImGuiWS.Controls;

public class Button(string label) : ClickableControl(label.Trim().Replace(' ', '_').ToLower())
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
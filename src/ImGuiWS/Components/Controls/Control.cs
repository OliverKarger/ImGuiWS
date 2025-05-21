using System.Numerics;
using ImGuiNET;
using ImGuiWS.Renderer;

namespace ImGuiWS.Components.Controls;

/// <summary>
///     Base Class for all Controls
/// </summary>
public class Control : RenderableComponent
{
    public Control(string id, Window window) : base(id, window)
    { }

    public Control(string id) : base(id)
    { }
   
    #region Properties
    /// <summary>
    ///     Size of the Control
    /// </summary>
    public Vector2 Size { get; set; } = Vector2.Zero;
    
    /// <summary>
    ///     Position of the Control
    /// </summary>
    public Vector2 Position { get; set; } = Vector2.Zero;
    #endregion
}
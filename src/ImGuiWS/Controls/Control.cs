namespace ImGuiWS.Controls;

/// <summary>
///     Base Class for all UI Controls
/// </summary>
/// <param name="id">
///     Internal ImGui ID
/// </param>
public abstract class Control(string id)
{
    /// <summary>
    ///     Internal Id used by the ImGui Backend
    ///     for Resource Allocation and Management
    /// </summary>
    internal Int32 ResourceId { get; set; } = 0;
    
    /// <summary>
    ///     Unique Id for ImGui' Tag System
    /// </summary>
    public string Id { get; internal set; } = id;

    /// <summary>
    ///     Render Method
    /// </summary>
    internal virtual void Render()
    {
        
    }
}
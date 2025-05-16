namespace ImGuiWS.Renderer;

/// <summary>
///     Interface for all Renderable Objects
/// </summary>
public interface IRenderable
{
    /// <summary>
    ///     Called before the Render Loop
    /// </summary>
    public void Start();
    
    /// <summary>
    ///     Called on each Frame
    /// </summary>
    public void Update();
    
    /// <summary>
    ///     Called after the Render Loop
    /// </summary>
    public void Shutdown();
}
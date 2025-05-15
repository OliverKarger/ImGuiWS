namespace ImGuiWS.Renderer;

public sealed class ResourceConfiguration
{

    public string VertexBufferName { get; internal set; } = "VertexBuffer";
    public string IndexBufferName { get; internal set; } = "IndexBuffer";
    public string ProjectionBufferName { get; internal set; } = "ProjectionBuffer";

}
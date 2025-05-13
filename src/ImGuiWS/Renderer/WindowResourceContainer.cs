using Veldrid;

namespace ImGuiWS.Renderer;

public class WindowResourceContainer : IDisposable
{
    public GraphicsDevice GraphicsDevice { get; internal set; }
    
    // Buffers
    public DeviceBuffer VertexBuffer { get; internal set; }
    public DeviceBuffer IndexBuffer { get; internal set; }
    public DeviceBuffer ProjectionBuffer { get; internal set; }
    
    public TextureContainer FontTexture { get; internal set; }
    public ShaderSet Shaders { get; internal set; }
    
    public ResourceLayout ResourceLayout { get; internal set; }
    public ResourceLayout TextureLayout { get; internal set; }
    public Pipeline RenderPipeline { get; internal set; }
    public ResourceSet MainResourceSet { get; internal set; }
    public ResourceSet FontTextureResourceSet { get; internal set; }

    internal readonly Dictionary<TextureView, ResourceSetInfo> SetsByView;
    internal readonly Dictionary<Texture, TextureView> ViewsByTexture;
    internal readonly Dictionary<IntPtr, ResourceSetInfo> ViewsById;
    internal readonly List<IDisposable> OwnedResources;
    public int LastAssignedId { get; internal set; }

    public WindowResourceContainer()
    {

        SetsByView = new Dictionary<TextureView, ResourceSetInfo>();
        ViewsByTexture = new Dictionary<Texture, TextureView>();
        ViewsById = new Dictionary<IntPtr, ResourceSetInfo>();
        OwnedResources = new List<IDisposable>();
        LastAssignedId = 100;
    }

    public WindowResourceContainer(
        GraphicsDevice graphicsDevice, 
        DeviceBuffer vertexBuffer, 
        DeviceBuffer indexBuffer, 
        DeviceBuffer projectionBuffer, 
        TextureContainer fontTexture, 
        ShaderSet shaders, 
        ResourceLayout resourceLayout, 
        ResourceLayout textureLayout, 
        Pipeline renderPipeline, 
        ResourceSet mainResourceSet, 
        ResourceSet fontTextureResourceSets) : this()
    {
        GraphicsDevice = graphicsDevice;
        VertexBuffer = vertexBuffer;
        IndexBuffer = indexBuffer;
        ProjectionBuffer = projectionBuffer;
        FontTexture = fontTexture;
        Shaders = shaders;
        ResourceLayout = resourceLayout;
        TextureLayout = textureLayout;
        RenderPipeline = renderPipeline;
        MainResourceSet = mainResourceSet;
        FontTextureResourceSet = fontTextureResourceSets;
    }

    internal IntPtr FontAtlasId = (IntPtr)1;
    
    public void Dispose()
    {
        // TODO release managed resources here
    }
}
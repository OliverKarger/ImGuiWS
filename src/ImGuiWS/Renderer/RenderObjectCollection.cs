using ImGuiWS.Controls;

namespace ImGuiWS.Renderer;

public abstract class RenderObjectCollection<TBase>(MainWindow rootWindow, Window? directParent) : IDisposable, IRenderable
{
    protected readonly HashSet<TBase> _objects = new();
    protected internal readonly Window? DirectParent = directParent;
    protected internal readonly MainWindow RootWindow = rootWindow;

    public void Dispose()
    {
        _objects.Clear();
    }
    
    public abstract RenderObjectCollection<TBase> Add<TDerived>(TDerived obj) where TDerived : TBase;
    public abstract RenderObjectCollection<TBase> Add<TDerived>(Func<TDerived> factory, Action<TDerived> configure) where TDerived: TBase;
    
    public abstract void Start();
    public abstract void Update();
    public abstract void Shutdown();
}
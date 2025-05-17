using ImGuiWS.Controls;

namespace ImGuiWS.Renderer;

public abstract class RenderObjectCollection<TBase>(MainWindow rootWindow, Window? directParent) : IDisposable, IRenderable where TBase : IRenderable
{
    protected readonly HashSet<TBase> _objects = new();
    protected internal readonly Window? DirectParent = directParent;
    protected internal readonly MainWindow RootWindow = rootWindow;

    public int Count
    {
        get => _objects.Count;
    }

    public void Dispose()
    {
        _objects.Clear();
    }
    
    public abstract RenderObjectCollection<TBase> Add<TDerived>(Func<TDerived> factory, Action<TDerived>? configure) where TDerived: TBase;

    public TDerived? Get<TDerived>(string id) where TDerived : ControlBase, TBase
    {
        TDerived? found = null;
        foreach (var obj in _objects)
        {
            if (typeof(ControlBase).IsAssignableFrom(obj.GetType()))
            {
                ControlBase control = (obj as ControlBase)!;
                if (control.Id == id)
                {
                    found = (TDerived?)control;
                    break;
                }
            }
        }

        if (found == null)
        {
            throw new NullReferenceException($"Control {id} not found");
        }
        
        return found;
    }

    public virtual void Start()
    {
        foreach (var obj in _objects) obj.Start();
    }

    public virtual void Update()
    {
        foreach (var obj in _objects) obj.Update();
    }

    public virtual void Shutdown()
    {
        foreach (var obj in _objects) obj.Shutdown();
    }
}
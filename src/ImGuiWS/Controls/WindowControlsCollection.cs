using System.Data;
using ImGuiNET;
using ImGuiWS.Controls;
using ImGuiWS.Controls.Utils;
using ImGuiWS.Logging;
using ImGuiWS.Renderer;
using Serilog;

namespace ImGuiWS;

public class WindowControlsCollection(MainWindow rootWindow, Window? directParent) : RenderObjectCollection<ControlBase>(rootWindow, directParent)
{
    private readonly ILogger _logger = LoggerFactory.Create<WindowControlsCollection>();

    public override WindowControlsCollection Add<T>(Func<T> factory, Action<T>? configure)
    {
        T? control = factory();
        
        control.RootWindow = RootWindow;
        control.DirectParent = DirectParent;
        
        configure?.Invoke(control);
        
        if (_objects.Any(e => e.Id == control.Id))
        {
            throw new DuplicateNameException("Duplicate control name/id"); 
        }
        
        _logger.Information("Added Control {id} to Window {window}", control.Id, DirectParent?.Id ?? RootWindow.Id);
        return this;
    }

    public T GetById<T>(string id) where T : ControlBase
    {
        var found = _objects.FirstOrDefault(e => e.Id == id);

        if (found == null)
        {
            throw new KeyNotFoundException($"Control with id {id} not found");
        }
        
        if (typeof(T) != found.GetType())
        {
            throw new Exception($"Type {found.GetType().Name} of Control {id} does not match with {typeof(T).Name}");
        }

        return (found as T)!;
    }

    public T GetByName<T>(string name) where T : ControlBase
    {
        return GetById<T>(name.ToControlId());
    }
    public override void Start()
    {
        foreach (var control in _objects)
        {
            control.Start();
            _logger.Information("Initialized Control {id} of Window {window}", control.Id, DirectParent?.Id ?? RootWindow.Id);
        }
    }

    public override void Update()
    {
        foreach (var control in _objects)
        {
            if (control.Visible)
            {
                ImGui.PushID(control.Id);
                control.Update();
                ImGui.PopID();
            }
        }
    }

    public override void Shutdown()
    {
        foreach (var control in _objects)
        {
            control.Shutdown();
            _logger.Information("Destroyed Control {id} of Window {window}", control.Id, DirectParent?.Id ?? RootWindow.Id);
        }
    }
}
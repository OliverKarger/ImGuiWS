using System.Data;
using ImGuiNET;
using ImGuiWS.Logging;
using ImGuiWS.Renderer;
using Serilog;

namespace ImGuiWS.Controls;

/// <summary>
///     Contains SubWindows to a Window
/// </summary>
/// <param name="parent">
///     Parent Window
/// </param>
public class WindowCollection(MainWindow rootWindow, Window? parent) : RenderObjectCollection<Window>(rootWindow, parent)
{
    private readonly ILogger _logger = LoggerFactory.Create<WindowCollection>(parent?.Options.Label);

    /// <summary>
    ///     Adds a new Window
    /// </summary>
    public override WindowCollection Add<TWindow>(Func<TWindow> factory, Action<TWindow>? configure)
    {
        TWindow window = factory();
        
        window.RootWindow = RootWindow;
        window.DirectParent = DirectParent;
        window.Controls = new WindowControlsCollection(RootWindow, window);
        window.Windows = new WindowCollection(RootWindow, window);
        
        configure?.Invoke(window);
        
        if (_objects.Any(e => e.Id == window.Id))
        {
            throw new DuplicateNameException("Duplicate SubWindow name/id");
        }
        
        _objects.Add(window);
        _logger.Information("Added Window {windowName}", window.Options.Label);
        return this;
    }

    public override void Start()
    {
        foreach (var obj in _objects)
        {
            obj.Start();
            _logger.Information("Initialized Window {name}",obj.Options.Label);
        }
    }

    public override void Update()
    {
        foreach (var obj in _objects)
        {
            obj.Update();
        }
    }

    public override void Shutdown()
    {
        foreach (var obj in _objects)
        {
            obj.Shutdown();
            _logger.Information("Shutdown Window {name}", obj.Options.Label);
        }
    }
}
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
    private readonly ILogger _logger = LoggerFactory.Create<WindowCollection>();

    /// <summary>
    ///     Adds a new Window
    /// </summary>
    /// <param name="window">
    ///     Window Object. Id must be unique!
    /// </param>
    /// <returns></returns>
    /// <exception cref="DuplicateNameException">
    ///     Thrown when a Window with the same Id already exists
    /// </exception>
    public override WindowCollection Add<Window>(Window obj)
    {
        if (_objects.Any(e => e.Id == obj.Id))
        {
            throw new DuplicateNameException("Duplicate SubWindow name/id");
        }

        obj.RootWindow = RootWindow;
        obj.DirectParent = DirectParent;
        _objects.Add(obj);
        _logger.Information("Added Window {windowName}", obj.Label);
        return this;
    }

    /// <summary>
    ///     Adds a new Window
    /// </summary>
    public override WindowCollection Add<Window>(Func<Window> factory, Action<Window> configure)
    {
        Window window = factory();
        configure(window);
        return Add(window);
    }

    public override void Start()
    {
        foreach (var obj in _objects)
        {
            obj.Start();
            _logger.Information("Initialized Window {name}",obj.Label);
        }
    }

    public override void Update()
    {
        foreach (var obj in _objects)
        {
            ImGui.PushID(obj.Id); 
            obj.Update();
            ImGui.PopID();
        }
    }

    public override void Shutdown()
    {
        foreach (var obj in _objects)
        {
            obj.Shutdown();
            _logger.Information("Shutdown Window {name}", obj.Label);
        }
    }
}
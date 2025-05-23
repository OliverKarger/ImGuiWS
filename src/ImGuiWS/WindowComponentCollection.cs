using ImGuiWS.Components;
using ImGuiWS.Components.Controls;
using ImGuiWS.Components.Modals;
using ImGuiWS.Components.Navigation;
using ImGuiWS.Logging;
using Serilog;

namespace ImGuiWS;

/// <summary>
///     Contains Window Components
/// </summary>
/// <typeparam name="T">Type of Component</typeparam>
public class WindowComponentCollection<T> : RenderableComponent where T : RenderableComponent
{
    public HashSet<T> Components { get; internal set; } = [];
    private readonly Dictionary<T, Action<T>> _configureActions = new();
    private readonly ILogger _logger;
    private readonly MainWindow _mainWindow;

    public WindowComponentCollection(Window? parentWindow, MainWindow mainWindow)
        : base($"WCC_{parentWindow?.Id}", mainWindow)
    {
        ParentWindow = parentWindow;
        _mainWindow = mainWindow;
        _logger = LoggerFactory.Create<WindowComponentCollection<T>>(parentWindow?.Id);
    }

    public TDerived? Add<TDerived>(TDerived component) where TDerived : T
    {
        return AddInternal(component, null);
    }

    public TDerived? Add<TDerived>(Func<TDerived> factory) where TDerived : T
    {
        return AddInternal(factory(), null);
    }

    public TDerived? Add<TDerived>(Func<TDerived> factory, Action<TDerived>? configure) where TDerived : T
    {
        return AddInternal(factory(), configure);
    }

    private TDerived? AddInternal<TDerived>(TDerived component, Action<TDerived>? configure) where TDerived : T
    {
        if (Components.Any(c => c.Id == component.Id))
        {
            _logger.Error("Component with ID {id} already present!", component.Id);
            return null;
        }

        component.ParentWindow ??= ParentWindow;
        component.MainWindow = _mainWindow;

        if (component is Window window)
        {
            window.Controls = new WindowComponentCollection<Control>(ParentWindow, _mainWindow);
            window.SubWindows = new WindowComponentCollection<Window>(ParentWindow, _mainWindow);
        }

        if (component is Menu menu)
        {
            menu.Items = new WindowComponentCollection<MenuItem>(ParentWindow, _mainWindow);
            menu.SubMenues = new WindowComponentCollection<Menu>(ParentWindow, _mainWindow);
        }

        if (component is Modal modal)
        {
            modal.Controls = new WindowComponentCollection<Control>(ParentWindow, _mainWindow);
        }

        component.SelfCheck();

        // Defer configuration
        if (configure != null)
        {
            _configureActions[component] = c => configure((TDerived)c);
        }

        Components.Add(component);
        _logger.Information("Added Component {id}", component.Id);

        return component;
    }

    public TComponent? Get<TComponent>(string id) where TComponent : T
    {
        var found = Components.FirstOrDefault(c => c.Id == id);
        if (found == null)
        {
            _logger.Error("No Component with ID {id} found", id);
            return null;
        }

        if (found is TComponent casted) return casted;

        _logger.Error("Component with ID {id} found, but Types don't match (Expected: {expected}, Actual: {actual})",
            id, typeof(TComponent).Name, found.GetType().Name);
        return null;
    }

    public override void Startup()
    {
        foreach (var component in Components)
        {
            if (_configureActions.TryGetValue(component, out var configure))
            {
                configure(component);
                _configureActions.Remove(component);
                _logger.Information("Configured Component {id}", component.Id);
            }

            component.Startup();
            _logger.Information("Initialized Component {id}", component.Id);
        }
    }

    public override void Update(float delta)
    {
        foreach (var component in Components)
        {
            component.Update(delta);
        }
    }

    public override void Shutdown()
    {
        foreach (var component in Components)
        {
            component.Shutdown();
            _logger.Information("Shutting down Component {id}", component.Id);
        }
    }
}
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
public class WindowComponentCollection<T> : RenderableComponent where T : RenderableComponent {
    private readonly Dictionary<T, Action<T>> _configureActions = new();
    private readonly ILogger _logger;
    private readonly MainWindow _mainWindow;

    public WindowComponentCollection(Window? parentWindow, MainWindow mainWindow)
        : base($"WCC_{parentWindow?.Id}", mainWindow) {
        this.ParentWindow = parentWindow;
        this._mainWindow = mainWindow;
        this._logger = LoggerFactory.Create<WindowComponentCollection<T>>(parentWindow?.Id);
    }

    public HashSet<T> Components { get; internal set; } = [];

    public TDerived? Add<TDerived>(TDerived component) where TDerived : T {
        return this.AddInternal(component, null);
    }

    public TDerived? Add<TDerived>(Func<TDerived> factory) where TDerived : T {
        return this.AddInternal(factory(), null);
    }

    public TDerived? Add<TDerived>(Func<TDerived> factory, Action<TDerived>? configure) where TDerived : T {
        return this.AddInternal(factory(), configure);
    }

    private TDerived? AddInternal<TDerived>(TDerived component, Action<TDerived>? configure)
        where TDerived : T {
        if(this.Components.Any(c => c.Id == component.Id)) {
            this._logger.Error("Component with ID {id} already present!", component.Id);
            return null;
        }

        component.ParentWindow ??= this.ParentWindow;
        component.MainWindow = this._mainWindow;

        if(component is Window window) {
            window.Controls = new WindowComponentCollection<Control>(this.ParentWindow, this._mainWindow);
            window.SubWindows = new WindowComponentCollection<Window>(this.ParentWindow, this._mainWindow);
        }

        if(component is Menu menu) {
            menu.Items = new WindowComponentCollection<MenuItem>(this.ParentWindow, this._mainWindow);
            menu.SubMenues = new WindowComponentCollection<Menu>(this.ParentWindow, this._mainWindow);
        }

        if(component is Modal modal) {
            modal.Controls = new WindowComponentCollection<Control>(this.ParentWindow, this._mainWindow);
        }

        component.SelfCheck();

        // Defer configuration
        if(configure != null) {
            this._configureActions[component] = c => configure((TDerived)c);
        }

        this.Components.Add(component);
        this._logger.Information("Added Component {id}", component.Id);

        return component;
    }

    public TComponent? Get<TComponent>(String id) where TComponent : T {
        T? found = this.Components.FirstOrDefault(c => c.Id == id);
        if(found == null) {
            this._logger.Error("No Component with ID {id} found", id);
            return null;
        }

        if(found is TComponent casted) {
            return casted;
        }

        this._logger.Error(
            "Component with ID {id} found, but Types don't match (Expected: {expected}, Actual: {actual})",
            id, typeof(TComponent).Name, found.GetType().Name);
        return null;
    }

    public override void Startup() {
        foreach(T component in this.Components) {
            if(this._configureActions.TryGetValue(component, out Action<T>? configure)) {
                configure(component);
                this._configureActions.Remove(component);
                this._logger.Information("Configured Component {id}", component.Id);
            }

            component.Startup();
            this._logger.Information("Initialized Component {id}", component.Id);
        }
    }

    public override void Update(Single delta) {
        foreach(T component in this.Components) {
            component.Update(delta);
        }
    }

    public override void Shutdown() {
        foreach(T component in this.Components) {
            component.Shutdown();
            this._logger.Information("Shutting down Component {id}", component.Id);
        }
    }
}
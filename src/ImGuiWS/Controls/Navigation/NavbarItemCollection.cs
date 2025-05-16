using System.Data;
using ImGuiWS.Logging;
using ImGuiWS.Renderer;
using Serilog;

namespace ImGuiWS.Controls.Navigation;

public class NavbarItemCollection(
    MainWindow rootWindow, 
    Navbar navbar,
    NavbarMenu? navbarMenu,
    Window? directParent) : RenderObjectCollection<NavbarItem>(rootWindow, directParent)
{
    private readonly ILogger _logger = LoggerFactory.Create<NavbarItemCollection>(directParent?.Options.Label);
    
    public Navbar Navbar { get; set; } = navbar;
    public NavbarMenu? NavbarMenu { get; set; } = navbarMenu;
    
    public override RenderObjectCollection<NavbarItem> Add<TDerived>(Func<TDerived> factory, Action<TDerived>? configure)
    {
        TDerived control = factory();

        control.RootWindow = RootWindow;
        control.DirectParent = DirectParent;
        control.ParentMenu = navbarMenu;
        control.Navbar = Navbar;
        
        configure?.Invoke(control);
        
        if (_objects.Any(e => e.Id == control.Id))
        {
            throw new DuplicateNameException("Duplicate control name/id"); 
        }

        _objects.Add(control);
        _logger.Information("Added Control {id} to Navbar {navbar}", control.Id, Navbar.Id);
        return this;
    }
}
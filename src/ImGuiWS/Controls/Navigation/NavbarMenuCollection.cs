using System.Data;
using ImGuiWS.Logging;
using ImGuiWS.Renderer;
using Serilog;

namespace ImGuiWS.Controls.Navigation;

public class NavbarMenuCollection(
    MainWindow rootWindow, 
    Navbar navbar,
    NavbarMenu? navbarMenu,
    Window? directParent) : RenderObjectCollection<NavbarMenu>(rootWindow, directParent)
{
    private readonly ILogger _logger = LoggerFactory.Create<NavbarItemCollection>(navbar.Id);
    
    public Navbar Navbar { get; set; } = navbar;
    public NavbarMenu? NavbarMenu { get; set; } = navbarMenu;
    
    public override RenderObjectCollection<NavbarMenu> Add<TDerived>(Func<TDerived> factory, Action<TDerived>? configure)
    {
        TDerived control = factory();

        control.RootWindow = RootWindow;
        control.DirectParent = DirectParent;
        control.ParentMenu = navbarMenu;
        control.Navbar = Navbar;

        control.Items = new NavbarItemCollection(RootWindow, Navbar, NavbarMenu, DirectParent);
        control.Menus = new NavbarMenuCollection(RootWindow, Navbar, NavbarMenu, DirectParent);
        
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
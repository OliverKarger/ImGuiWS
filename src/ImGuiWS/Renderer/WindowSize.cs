namespace ImGuiWS.Renderer;

public class WindowSize<T> where T : struct, IComparable, IComparable<T>, IConvertible, IEquatable<T>, IFormattable
{
    public T Width { get; set; }
    public T Height { get; set; } 
}
namespace ImGuiWS.Renderer;

public class WindowPosition<T> where T : struct, IComparable, IComparable<T>, IConvertible, IEquatable<T>, IFormattable
{
    public T Left { get; set; }
    public T Top { get; set; } 
}

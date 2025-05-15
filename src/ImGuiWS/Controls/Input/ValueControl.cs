using ImGuiWS.Controls.Utils;

namespace ImGuiWS.Controls;

public abstract class ValueControl<T>(string id) : ControlBase(id)
{
    protected ValueControl(T initialValue, string id) : this(id)
    {
        Value = initialValue;
    }
    
    public T? Value { get; set; }
    
    public Action<T>? OnValueChanged;

    internal void ValueChanged(T value) => OnValueChanged?.Invoke(value);
}
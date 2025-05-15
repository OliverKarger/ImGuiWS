using System.Runtime.CompilerServices;
using ImGuiNET;
using ImGuiWS.Controls.Utils;

namespace ImGuiWS.Controls;

public class InputBox<T> : ValueControl<T>
{
    public string Label { get; set; }
    public uint MaxLength { get; set; } = 32;
    
    public InputBox(string label) : base(label.ToControlId())
    {
        Label = label;
    }

    public InputBox(string label, T initialValue) : base(initialValue, label.ToControlId())
    {
        Label = label;
    }
    
    public override void Render()
    {
        switch (Type.GetTypeCode(typeof(T)))
        {
            case TypeCode.String:
            {
                string tempValue = (Value as string) ?? string.Empty;
                if (ImGui.InputText(Label, ref tempValue, MaxLength))
                {
                    Value = (T)Convert.ChangeType(tempValue, typeof(T));
                    ValueChanged(Value);
                }
                break;
            }

            case TypeCode.Int16:
            {
                int temp = Convert.ToInt32(Value);
                if (ImGui.InputInt(Label, ref temp))
                {
                    Value = (T)Convert.ChangeType((short)temp, typeof(T));
                    ValueChanged(Value);
                }
                break;
            }

            case TypeCode.Int32:
            {
                int temp = Convert.ToInt32(Value);
                if (ImGui.InputInt(Label, ref temp))
                {
                    Value = (T)Convert.ChangeType(temp, typeof(T));
                    ValueChanged(Value);
                }
                break;
            }

            case TypeCode.Int64:
            {
                long longValue = Convert.ToInt64(Value);
                int temp = (int)Math.Clamp(longValue, int.MinValue, int.MaxValue);
                if (ImGui.InputInt(Label, ref temp))
                {
                    Value = (T)Convert.ChangeType((long)temp, typeof(T));
                    ValueChanged(Value);
                }
                break;
            }

            case TypeCode.UInt16:
            {
                int temp = Convert.ToInt32(Value);
                if (ImGui.InputInt(Label, ref temp))
                {
                    Value = (T)Convert.ChangeType((ushort)Math.Max(0, temp), typeof(T));
                    ValueChanged(Value);
                }
                break;
            }

            case TypeCode.UInt32:
            {
                int temp = Convert.ToInt32(Value);
                if (ImGui.InputInt(Label, ref temp))
                {
                    Value = (T)Convert.ChangeType((uint)Math.Max(0, temp), typeof(T));
                    ValueChanged(Value);
                }
                break;
            }

            case TypeCode.UInt64:
            {
                ulong ulongValue = Convert.ToUInt64(Value);
                int temp = (int)Math.Clamp(ulongValue, 0, int.MaxValue);
                if (ImGui.InputInt(Label, ref temp))
                {
                    Value = (T)Convert.ChangeType((ulong)temp, typeof(T));
                    ValueChanged(Value);
                }
                break;
            }

            case TypeCode.Single:
            {
                float temp = Convert.ToSingle(Value);
                if (ImGui.InputFloat(Label, ref temp))
                {
                    Value = (T)Convert.ChangeType(temp, typeof(T));
                    ValueChanged(Value);
                }
                break;
            }

            case TypeCode.Double:
            {
                double temp = Convert.ToDouble(Value);
                if (ImGui.InputDouble(Label, ref temp))
                {
                    Value = (T)Convert.ChangeType(temp, typeof(T));
                    ValueChanged(Value);
                }
                break;
            }

            case TypeCode.Decimal:
            {
                float temp = (float)Convert.ToDecimal(Value);
                if (ImGui.InputFloat(Label, ref temp))
                {
                    Value = (T)Convert.ChangeType((decimal)temp, typeof(T));
                    ValueChanged(Value);
                }
                break;
            }

            case TypeCode.Boolean:
            {
                bool temp = Convert.ToBoolean(Value);
                if (ImGui.Checkbox(Label, ref temp))
                {
                    Value = (T)Convert.ChangeType(temp, typeof(T));
                    ValueChanged(Value);
                }
                break;
            }

            default:
                throw new NotImplementedException($"Type {typeof(T).Name} is not implemented!");
        }
    }
}
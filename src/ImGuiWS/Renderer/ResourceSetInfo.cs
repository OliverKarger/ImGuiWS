using Veldrid;

namespace ImGuiWS.Renderer;

/// <summary>
///     Contains Informations about a Resource Set Binding in ImGui
/// </summary>
public class ResourceSetInfo(IntPtr imGuiBinding, ResourceSet resourceSet)
{
    public readonly IntPtr ImGuiBinding = imGuiBinding;
    public readonly ResourceSet ResourceSet = resourceSet;
}
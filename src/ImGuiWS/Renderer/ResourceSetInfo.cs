using Veldrid;

namespace ImGuiWS.Renderer;

public class ResourceSetInfo
{
    public readonly IntPtr ImGuiBinding;
    public readonly ResourceSet ResourceSet;

    public ResourceSetInfo(IntPtr imGuiBinding, ResourceSet resourceSet)
    {
        ImGuiBinding = imGuiBinding;
        ResourceSet = resourceSet;
    }
}
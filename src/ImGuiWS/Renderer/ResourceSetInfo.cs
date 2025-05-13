using System.Resources;

namespace ImGuiWS.Renderer;

public class ResourceSetInfo(IntPtr bindingId, ResourceSet resourceSet)
{
    public readonly IntPtr BindingId = bindingId;
    public readonly ResourceSet ResourceSet = resourceSet;
}
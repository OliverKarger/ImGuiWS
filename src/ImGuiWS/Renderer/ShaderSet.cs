using Veldrid;

namespace ImGuiWS.Renderer;

public class ShaderSet(Shader vertexShader, Shader fragmentShader) : IDisposable
{
   public Shader VertexShader { get; set; } = vertexShader;
   public Shader FragmentShader { get; set; } = fragmentShader;

   public void Dispose()
   {
      VertexShader.Dispose();
      FragmentShader.Dispose();
      VertexShader.Dispose();
      FragmentShader.Dispose();
   }
}
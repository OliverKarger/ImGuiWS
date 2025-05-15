using System.Numerics;
using System.Reflection;
using System.Runtime.CompilerServices;
using ImGuiNET;
using Veldrid;
using Veldrid.Sdl2;
using Veldrid.StartupUtilities;

namespace ImGuiWS.Renderer;

public class WindowBackend : IDisposable
{
    public WindowBackendContext Context { get; internal set; } = new();
    public WindowModifierKeyState ModifierKeyState { get; internal set; } = new();
    public WindowState State { get; internal set; } = new();

    private readonly MainWindow _mainWindow;
    
    /// <summary>
    /// Constructs a new ImGuiController.
    /// </summary>
    public WindowBackend(WindowCreateInfo windowCreateInfo, MainWindow mainWindowHandle)
    {
        _mainWindow = mainWindowHandle;
        VeldridStartup.CreateWindowAndGraphicsDevice(
            windowCreateInfo,
            new GraphicsDeviceOptions(true, null, true, ResourceBindingModel.Improved, true, true),
            out var window,
            out var graphicsDevice);
        
        Context.GraphicsDevice = graphicsDevice;
        Context.Window = window;

        Context.Window.Resized += () =>
        {
            Context.GraphicsDevice.MainSwapchain.Resize(
                (uint)Context.Window.Width, 
                (uint)Context.Window.Height);
            
            State.WindowSize = State.WindowSize with
            {
                X = (float)Context.Window.Width, 
                Y = (float)Context.Window.Height
            };
            _mainWindow.Events.InvokeWindowResized(
                Context.Window.Width,
                Context.Window.Height);
        };
        
        Context.CommandList = Context.GraphicsDevice.ResourceFactory.CreateCommandList();

        State.WindowSize = State.WindowSize with { X = (float)windowCreateInfo.WindowWidth, Y = (float)windowCreateInfo.WindowHeight };
        ImGui.CreateContext();
        var io = ImGui.GetIO();
        io.BackendFlags |= ImGuiBackendFlags.RendererHasVtxOffset;
        io.ConfigFlags |= ImGuiConfigFlags.NavEnableKeyboard |
                          ImGuiConfigFlags.DockingEnable;
        io.Fonts.Flags &= ~ImFontAtlasFlags.NoBakedLines;
        // io.Fonts.Flags |= ImFontAtlasFlags.NoBakedLines;
    }

    internal void PrepareRender()
    {
        CreateDeviceResources(Context.GraphicsDevice, Context.GraphicsDevice.MainSwapchain.Framebuffer.OutputDescription);
        SetPerFrameImGuiData(1f / 60f);
        ImGui.NewFrame();
        State.FrameBegun = true;
    }
    
    public void CreateDeviceResources(GraphicsDevice gd, OutputDescription outputDescription)
    {
        ResourceFactory factory = gd.ResourceFactory;
        Context.VertexBuffer = factory.CreateBuffer(new BufferDescription(10000, BufferUsage.VertexBuffer | BufferUsage.Dynamic));
        Context.VertexBuffer.Name = "ImGui.NET Vertex Buffer";
        Context.IndexBuffer = factory.CreateBuffer(new BufferDescription(2000, BufferUsage.IndexBuffer | BufferUsage.Dynamic));
        Context.IndexBuffer.Name = "ImGui.NET Index Buffer";
        RecreateFontDeviceTexture(gd);

        Context.ProjectionBuffer = factory.CreateBuffer(new BufferDescription(64, BufferUsage.UniformBuffer | BufferUsage.Dynamic));
        Context.ProjectionBuffer.Name = "ImGui.NET Projection Buffer";

        byte[] vertexShaderBytes = LoadEmbeddedShaderCode(gd.ResourceFactory, "imgui-vertex", ShaderStages.Vertex);
        byte[] fragmentShaderBytes = LoadEmbeddedShaderCode(gd.ResourceFactory, "imgui-frag", ShaderStages.Fragment);
        Context.VertexShader = factory.CreateShader(new ShaderDescription(ShaderStages.Vertex, vertexShaderBytes, gd.BackendType == GraphicsBackend.Metal ? "VS" : "main"));
        Context.FragmentShader = factory.CreateShader(new ShaderDescription(ShaderStages.Fragment, fragmentShaderBytes, gd.BackendType == GraphicsBackend.Metal ? "FS" : "main"));

        VertexLayoutDescription[] vertexLayouts = new VertexLayoutDescription[]
        {
            new VertexLayoutDescription(
                new VertexElementDescription("in_position", VertexElementSemantic.Position, VertexElementFormat.Float2),
                new VertexElementDescription("in_texCoord", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Float2),
                new VertexElementDescription("in_color", VertexElementSemantic.Color, VertexElementFormat.Byte4_Norm))
        };

        Context.ResLayout = factory.CreateResourceLayout(new ResourceLayoutDescription(
            new ResourceLayoutElementDescription("ProjectionMatrixBuffer", ResourceKind.UniformBuffer, ShaderStages.Vertex),
            new ResourceLayoutElementDescription("MainSampler", ResourceKind.Sampler, ShaderStages.Fragment)));
        Context.TexLayout = factory.CreateResourceLayout(new ResourceLayoutDescription(
            new ResourceLayoutElementDescription("MainTexture", ResourceKind.TextureReadOnly, ShaderStages.Fragment)));

        GraphicsPipelineDescription pd = new GraphicsPipelineDescription(
            BlendStateDescription.SingleAlphaBlend,
            new DepthStencilStateDescription(false, false, ComparisonKind.Always),
            new RasterizerStateDescription(FaceCullMode.None, PolygonFillMode.Solid, FrontFace.Clockwise, false, true),
            PrimitiveTopology.TriangleList,
            new ShaderSetDescription(vertexLayouts, new[] { Context.VertexShader, Context.FragmentShader}),
            new ResourceLayout[] {Context.ResLayout, Context.TexLayout},
            outputDescription,
            ResourceBindingModel.Default);
        Context.Pipeline = factory.CreateGraphicsPipeline(ref pd);

        Context.MainResourceSet= factory.CreateResourceSet(new ResourceSetDescription(Context.ResLayout,
            Context.ProjectionBuffer,
            gd.PointSampler));

        Context.FontTextureResourceSet = factory.CreateResourceSet(new ResourceSetDescription(Context.TexLayout, Context.FontTextureView));
    }

    /// <summary>
    /// Gets or creates a handle for a texture to be drawn with ImGui.
    /// Pass the returned handle to Image() or ImageButton().
    /// </summary>
    public IntPtr GetOrCreateImGuiBinding(ResourceFactory factory, TextureView textureView)
    {
        if (!Context.SetsByView.TryGetValue(textureView, out ResourceSetInfo rsi))
        {
            ResourceSet resourceSet = factory.CreateResourceSet(new ResourceSetDescription(Context.TexLayout, textureView));
            rsi = new ResourceSetInfo(GetNextImGuiBindingID(), resourceSet);

            Context.SetsByView.Add(textureView, rsi);
            Context.ViewsById.Add(rsi.ImGuiBinding, rsi);
            Context.OwnedResources.Add(resourceSet);
        }

        return rsi.ImGuiBinding;
    }

    private IntPtr GetNextImGuiBindingID()
    {
        int newID = State.LastAssignedId++;
        return (IntPtr)newID;
    }

    /// <summary>
    /// Gets or creates a handle for a texture to be drawn with ImGui.
    /// Pass the returned handle to Image() or ImageButton().
    /// </summary>
    public IntPtr GetOrCreateImGuiBinding(ResourceFactory factory, Texture texture)
    {
        if (!Context.AutoViewsByTexture.TryGetValue(texture, out TextureView textureView))
        {
            textureView = factory.CreateTextureView(texture);
            Context.AutoViewsByTexture.Add(texture, textureView);
            Context.OwnedResources.Add(textureView);
        }

        return GetOrCreateImGuiBinding(factory, textureView);
    }

    /// <summary>
    /// Retrieves the shader texture binding for the given helper handle.
    /// </summary>
    public ResourceSet GetImageResourceSet(IntPtr imGuiBinding)
    {
        if (!Context.ViewsById.TryGetValue(imGuiBinding, out ResourceSetInfo tvi))
        {
            throw new InvalidOperationException("No registered ImGui binding with id " + imGuiBinding.ToString());
        }

        return tvi.ResourceSet;
    }

    private byte[] LoadEmbeddedShaderCode(ResourceFactory factory, string name, ShaderStages stage)
    {
        switch (factory.BackendType)
        {
            case GraphicsBackend.Direct3D11:
            {
                string resourceName = name + ".hlsl.bytes";
                return GetEmbeddedResourceBytes(resourceName);
            }
            case GraphicsBackend.OpenGL:
            {
                string resourceName = name + ".glsl";
                return GetEmbeddedResourceBytes(resourceName);
            }
            case GraphicsBackend.Vulkan:
            {
                string resourceName = name + ".spv";
                return GetEmbeddedResourceBytes(resourceName);
            }
            case GraphicsBackend.Metal:
            {
                string resourceName = name + ".metallib";
                return GetEmbeddedResourceBytes(resourceName);
            }
            default:
                throw new NotImplementedException();
        }
    }

    private byte[] GetEmbeddedResourceBytes(string resourceName)
    {
        Assembly assembly = typeof(WindowBackend).Assembly;
        using (Stream s = assembly.GetManifestResourceStream(resourceName))
        {
            byte[] ret = new byte[s.Length];
            s.Read(ret, 0, (int)s.Length);
            return ret;
        }
    }

    /// <summary>
    /// Recreates the device texture used to render text.
    /// </summary>
    public void RecreateFontDeviceTexture(GraphicsDevice gd)
    {
        ImGuiIOPtr io = ImGui.GetIO();
        // Build
        IntPtr pixels;
        int width, height, bytesPerPixel;
        io.Fonts.GetTexDataAsRGBA32(out pixels, out width, out height, out bytesPerPixel);
        // Store our identifier
        io.Fonts.SetTexID(State.FontAtlasId);

        Context.FontTexture = gd.ResourceFactory.CreateTexture(TextureDescription.Texture2D(
            (uint)width,
            (uint)height,
            1,
            1,
            PixelFormat.R8_G8_B8_A8_UNorm,
            TextureUsage.Sampled));
        Context.FontTexture.Name = "ImGui.NET Font Texture";
        gd.UpdateTexture(
            Context.FontTexture,
            pixels,
            (uint)(bytesPerPixel * width * height),
            0,
            0,
            0,
            (uint)width,
            (uint)height,
            1,
            0,
            0);
        Context.FontTextureView = gd.ResourceFactory.CreateTextureView(Context.FontTexture);

        io.Fonts.ClearTexData();
    }

    public void Submit()
    {
        Context.CommandList.Begin();
        Context.CommandList.SetFramebuffer(Context.GraphicsDevice.MainSwapchain.Framebuffer);
        Context.CommandList.ClearColorTarget(0, 
            new RgbaFloat(State.ClearColor.X, State.ClearColor.Y, State.ClearColor.Z, State.ClearColor.W));
        Render();
        Context.CommandList.End();
        Context.GraphicsDevice.SubmitCommands(Context.CommandList);
        Context.GraphicsDevice.SwapBuffers(Context.GraphicsDevice.MainSwapchain);
    }
    
    /// <summary>
    /// Renders the ImGui draw list data.
    /// This method requires a <see cref="GraphicsDevice"/> because it may create new DeviceBuffers if the size of vertex
    /// or index data has increased beyond the capacity of the existing buffers.
    /// A <see cref="CommandList"/> is needed to submit drawing and resource update commands.
    /// </summary>
    private void Render()
    {
        if (State.FrameBegun)
        {
            State.FrameBegun = false;
            ImGui.Render();
            RenderImDrawData(ImGui.GetDrawData(), Context.GraphicsDevice, Context.CommandList);
        }
    }

    /// <summary>
    /// Updates ImGui input and IO configuration state.
    /// </summary>
    public void Update(float deltaSeconds, InputSnapshot snapshot)
    {
        State.RenderingBegun = true;
        if (State.FrameBegun)
        {
            ImGui.Render();
        }

        SetPerFrameImGuiData(deltaSeconds);
        UpdateImGuiInput(snapshot);

        State.FrameBegun = true;
        ImGui.NewFrame();
    }

    /// <summary>
    /// Sets per-frame data based on the associated window.
    /// This is called by Update(float).
    /// </summary>
    private void SetPerFrameImGuiData(float deltaSeconds)
    {
        ImGuiIOPtr io = ImGui.GetIO();
        io.DisplaySize = new Vector2(
            State.WindowSize.X / State.ScaleFactor.X,
            State.WindowSize.Y / State.ScaleFactor.Y);
        io.DisplayFramebufferScale = State.ScaleFactor;
        io.DeltaTime = deltaSeconds; // DeltaTime is in seconds.
    }

    private bool TryMapKey(Key key, out ImGuiKey result)
    {
        ImGuiKey KeyToImGuiKeyShortcut(Key keyToConvert, Key startKey1, ImGuiKey startKey2)
        {
            int changeFromStart1 = (int)keyToConvert - (int)startKey1;
            return startKey2 + changeFromStart1;
        }

        result = key switch
        {
            >= Key.F1 and <= Key.F24 => KeyToImGuiKeyShortcut(key, Key.F1, ImGuiKey.F1),
            >= Key.Keypad0 and <= Key.Keypad9 => KeyToImGuiKeyShortcut(key, Key.Keypad0, ImGuiKey.Keypad0),
            >= Key.A and <= Key.Z => KeyToImGuiKeyShortcut(key, Key.A, ImGuiKey.A),
            >= Key.Number0 and <= Key.Number9 => KeyToImGuiKeyShortcut(key, Key.Number0, ImGuiKey._0),
            Key.ShiftLeft or Key.ShiftRight => ImGuiKey.ModShift,
            Key.ControlLeft or Key.ControlRight => ImGuiKey.ModCtrl,
            Key.AltLeft or Key.AltRight => ImGuiKey.ModAlt,
            Key.WinLeft or Key.WinRight => ImGuiKey.ModSuper,
            Key.Menu => ImGuiKey.Menu,
            Key.Up => ImGuiKey.UpArrow,
            Key.Down => ImGuiKey.DownArrow,
            Key.Left => ImGuiKey.LeftArrow,
            Key.Right => ImGuiKey.RightArrow,
            Key.Enter => ImGuiKey.Enter,
            Key.Escape => ImGuiKey.Escape,
            Key.Space => ImGuiKey.Space,
            Key.Tab => ImGuiKey.Tab,
            Key.BackSpace => ImGuiKey.Backspace,
            Key.Insert => ImGuiKey.Insert,
            Key.Delete => ImGuiKey.Delete,
            Key.PageUp => ImGuiKey.PageUp,
            Key.PageDown => ImGuiKey.PageDown,
            Key.Home => ImGuiKey.Home,
            Key.End => ImGuiKey.End,
            Key.CapsLock => ImGuiKey.CapsLock,
            Key.ScrollLock => ImGuiKey.ScrollLock,
            Key.PrintScreen => ImGuiKey.PrintScreen,
            Key.Pause => ImGuiKey.Pause,
            Key.NumLock => ImGuiKey.NumLock,
            Key.KeypadDivide => ImGuiKey.KeypadDivide,
            Key.KeypadMultiply => ImGuiKey.KeypadMultiply,
            Key.KeypadSubtract => ImGuiKey.KeypadSubtract,
            Key.KeypadAdd => ImGuiKey.KeypadAdd,
            Key.KeypadDecimal => ImGuiKey.KeypadDecimal,
            Key.KeypadEnter => ImGuiKey.KeypadEnter,
            Key.Tilde => ImGuiKey.GraveAccent,
            Key.Minus => ImGuiKey.Minus,
            Key.Plus => ImGuiKey.Equal,
            Key.BracketLeft => ImGuiKey.LeftBracket,
            Key.BracketRight => ImGuiKey.RightBracket,
            Key.Semicolon => ImGuiKey.Semicolon,
            Key.Quote => ImGuiKey.Apostrophe,
            Key.Comma => ImGuiKey.Comma,
            Key.Period => ImGuiKey.Period,
            Key.Slash => ImGuiKey.Slash,
            Key.BackSlash or Key.NonUSBackSlash => ImGuiKey.Backslash,
            _ => ImGuiKey.None
        };

        return result != ImGuiKey.None;
    }

    private void UpdateImGuiInput(InputSnapshot snapshot)
    {
        ImGuiIOPtr io = ImGui.GetIO();
        io.AddMousePosEvent(snapshot.MousePosition.X, snapshot.MousePosition.Y);
        io.AddMouseButtonEvent(0, snapshot.IsMouseDown(MouseButton.Left));
        io.AddMouseButtonEvent(1, snapshot.IsMouseDown(MouseButton.Right));
        io.AddMouseButtonEvent(2, snapshot.IsMouseDown(MouseButton.Middle));
        io.AddMouseButtonEvent(3, snapshot.IsMouseDown(MouseButton.Button1));
        io.AddMouseButtonEvent(4, snapshot.IsMouseDown(MouseButton.Button2));
        io.AddMouseWheelEvent(0f, snapshot.WheelDelta);
        for (int i = 0; i < snapshot.KeyCharPresses.Count; i++)
        {
            io.AddInputCharacter(snapshot.KeyCharPresses[i]);
        }

        for (int i = 0; i < snapshot.KeyEvents.Count; i++)
        {
            KeyEvent keyEvent = snapshot.KeyEvents[i];
            if (TryMapKey(keyEvent.Key, out ImGuiKey imguikey))
            {
                io.AddKeyEvent(imguikey, keyEvent.Down);
            }
        }
    }

    private void RenderImDrawData(ImDrawDataPtr draw_data, GraphicsDevice gd, CommandList cl)
    {
        uint vertexOffsetInVertices = 0;
        uint indexOffsetInElements = 0;

        if (draw_data.CmdListsCount == 0)
        {
            return;
        }

        uint totalVBSize = (uint)(draw_data.TotalVtxCount * Unsafe.SizeOf<ImDrawVert>());
        if (totalVBSize > Context.VertexBuffer.SizeInBytes)
        {
            gd.DisposeWhenIdle(Context.VertexBuffer);
            Context.VertexBuffer = gd.ResourceFactory.CreateBuffer(new BufferDescription((uint)(totalVBSize * 1.5f), BufferUsage.VertexBuffer | BufferUsage.Dynamic));
        }

        uint totalIBSize = (uint)(draw_data.TotalIdxCount * sizeof(ushort));
        if (totalIBSize > Context.IndexBuffer.SizeInBytes)
        {
            gd.DisposeWhenIdle(Context.IndexBuffer);
            Context.IndexBuffer = gd.ResourceFactory.CreateBuffer(new BufferDescription((uint)(totalIBSize * 1.5f), BufferUsage.IndexBuffer | BufferUsage.Dynamic));
        }

        for (int i = 0; i < draw_data.CmdListsCount; i++)
        {
            ImDrawListPtr cmd_list = draw_data.CmdLists[i];

            cl.UpdateBuffer(
                Context.VertexBuffer,
                vertexOffsetInVertices * (uint)Unsafe.SizeOf<ImDrawVert>(),
                cmd_list.VtxBuffer.Data,
                (uint)(cmd_list.VtxBuffer.Size * Unsafe.SizeOf<ImDrawVert>()));

            cl.UpdateBuffer(
                Context.IndexBuffer,
                indexOffsetInElements * sizeof(ushort),
                cmd_list.IdxBuffer.Data,
                (uint)(cmd_list.IdxBuffer.Size * sizeof(ushort)));

            vertexOffsetInVertices += (uint)cmd_list.VtxBuffer.Size;
            indexOffsetInElements += (uint)cmd_list.IdxBuffer.Size;
        }

        // Setup orthographic projection matrix into our constant buffer
        ImGuiIOPtr io = ImGui.GetIO();
        Matrix4x4 mvp = Matrix4x4.CreateOrthographicOffCenter(
            0f,
            io.DisplaySize.X,
            io.DisplaySize.Y,
            0.0f,
            -1.0f,
            1.0f);

        Context.GraphicsDevice.UpdateBuffer(Context.ProjectionBuffer, 0, ref mvp);

        cl.SetVertexBuffer(0, Context.VertexBuffer);
        cl.SetIndexBuffer(Context.IndexBuffer, IndexFormat.UInt16);
        cl.SetPipeline(Context.Pipeline);
        cl.SetGraphicsResourceSet(0, Context.MainResourceSet);

        draw_data.ScaleClipRects(io.DisplayFramebufferScale);

        // Render command lists
        int vtx_offset = 0;
        int idx_offset = 0;
        for (int n = 0; n < draw_data.CmdListsCount; n++)
        {
            ImDrawListPtr cmd_list = draw_data.CmdLists[n];
            for (int cmd_i = 0; cmd_i < cmd_list.CmdBuffer.Size; cmd_i++)
            {
                ImDrawCmdPtr pcmd = cmd_list.CmdBuffer[cmd_i];
                if (pcmd.UserCallback != IntPtr.Zero)
                {
                    throw new NotImplementedException();
                }
                else
                {
                    if (pcmd.TextureId != IntPtr.Zero)
                    {
                        if (pcmd.TextureId == State.FontAtlasId)
                        {
                            cl.SetGraphicsResourceSet(1, Context.FontTextureResourceSet);
                        }
                        else
                        {
                            cl.SetGraphicsResourceSet(1, GetImageResourceSet(pcmd.TextureId));
                        }
                    }

                    cl.SetScissorRect(
                        0,
                        (uint)pcmd.ClipRect.X,
                        (uint)pcmd.ClipRect.Y,
                        (uint)(pcmd.ClipRect.Z - pcmd.ClipRect.X),
                        (uint)(pcmd.ClipRect.W - pcmd.ClipRect.Y));

                    cl.DrawIndexed(pcmd.ElemCount, 1, pcmd.IdxOffset + (uint)idx_offset, (int)pcmd.VtxOffset + vtx_offset, 0);
                }
            }
            vtx_offset += cmd_list.VtxBuffer.Size;
            idx_offset += cmd_list.IdxBuffer.Size;
        }
    }

    /// <summary>
    /// Frees all graphics resources used by the renderer.
    /// </summary>
    public void Dispose()
    {
        Context.Dispose();
    }
}
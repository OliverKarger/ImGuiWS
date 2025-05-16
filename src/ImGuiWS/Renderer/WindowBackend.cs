using System.Numerics;
using System.Reflection;
using System.Runtime.CompilerServices;
using ImGuiNET;
using ImGuiWS.Utils;
using Veldrid;
using Veldrid.Sdl2;
using Veldrid.StartupUtilities;

namespace ImGuiWS.Renderer;

public class WindowBackend : IDisposable
{
    /// <inheritdoc cref="WindowBackendContext"/> 
    public WindowBackendContext Context { get; internal set; } = new();
    
    /// <inheritdoc cref="WindowModifierKeyState"/> 
    public WindowModifierKeyState ModifierKeyState { get; internal set; } = new();
    
    /// <inheritdoc cref="WindowState"/>     
    public WindowState State { get; internal set; } = new();

    /// <summary>
    ///     Returns the next available ImGui Binding ID
    ///     or Updates the current one
    /// </summary>
    public IntPtr NextId
    {
        get
        {
            IntPtr nextId = State.LastAssignedId++;
            return nextId;
        }
        private set => State.LastAssignedId = value;
    }

    /// <summary>
    ///     Main Constructor
    ///     Initializes Rendering and ImGui Context
    /// </summary>
    /// <param name="windowCreateInfo">
    ///     Setup Information about the SDL2/Veldrid Window
    /// </param>
    /// <param name="mainWindowHandle">
    ///     Handle to the <see cref="MainWindow"/> associated with this Backend
    /// </param>
    public WindowBackend(WindowCreateInfo windowCreateInfo, MainWindow mainWindowHandle)
    {
        var mainWindow = mainWindowHandle;
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
            mainWindow.Events.InvokeWindowResized(
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
    }

    /// <summary>
    ///     Prepares the ImGui Context
    /// </summary>
    /// <remarks>
    ///     Needs to be called <b>after</b> Font/Resource allocation!
    /// </remarks>
    internal void SetupContext()
    {
        CreateDeviceResources();
        SetPerFrameImGuiData(1f / 60f);
        ImGui.NewFrame();
        State.FrameBegun = true;
    }
    
    /// <summary>
    ///     Allocates Resources like Shaders and Buffers
    /// </summary>
    private void CreateDeviceResources()
    {
        ResourceFactory factory = Context.GraphicsDevice.ResourceFactory;
        Context.VertexBuffer = factory.CreateBuffer(new BufferDescription(10000, BufferUsage.VertexBuffer | BufferUsage.Dynamic));
        Context.VertexBuffer.Name = "ImGui.NET Vertex Buffer";
        Context.IndexBuffer = factory.CreateBuffer(new BufferDescription(2000, BufferUsage.IndexBuffer | BufferUsage.Dynamic));
        Context.IndexBuffer.Name = "ImGui.NET Index Buffer";
        RecreateFontDeviceTexture();

        Context.ProjectionBuffer = factory.CreateBuffer(new BufferDescription(64, BufferUsage.UniformBuffer | BufferUsage.Dynamic));
        Context.ProjectionBuffer.Name = "ImGui.NET Projection Buffer";

        byte[] vertexShaderBytes = LoadEmbeddedShaderCode( "imgui-vertex", ShaderStages.Vertex);
        byte[] fragmentShaderBytes = LoadEmbeddedShaderCode("imgui-frag", ShaderStages.Fragment);
        Context.VertexShader = factory.CreateShader(new ShaderDescription(ShaderStages.Vertex, vertexShaderBytes, Context.GraphicsDevice.BackendType == GraphicsBackend.Metal ? "VS" : "main"));
        Context.FragmentShader = factory.CreateShader(new ShaderDescription(ShaderStages.Fragment, fragmentShaderBytes, Context.GraphicsDevice.BackendType == GraphicsBackend.Metal ? "FS" : "main"));

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
            Context.GraphicsDevice.MainSwapchain.Framebuffer.OutputDescription,
            ResourceBindingModel.Default);
        Context.Pipeline = factory.CreateGraphicsPipeline(ref pd);

        Context.MainResourceSet = factory.CreateResourceSet(new ResourceSetDescription(Context.ResLayout,
            Context.ProjectionBuffer,
            Context.GraphicsDevice.PointSampler));

        Context.FontTextureResourceSet = factory.CreateResourceSet(new ResourceSetDescription(Context.TexLayout, Context.FontTextureView));
    }

    /// <summary>
    ///     Returns or Creates a ImGui Binding for a <see cref="TextureView"/>
    /// </summary>
    public IntPtr GetOrCreateImGuiBinding(TextureView textureView)
    {
        if (!Context.SetsByView.TryGetValue(textureView, out ResourceSetInfo rsi))
        {
            ResourceSet resourceSet = Context.GraphicsDevice.ResourceFactory.CreateResourceSet(new ResourceSetDescription(Context.TexLayout, textureView));
            rsi = new ResourceSetInfo(NextId, resourceSet);

            Context.SetsByView.Add(textureView, rsi);
            Context.ViewsById.Add(rsi.ImGuiBinding, rsi);
            Context.OwnedResources.Add(resourceSet);
        }

        return rsi.ImGuiBinding;
    }
    
    /// <summary>
    ///     Returns or Creates a ImGui Binding for a <see cref="Texture"/>
    /// </summary>
    public IntPtr GetOrCreateImGuiBinding(Texture texture)
    {
        if (!Context.AutoViewsByTexture.TryGetValue(texture, out TextureView textureView))
        {
            textureView = Context.GraphicsDevice.ResourceFactory.CreateTextureView(texture);
            Context.AutoViewsByTexture.Add(texture, textureView);
            Context.OwnedResources.Add(textureView);
        }

        return GetOrCreateImGuiBinding(textureView);
        
    }

    /// <summary>
    ///     Returns a Resource Set for the specified ImGui Binding
    /// </summary>
    /// <param name="imGuiBinding">
    ///     ImGui Binding ID
    /// </param>
    /// <exception cref="InvalidOperationException">
    ///     Thrown if no Resource Set for the specified Binding Id exists
    /// </exception>
    public ResourceSet GetImageResourceSet(IntPtr imGuiBinding)
    {
        if (!Context.ViewsById.TryGetValue(imGuiBinding, out ResourceSetInfo tvi))
        {
            throw new InvalidOperationException("No registered ImGui binding with id " + imGuiBinding.ToString());
        }

        return tvi.ResourceSet;
    }

    /// <summary>
    ///     Loads the embedded shader code for the specified name and shader stage.
    ///     The backend is determined using <see cref="Veldrid.GraphicsDevice.BackendType"/> 
    ///     via <c>WindowBackend.Context.GraphicsDevice.ResourceFactory.BackendType</c>.
    /// </summary>
    /// <param name="name">The name of the embedded shader.</param>
    /// <param name="stage">The shader stage (e.g., Vertex, Fragment).</param>
    /// <returns>The source code of the shader as a string.</returns>
    /// <exception cref="System.NotImplementedException">
    ///     Thrown if the method is not implemented for the specified backend.
    /// </exception>
    private byte[] LoadEmbeddedShaderCode(string name, ShaderStages stage)
    {
        string resourceName;
        switch (Context.GraphicsDevice.ResourceFactory.BackendType)
        {
            case GraphicsBackend.Direct3D11:
            {
                resourceName = name + ".hlsl.bytes";
                break;
            }
            case GraphicsBackend.OpenGL:
            {
                resourceName = name + ".glsl";
                break;
            }
            case GraphicsBackend.Vulkan:
            {
                resourceName = name + ".spv";
                break;
            }
            case GraphicsBackend.Metal:
            {
                resourceName = name + ".metallib";
                break;
            }
            default:
                throw new NotImplementedException();
        }
        
        return EmbeddedResourceManager.GetEmbeddedResourceBytes<WindowBackend>(resourceName);
    }

    /// <summary>
    ///     Recreates the Font Texture
    /// </summary>
    /// <remarks>
    ///     Needs to be called <b>after</b> loading/changing Fonts
    /// </remarks>
    public void RecreateFontDeviceTexture()
    {
        ImGuiIOPtr io = ImGui.GetIO();
        // Build
        IntPtr pixels;
        int width, height, bytesPerPixel;
        io.Fonts.GetTexDataAsRGBA32(out pixels, out width, out height, out bytesPerPixel);
        // Store our identifier
        io.Fonts.SetTexID(State.FontAtlasId);

        Context.FontTexture = Context.GraphicsDevice.ResourceFactory.CreateTexture(TextureDescription.Texture2D(
            (uint)width,
            (uint)height,
            1,
            1,
            PixelFormat.R8_G8_B8_A8_UNorm,
            TextureUsage.Sampled));
        Context.FontTexture.Name = "ImGui.NET Font Texture";
        Context.GraphicsDevice.UpdateTexture(
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
        Context.FontTextureView = Context.GraphicsDevice.ResourceFactory.CreateTextureView(Context.FontTexture);

        io.Fonts.ClearTexData();
    }

    /// <summary>
    ///     Submits Draw Commands to the Backend Renderer
    /// </summary>
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
    ///     Renders ImGui Draw Data
    /// </summary>
    private void Render()
    {
        if (!State.FrameBegun) return;
        State.FrameBegun = false;
        ImGui.Render();
        RenderImDrawData(ImGui.GetDrawData());
    }

    /// <summary>
    ///     Updates the Input I/O State with the current Input Snapshot
    /// </summary>
    /// <param name="deltaSeconds">
    ///     Frame Time as Delta-Time
    /// </param>
    /// <param name="snapshot">
    ///     Input Snapshot
    /// </param>
    public void UpdateInput(float deltaSeconds, InputSnapshot snapshot)
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
    ///     Sets the Per-Frame ImGui Data
    /// </summary>
    /// <param name="deltaSeconds"></param>
    private void SetPerFrameImGuiData(float deltaSeconds)
    {
        ImGuiIOPtr io = ImGui.GetIO();
        io.DisplaySize = new Vector2(
            State.WindowSize.X / State.ScaleFactor.X,
            State.WindowSize.Y / State.ScaleFactor.Y);
        io.DisplayFramebufferScale = State.ScaleFactor;
        io.DeltaTime = deltaSeconds; // DeltaTime is in seconds.
    }

    /// <summary>
    ///     Attempts to map SDL2/Veldrid Keys to ImGui Keys
    /// </summary>
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

    /// <summary>
    ///     Updates ImGui Input Data
    /// </summary>
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
        foreach (var t in snapshot.KeyCharPresses)
        {
            io.AddInputCharacter(t);
        }

        foreach (var keyEvent in snapshot.KeyEvents)
        {
            if (TryMapKey(keyEvent.Key, out ImGuiKey imguikey))
            {
                io.AddKeyEvent(imguikey, keyEvent.Down);
            }
        }
    }

    /// <summary>
    ///     Renders the current ImGui Draw Data
    /// </summary>
    /// <param name="drawData">
    ///     ImGui Draw Data
    /// </param>
    private void RenderImDrawData(ImDrawDataPtr drawData)
    {
        uint vertexOffsetInVertices = 0;
        uint indexOffsetInElements = 0;

        if (drawData.CmdListsCount == 0)
        {
            return;
        }

        uint totalVBSize = (uint)(drawData.TotalVtxCount * Unsafe.SizeOf<ImDrawVert>());
        if (totalVBSize > Context.VertexBuffer.SizeInBytes)
        {
            Context.GraphicsDevice.DisposeWhenIdle(Context.VertexBuffer);
            Context.VertexBuffer = Context.GraphicsDevice.ResourceFactory.CreateBuffer(new BufferDescription((uint)(totalVBSize * 1.5f), BufferUsage.VertexBuffer | BufferUsage.Dynamic));
        }

        uint totalIBSize = (uint)(drawData.TotalIdxCount * sizeof(ushort));
        if (totalIBSize > Context.IndexBuffer.SizeInBytes)
        {
            Context.GraphicsDevice.DisposeWhenIdle(Context.IndexBuffer);
            Context.IndexBuffer = Context.GraphicsDevice.ResourceFactory.CreateBuffer(new BufferDescription((uint)(totalIBSize * 1.5f), BufferUsage.IndexBuffer | BufferUsage.Dynamic));
        }

        for (int i = 0; i < drawData.CmdListsCount; i++)
        {
            ImDrawListPtr cmd_list = drawData.CmdLists[i];

            Context.CommandList.UpdateBuffer(
                Context.VertexBuffer,
                vertexOffsetInVertices * (uint)Unsafe.SizeOf<ImDrawVert>(),
                cmd_list.VtxBuffer.Data,
                (uint)(cmd_list.VtxBuffer.Size * Unsafe.SizeOf<ImDrawVert>()));

            Context.CommandList.UpdateBuffer(
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

        Context.CommandList.SetVertexBuffer(0, Context.VertexBuffer);
        Context.CommandList.SetIndexBuffer(Context.IndexBuffer, IndexFormat.UInt16);
        Context.CommandList.SetPipeline(Context.Pipeline);
        Context.CommandList.SetGraphicsResourceSet(0, Context.MainResourceSet);

        drawData.ScaleClipRects(io.DisplayFramebufferScale);

        // Render command lists
        int vtx_offset = 0;
        int idx_offset = 0;
        for (int n = 0; n < drawData.CmdListsCount; n++)
        {
            ImDrawListPtr cmd_list = drawData.CmdLists[n];
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
                            Context.CommandList.SetGraphicsResourceSet(1, Context.FontTextureResourceSet);
                        }
                        else
                        {
                            Context.CommandList.SetGraphicsResourceSet(1, GetImageResourceSet(pcmd.TextureId));
                        }
                    }

                    Context.CommandList.SetScissorRect(
                        0,
                        (uint)pcmd.ClipRect.X,
                        (uint)pcmd.ClipRect.Y,
                        (uint)(pcmd.ClipRect.Z - pcmd.ClipRect.X),
                        (uint)(pcmd.ClipRect.W - pcmd.ClipRect.Y));

                    Context.CommandList.DrawIndexed(pcmd.ElemCount, 1, pcmd.IdxOffset + (uint)idx_offset, (int)pcmd.VtxOffset + vtx_offset, 0);
                }
            }
            vtx_offset += cmd_list.VtxBuffer.Size;
            idx_offset += cmd_list.IdxBuffer.Size;
        }
    }

    public void Dispose() => Context.Dispose();
}
using System.Diagnostics;
using System.Numerics;
using System.Runtime.CompilerServices;
using ImGuiNET;
using ImGuiWS.Components;
using ImGuiWS.Logging;
using ImGuiWS.Utils;
using Serilog;
using Veldrid;
using Veldrid.Sdl2;
using Veldrid.StartupUtilities;

namespace ImGuiWS.Renderer;

/// <summary>
///     Main Rendering Backend based on <see cref="Veldrid" />
/// </summary>
public class WindowBackend : IDisposable {
    internal readonly ILogger logger = LoggerFactory.Create<WindowBackend>();

    /// <summary>
    ///     Main Constructor
    ///     Initializes Rendering and ImGui Context
    /// </summary>
    /// <param name="windowCreateInfo">
    ///     Setup Information about the SDL2/Veldrid Window
    /// </param>
    /// <param name="mainWindowHandle">
    ///     Handle to the <see cref="MainWindow" /> associated with this Backend
    /// </param>
    public WindowBackend(WindowSetupOptions setupOptions, MainWindow mainWindow) {
        this.logger.Debug("Initializing Backend for Window {windowName}", setupOptions.Title);
        this.MainWindow = mainWindow;

        this.Context = new WindowBackendContext(this);

        this.Context.Window = new Sdl2Window(
            setupOptions.Title,
            (Int32)setupOptions.Position.X,
            (Int32)setupOptions.Position.Y,
            (Int32)setupOptions.Size.X,
            (Int32)setupOptions.Size.Y,
            setupOptions.Flags,
            false);

        this.logger.Debug("Created SDL2 Window");

        Boolean debug = Debugger.IsAttached;
        GraphicsDeviceOptions gdOptions = new(
            debug,
            null,
            true,
            ResourceBindingModel.Improved,
            preferStandardClipSpaceYDirection: true,
            preferDepthRangeZeroToOne: true);

        this.Context.GraphicsDevice =
            VeldridStartup.CreateGraphicsDevice(this.Context.Window, gdOptions, setupOptions.Backend);
        this.logger.Debug("Graphics Device initialized (Debug: {debugMode}, GraphicsBackend: {backend})", debug,
            setupOptions.Backend);

        this.Context.Window.Resized += () => {
            this.Context.GraphicsDevice.MainSwapchain.Resize(
                (UInt32)this.Context.Window.Width,
                (UInt32)this.Context.Window.Height);

            this.State.WindowSize = this.State.WindowSize with {
                X = this.Context.Window.Width,
                Y = this.Context.Window.Height
            };
        };

        this.Context.CommandList = this.Context.GraphicsDevice.ResourceFactory.CreateCommandList();

        this.State.WindowSize = this.State.WindowSize with { X = setupOptions.Size.X, Y = setupOptions.Size.Y };
        ImGui.CreateContext();
        ImGuiIOPtr io = ImGui.GetIO();
        io.BackendFlags |= ImGuiBackendFlags.RendererHasVtxOffset;
        io.ConfigFlags |= ImGuiConfigFlags.NavEnableKeyboard |
                          ImGuiConfigFlags.DockingEnable;
        io.Fonts.Flags &= ~ImFontAtlasFlags.NoBakedLines;

        Vector2 actualScreen = this.GetScreenSize();
        if(actualScreen == Vector2.Zero) {
            throw new InvalidOperationException("Failed to get Screen Size! Vector was zero");
        }

        this.DpiScale = actualScreen.X / 1920f;

        this.logger.Debug("ImGui Context initialized");
        this.logger.Information("Window Backend initialization done");
        this.logger.Debug("DPI Scale Factor: {scaleFactor}", this.DpiScale);
    }

    /// <inheritdoc cref="WindowBackendContext" />
    public WindowBackendContext Context { get; internal set; }

    /// <inheritdoc cref="WindowModifierKeyState" />
    public WindowModifierKeyState ModifierKeyState { get; internal set; } = new();

    /// <inheritdoc cref="WindowState" />
    public WindowState State { get; internal set; } = new();

    internal MainWindow MainWindow { get; set; }

    public Single DpiScale { get; }

    /// <summary>
    ///     Returns the next available ImGui Binding ID
    ///     or Updates the current one
    /// </summary>
    public IntPtr NextId {
        get {
            IntPtr nextId = this.State.LastAssignedId++;
            return nextId;
        }
        private set => this.State.LastAssignedId = value;
    }

    public void Dispose() {
        this.Context.Dispose();
    }

    /// <summary>
    ///     Prepares the ImGui Context
    /// </summary>
    /// <remarks>
    ///     Needs to be called <b>after</b> Font/Resource allocation!
    /// </remarks>
    internal void SetupContext() {
        this.CreateDeviceResources();
        this.SetPerFrameImGuiData(1f / 60f);
        ImGui.NewFrame();
        this.State.FrameBegun = true;

        ImGuiStylePtr style = ImGui.GetStyle();
        style.WindowPadding = new Vector2(8);
        style.FramePadding = new Vector2(8);
        style.ItemSpacing = new Vector2(8);
        style.ScrollbarSize = 16;

        style.WindowRounding = 6;
        style.ChildRounding = 6;
        style.FrameRounding = 6;
        style.PopupRounding = 6;
        style.ScrollbarRounding = 6;
        style.GrabRounding = 6;
        style.TabRounding = 6;

        style.WindowBorderSize = 0;

        style.WindowMenuButtonPosition = ImGuiDir.Left;
        style.ColorButtonPosition = ImGuiDir.Right;
        style.WindowTitleAlign = new Vector2(0.5f);

        this.logger.Debug("Prepared ImGui Context");
    }

    /// <summary>
    ///     Allocates Resources like Shaders and Buffers
    /// </summary>
    private void CreateDeviceResources() {
        this.logger.Debug("Creating Device Resources");
        ResourceFactory factory = this.Context.GraphicsDevice.ResourceFactory;
        this.Context.VertexBuffer =
            factory.CreateBuffer(new BufferDescription(10000, BufferUsage.VertexBuffer | BufferUsage.Dynamic));
        this.Context.VertexBuffer.Name = "ImGui.NET Vertex Buffer";
        this.Context.IndexBuffer =
            factory.CreateBuffer(new BufferDescription(2000, BufferUsage.IndexBuffer | BufferUsage.Dynamic));
        this.Context.IndexBuffer.Name = "ImGui.NET Index Buffer";
        this.RecreateFontDeviceTexture();


        this.Context.ProjectionBuffer =
            factory.CreateBuffer(new BufferDescription(64, BufferUsage.UniformBuffer | BufferUsage.Dynamic));
        this.Context.ProjectionBuffer.Name = "ImGui.NET Projection Buffer";

        this.logger.Debug("Created Buffers");

        Byte[] vertexShaderBytes = this.LoadEmbeddedShaderCode("imgui-vertex", ShaderStages.Vertex);
        Byte[] fragmentShaderBytes = this.LoadEmbeddedShaderCode("imgui-frag", ShaderStages.Fragment);
        this.Context.VertexShader = factory.CreateShader(new ShaderDescription(ShaderStages.Vertex,
            vertexShaderBytes, this.Context.GraphicsDevice.BackendType == GraphicsBackend.Metal ? "VS" : "main"));
        this.Context.FragmentShader = factory.CreateShader(new ShaderDescription(ShaderStages.Fragment,
            fragmentShaderBytes, this.Context.GraphicsDevice.BackendType == GraphicsBackend.Metal ? "FS" : "main"));

        VertexLayoutDescription[] vertexLayouts = new[] {
            new VertexLayoutDescription(
                new VertexElementDescription("in_position", VertexElementSemantic.Position,
                    VertexElementFormat.Float2),
                new VertexElementDescription("in_texCoord", VertexElementSemantic.TextureCoordinate,
                    VertexElementFormat.Float2),
                new VertexElementDescription("in_color", VertexElementSemantic.Color,
                    VertexElementFormat.Byte4_Norm))
        };

        this.Context.ResLayout = factory.CreateResourceLayout(new ResourceLayoutDescription(
            new ResourceLayoutElementDescription("ProjectionMatrixBuffer", ResourceKind.UniformBuffer,
                ShaderStages.Vertex),
            new ResourceLayoutElementDescription("MainSampler", ResourceKind.Sampler, ShaderStages.Fragment)));
        this.Context.TexLayout = factory.CreateResourceLayout(new ResourceLayoutDescription(
            new ResourceLayoutElementDescription("MainTexture", ResourceKind.TextureReadOnly,
                ShaderStages.Fragment)));

        this.logger.Debug("Created Shaders and Texture Layout");

        GraphicsPipelineDescription pd = new(
            BlendStateDescription.SingleAlphaBlend,
            new DepthStencilStateDescription(false, false, ComparisonKind.Always),
            new RasterizerStateDescription(FaceCullMode.None, PolygonFillMode.Solid, FrontFace.Clockwise, false,
                true),
            PrimitiveTopology.TriangleList,
            new ShaderSetDescription(vertexLayouts,
                new[] { this.Context.VertexShader, this.Context.FragmentShader }),
            new[] { this.Context.ResLayout, this.Context.TexLayout },
            this.Context.GraphicsDevice.MainSwapchain.Framebuffer.OutputDescription,
            ResourceBindingModel.Default);
        this.Context.Pipeline = factory.CreateGraphicsPipeline(ref pd);

        this.logger.Debug("Created Graphics Pipeline");

        this.Context.MainResourceSet = factory.CreateResourceSet(new ResourceSetDescription(this.Context.ResLayout,
            this.Context.ProjectionBuffer, this.Context.GraphicsDevice.PointSampler));

        this.Context.FontTextureResourceSet =
            factory.CreateResourceSet(new ResourceSetDescription(this.Context.TexLayout,
                this.Context.FontTexture.View));

        this.logger.Debug("Created Font Texture");
        this.logger.Information("Device Resource initialization done");
    }

    public IntPtr CreateImGuiBinding(ref Texture texture) {
        ResourceSetDescription rsd = new(this.Context.TexLayout, texture.View);
        texture.ResourceSet = this.Context.GraphicsDevice.ResourceFactory.CreateResourceSet(rsd);
        texture.Id = this.NextId;
        this.NextId++;
        return(IntPtr)texture.Id;
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
    public ResourceSet GetImageResourceSet(IntPtr imGuiBinding) {
        Texture? texture = this.Context.Textures.GetByBindingId(imGuiBinding);
        if(texture == null) {
            this.logger.Fatal("Found no Resource Set for ImGui Binding Id {bindingId}", imGuiBinding);
            throw new InvalidOperationException("No registered ImGui binding with id " + imGuiBinding);
        }

        this.logger.Verbose("Got Image Resource Set for ImGui Binding Id {bindingId}", imGuiBinding);
        return texture.ResourceSet;
    }

    /// <summary>
    ///     Loads the embedded shader code for the specified name and shader stage.
    ///     The backend is determined using <see cref="Veldrid.GraphicsDevice.BackendType" />
    ///     via <c>WindowBackend.Context.GraphicsDevice.ResourceFactory.BackendType</c>.
    /// </summary>
    /// <param name="name">The name of the embedded shader.</param>
    /// <param name="stage">The shader stage (e.g., Vertex, Fragment).</param>
    /// <returns>The source code of the shader as a string.</returns>
    /// <exception cref="System.NotImplementedException">
    ///     Thrown if the method is not implemented for the specified backend.
    /// </exception>
    private Byte[] LoadEmbeddedShaderCode(String name, ShaderStages stage) {
        String resourceName;
        switch(this.Context.GraphicsDevice.ResourceFactory.BackendType) {
            case GraphicsBackend.Direct3D11: {
                resourceName = name + ".hlsl.bytes";
                break;
            }
            case GraphicsBackend.OpenGL: {
                resourceName = name + ".glsl";
                break;
            }
            case GraphicsBackend.Vulkan: {
                resourceName = name + ".spv";
                break;
            }
            case GraphicsBackend.Metal: {
                resourceName = name + ".metallib";
                break;
            }
            default:
                this.logger.Error(
                    "Failed to load Shader Code for {name} (Stage: {stage}): Invalid Backend {graphicsBackend}",
                    name,
                    stage, this.Context.GraphicsDevice.ResourceFactory.BackendType);
                throw new NotImplementedException();
        }

        this.logger.Debug("Loaded Shader Code for {name} (Stage: {stage})", name, stage);
        return EmbeddedResourceManager.GetEmbeddedResourceBytes<WindowBackend>(resourceName);
    }

    /// <summary>
    ///     Recreates the Font Texture
    /// </summary>
    /// <remarks>
    ///     Needs to be called <b>after</b> loading/changing Fonts
    /// </remarks>
    public void RecreateFontDeviceTexture() {
        ImGuiIOPtr io = ImGui.GetIO();
        // Build
        IntPtr pixels;
        Int32 width, height, bytesPerPixel;
        io.Fonts.GetTexDataAsRGBA32(out pixels, out width, out height, out bytesPerPixel);
        // Store our identifier
        io.Fonts.SetTexID(this.State.FontAtlasId);

        if(this.Context.FontTexture == null) {
            throw new Exception("Font Texture is null!");
        }

        this.Context.FontTexture.Tex = this.Context.GraphicsDevice.ResourceFactory.CreateTexture(
            TextureDescription.Texture2D(
                (UInt32)width,
                (UInt32)height,
                1,
                1,
                PixelFormat.R8_G8_B8_A8_UNorm,
                TextureUsage.Sampled));
        this.Context.FontTexture.Tex.Name = "ImGui.NET Font Texture";
        this.Context.FontTexture.Name = "ImGui.NET Font Texture";
        this.Context.GraphicsDevice.UpdateTexture(this.Context.FontTexture.Tex,
            pixels,
            (UInt32)(bytesPerPixel * width * height),
            0,
            0,
            0,
            (UInt32)width,
            (UInt32)height,
            1,
            0,
            0);

        this.Context.FontTexture.View =
            this.Context.GraphicsDevice.ResourceFactory.CreateTextureView(this.Context.FontTexture.Tex);

        io.Fonts.ClearTexData();
        this.logger.Information("Created/Recreated Font Device Texture");
    }

    public void BeginRender() {
        this.Context.CommandList.Begin();
        this.Context.CommandList.SetFramebuffer(this.Context.GraphicsDevice.MainSwapchain.Framebuffer);
        this.Context.CommandList.SetPipeline(this.Context.Pipeline);
        this.Context.CommandList.ClearColorTarget(0,
            new RgbaFloat(this.State.ClearColor.X, this.State.ClearColor.Y, this.State.ClearColor.Z,
                this.State.ClearColor.W));
    }

    public void EndRender() {
        this.Context.CommandList.End();
        this.Context.GraphicsDevice.SubmitCommands(this.Context.CommandList);
        this.Context.GraphicsDevice.SwapBuffers(this.Context.GraphicsDevice.MainSwapchain);
        this.logger.Verbose("Submitted Command List to Renderer");
    }

    /// <summary>
    ///     Renders ImGui Draw Data
    /// </summary>
    public void Render() {
        if(!this.State.FrameBegun) {
            return;
        }

        this.State.FrameBegun = false;
        // ImGui.SetWindowFontScale(DpiScale);
        ImGui.Render();
        this.RenderImDrawData(ImGui.GetDrawData());
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
    public void UpdateInput(Single deltaSeconds, InputSnapshot snapshot) {
        this.State.RenderingBegun = true;
        if(this.State.FrameBegun) {
            ImGui.Render();
        }

        this.SetPerFrameImGuiData(deltaSeconds);
        this.UpdateImGuiInput(snapshot);

        this.State.FrameBegun = true;
        ImGui.NewFrame();
    }

    /// <summary>
    ///     Sets the Per-Frame ImGui Data
    /// </summary>
    /// <param name="deltaSeconds"></param>
    private void SetPerFrameImGuiData(Single deltaSeconds) {
        ImGuiIOPtr io = ImGui.GetIO();
        io.DisplaySize = new Vector2(this.State.WindowSize.X / this.State.ScaleFactor.X,
            this.State.WindowSize.Y / this.State.ScaleFactor.Y);
        io.DisplayFramebufferScale = this.State.ScaleFactor;
        io.DeltaTime = deltaSeconds; // DeltaTime is in seconds.
    }

    /// <summary>
    ///     Attempts to map SDL2/Veldrid Keys to ImGui Keys
    /// </summary>
    private Boolean TryMapKey(Key key, out ImGuiKey result) {
        ImGuiKey KeyToImGuiKeyShortcut(Key keyToConvert, Key startKey1, ImGuiKey startKey2) {
            Int32 changeFromStart1 = (Int32)keyToConvert - (Int32)startKey1;
            return startKey2 + changeFromStart1;
        }

        result = key switch {
            >= Key.F1 and<= Key.F24 => KeyToImGuiKeyShortcut(key, Key.F1, ImGuiKey.F1),
            >= Key.Keypad0 and<= Key.Keypad9 => KeyToImGuiKeyShortcut(key, Key.Keypad0, ImGuiKey.Keypad0),
            >= Key.A and<= Key.Z => KeyToImGuiKeyShortcut(key, Key.A, ImGuiKey.A),
            >= Key.Number0 and<= Key.Number9 => KeyToImGuiKeyShortcut(key, Key.Number0, ImGuiKey._0),
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
    private void UpdateImGuiInput(InputSnapshot snapshot) {
        ImGuiIOPtr io = ImGui.GetIO();
        io.AddMousePosEvent(snapshot.MousePosition.X, snapshot.MousePosition.Y);
        io.AddMouseButtonEvent(0, snapshot.IsMouseDown(MouseButton.Left));
        io.AddMouseButtonEvent(1, snapshot.IsMouseDown(MouseButton.Right));
        io.AddMouseButtonEvent(2, snapshot.IsMouseDown(MouseButton.Middle));
        io.AddMouseButtonEvent(3, snapshot.IsMouseDown(MouseButton.Button1));
        io.AddMouseButtonEvent(4, snapshot.IsMouseDown(MouseButton.Button2));
        io.AddMouseWheelEvent(0f, snapshot.WheelDelta);
        foreach(Char t in snapshot.KeyCharPresses) {
            io.AddInputCharacter(t);
        }

        foreach(KeyEvent keyEvent in snapshot.KeyEvents) {
            if(this.TryMapKey(keyEvent.Key, out ImGuiKey imguikey)) {
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
    private void RenderImDrawData(ImDrawDataPtr drawData) {
        UInt32 vertexOffsetInVertices = 0;
        UInt32 indexOffsetInElements = 0;

        if(drawData.CmdListsCount == 0) {
            return;
        }

        UInt32 totalVBSize = (UInt32)(drawData.TotalVtxCount * Unsafe.SizeOf<ImDrawVert>());
        if(totalVBSize > this.Context.VertexBuffer.SizeInBytes) {
            this.Context.GraphicsDevice.DisposeWhenIdle(this.Context.VertexBuffer);
            this.Context.VertexBuffer = this.Context.GraphicsDevice.ResourceFactory.CreateBuffer(
                new BufferDescription((UInt32)(totalVBSize * 1.5f),
                    BufferUsage.VertexBuffer | BufferUsage.Dynamic));
        }

        UInt32 totalIBSize = (UInt32)(drawData.TotalIdxCount * sizeof(UInt16));
        if(totalIBSize > this.Context.IndexBuffer.SizeInBytes) {
            this.Context.GraphicsDevice.DisposeWhenIdle(this.Context.IndexBuffer);
            this.Context.IndexBuffer = this.Context.GraphicsDevice.ResourceFactory.CreateBuffer(
                new BufferDescription((UInt32)(totalIBSize * 1.5f),
                    BufferUsage.IndexBuffer | BufferUsage.Dynamic));
        }

        for(Int32 i = 0; i < drawData.CmdListsCount; i++) {
            ImDrawListPtr cmd_list = drawData.CmdLists[i];

            this.Context.CommandList.UpdateBuffer(this.Context.VertexBuffer,
                vertexOffsetInVertices * (UInt32)Unsafe.SizeOf<ImDrawVert>(),
                cmd_list.VtxBuffer.Data,
                (UInt32)(cmd_list.VtxBuffer.Size * Unsafe.SizeOf<ImDrawVert>()));

            this.Context.CommandList.UpdateBuffer(this.Context.IndexBuffer,
                indexOffsetInElements * sizeof(UInt16),
                cmd_list.IdxBuffer.Data,
                (UInt32)(cmd_list.IdxBuffer.Size * sizeof(UInt16)));

            vertexOffsetInVertices += (UInt32)cmd_list.VtxBuffer.Size;
            indexOffsetInElements += (UInt32)cmd_list.IdxBuffer.Size;
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

        this.Context.GraphicsDevice.UpdateBuffer(this.Context.ProjectionBuffer, 0, ref mvp);

        this.Context.CommandList.SetVertexBuffer(0, this.Context.VertexBuffer);
        this.Context.CommandList.SetIndexBuffer(this.Context.IndexBuffer, IndexFormat.UInt16);
        this.Context.CommandList.SetPipeline(this.Context.Pipeline);
        this.Context.CommandList.SetGraphicsResourceSet(0, this.Context.MainResourceSet);

        drawData.ScaleClipRects(io.DisplayFramebufferScale);

        // Render command lists
        Int32 vtx_offset = 0;
        Int32 idx_offset = 0;
        for(Int32 n = 0; n < drawData.CmdListsCount; n++) {
            ImDrawListPtr cmd_list = drawData.CmdLists[n];
            for(Int32 cmd_i = 0; cmd_i < cmd_list.CmdBuffer.Size; cmd_i++) {
                ImDrawCmdPtr pcmd = cmd_list.CmdBuffer[cmd_i];
                if(pcmd.UserCallback != IntPtr.Zero) {
                    throw new NotImplementedException();
                }

                if(pcmd.TextureId != IntPtr.Zero) {
                    if(pcmd.TextureId == this.State.FontAtlasId) {
                        this.Context.CommandList.SetGraphicsResourceSet(1, this.Context.FontTextureResourceSet);
                    }
                    else {
                        this.Context.CommandList.SetGraphicsResourceSet(1,
                            this.GetImageResourceSet(pcmd.TextureId));
                    }
                }

                this.Context.CommandList.SetScissorRect(
                    0,
                    (UInt32)pcmd.ClipRect.X,
                    (UInt32)pcmd.ClipRect.Y,
                    (UInt32)(pcmd.ClipRect.Z - pcmd.ClipRect.X),
                    (UInt32)(pcmd.ClipRect.W - pcmd.ClipRect.Y));

                this.Context.CommandList.DrawIndexed(pcmd.ElemCount, 1, pcmd.IdxOffset + (UInt32)idx_offset,
                    (Int32)pcmd.VtxOffset + vtx_offset, 0);
            }

            vtx_offset += cmd_list.VtxBuffer.Size;
            idx_offset += cmd_list.IdxBuffer.Size;
        }
    }
}
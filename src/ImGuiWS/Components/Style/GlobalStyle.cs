using System.Numerics;
using System.Reflection;
using ImGuiNET;

namespace ImGuiWS.Components.Style;

/// <summary>
///     Provides the global Default Styling
/// </summary>
public static class GlobalStyle
{
    #region Styles

    public static float Alpha { get; internal set; } = 1.0f;
    public static float DisabledAlpha { get; internal set; } = 0.6f;
    public static Vector2 WindowPadding { get; internal set; } = new Vector2(8.0f, 8.0f);
    public static float WindowRounding { get; internal set; } = 7.0f;
    public static float WindowBorderSize { get; internal set; } = 1.0f;
    public static Vector2 WindowMinSize { get; internal set; } = new Vector2(32.0f, 32.0f);
    public static Vector2 WindowTitleAlign { get; internal set; } = new Vector2(0.0f, 0.5f);
    public static ImGuiDir WindowMenuButtonPosition { get; internal set; } = ImGuiDir.Left;
    public static float ChildRounding { get; internal set; } = 4.0f;
    public static float ChildBorderSize { get; internal set; } = 1.0f;
    public static float PopupRounding { get; internal set; } = 4.0f;
    public static float PopupBorderSize { get; internal set; } = 1.0f;
    public static Vector2 FramePadding { get; internal set; } = new Vector2(4.0f, 3.0f);
    public static float FrameRounding { get; internal set; } = 3.0f;
    public static float FrameBorderSize { get; internal set; } = 0.0f;
    public static Vector2 ItemSpacing { get; internal set; } = new Vector2(8.0f, 4.0f);
    public static Vector2 ItemInnerSpacing { get; internal set; } = new Vector2(4.0f, 4.0f);
    public static Vector2 CellPadding { get; internal set; } = new Vector2(4.0f, 2.0f);
    public static Vector2 TouchExtraPadding { get; internal set; } = new Vector2(0.0f, 0.0f);
    public static float IndentSpacing { get; internal set; } = 21.0f;
    public static float ColumnsMinSpacing { get; internal set; } = 6.0f;
    public static float ScrollbarSize { get; internal set; } = 14.0f;
    public static float ScrollbarRounding { get; internal set; } = 9.0f;
    public static float GrabMinSize { get; internal set; } = 10.0f;
    public static float GrabRounding { get; internal set; } = 3.0f;
    public static float LogSliderDeadzone { get; internal set; } = 4.0f;
    public static float TabRounding { get; internal set; } = 4.0f;
    public static float TabBorderSize { get; internal set; } = 0.0f;
    public static float TabMinWidthForCloseButton { get; internal set; } = 0.0f;
    public static ImGuiDir ColorButtonPosition { get; internal set; } = ImGuiDir.Right;
    public static Vector2 ButtonTextAlign { get; internal set; } = new Vector2(0.5f, 0.5f);
    public static Vector2 SelectableTextAlign { get; internal set; } = new Vector2(0.0f, 0.0f);
    public static Vector2 DisplayWindowPadding { get; internal set; } = new Vector2(19.0f, 19.0f);
    public static Vector2 DisplaySafeAreaPadding { get; internal set; } = new Vector2(3.0f, 3.0f);
    public static float MouseCursorScale { get; internal set; } = 1.0f;
    public static bool AntiAliasedLines { get; internal set; } = true;
    public static bool AntiAliasedLinesUseTex { get; internal set; } = true;
    public static bool AntiAliasedFill { get; internal set; } = true;
    public static float CurveTessellationTol { get; internal set; } = 1.25f;
    public static float CircleTessellationMaxError { get; internal set; } = 0.30f;

    #endregion

    #region Colors

    public static Vector4 Text { get; internal set; } = new Vector4(1.00f, 1.00f, 1.00f, 1.00f);
    public static Vector4 TextDisabled { get; internal set; } = new Vector4(0.50f, 0.50f, 0.50f, 1.00f);
    public static Vector4 WindowBg { get; internal set; } = new Vector4(0.06f, 0.06f, 0.06f, 0.94f);
    public static Vector4 ChildBg { get; internal set; } = new Vector4(0.00f, 0.00f, 0.00f, 0.00f);
    public static Vector4 PopupBg { get; internal set; } = new Vector4(0.08f, 0.08f, 0.08f, 0.94f);
    public static Vector4 Border { get; internal set; } = new Vector4(0.43f, 0.43f, 0.50f, 0.50f);
    public static Vector4 BorderShadow { get; internal set; } = new Vector4(0.00f, 0.00f, 0.00f, 0.00f);
    public static Vector4 FrameBg { get; internal set; } = new Vector4(0.16f, 0.29f, 0.48f, 0.54f);
    public static Vector4 FrameBgHovered { get; internal set; } = new Vector4(0.26f, 0.59f, 0.98f, 0.40f);
    public static Vector4 FrameBgActive { get; internal set; } = new Vector4(0.26f, 0.59f, 0.98f, 0.67f);
    public static Vector4 TitleBg { get; internal set; } = new Vector4(0.04f, 0.04f, 0.04f, 1.00f);
    public static Vector4 TitleBgActive { get; internal set; } = new Vector4(0.16f, 0.29f, 0.48f, 1.00f);
    public static Vector4 TitleBgCollapsed { get; internal set; } = new Vector4(0.00f, 0.00f, 0.00f, 0.51f);
    public static Vector4 MenuBarBg { get; internal set; } = new Vector4(0.14f, 0.14f, 0.14f, 1.00f);
    public static Vector4 ScrollbarBg { get; internal set; } = new Vector4(0.02f, 0.02f, 0.02f, 0.53f);
    public static Vector4 ScrollbarGrab { get; internal set; } = new Vector4(0.31f, 0.31f, 0.31f, 1.00f);
    public static Vector4 ScrollbarGrabHovered { get; internal set; } = new Vector4(0.41f, 0.41f, 0.41f, 1.00f);
    public static Vector4 ScrollbarGrabActive { get; internal set; } = new Vector4(0.51f, 0.51f, 0.51f, 1.00f);
    public static Vector4 CheckMark { get; internal set; } = new Vector4(0.26f, 0.59f, 0.98f, 1.00f);
    public static Vector4 SliderGrab { get; internal set; } = new Vector4(0.24f, 0.52f, 0.88f, 1.00f);
    public static Vector4 SliderGrabActive { get; internal set; } = new Vector4(0.26f, 0.59f, 0.98f, 1.00f);
    public static Vector4 Button { get; internal set; } = new Vector4(0.26f, 0.59f, 0.98f, 0.40f);
    public static Vector4 ButtonHovered { get; internal set; } = new Vector4(0.26f, 0.59f, 0.98f, 1.00f);
    public static Vector4 ButtonActive { get; internal set; } = new Vector4(0.06f, 0.53f, 0.98f, 1.00f);
    public static Vector4 Header { get; internal set; } = new Vector4(0.26f, 0.59f, 0.98f, 0.31f);
    public static Vector4 HeaderHovered { get; internal set; } = new Vector4(0.26f, 0.59f, 0.98f, 0.80f);
    public static Vector4 HeaderActive { get; internal set; } = new Vector4(0.26f, 0.59f, 0.98f, 1.00f);
    public static Vector4 Separator { get; internal set; } = new Vector4(0.43f, 0.43f, 0.50f, 0.50f);
    public static Vector4 SeparatorHovered { get; internal set; } = new Vector4(0.10f, 0.40f, 0.75f, 0.78f);
    public static Vector4 SeparatorActive { get; internal set; } = new Vector4(0.10f, 0.40f, 0.75f, 1.00f);
    public static Vector4 ResizeGrip { get; internal set; } = new Vector4(0.26f, 0.59f, 0.98f, 0.25f);
    public static Vector4 ResizeGripHovered { get; internal set; } = new Vector4(0.26f, 0.59f, 0.98f, 0.67f);
    public static Vector4 ResizeGripActive { get; internal set; } = new Vector4(0.26f, 0.59f, 0.98f, 0.95f);
    public static Vector4 Tab { get; internal set; } = new Vector4(0.18f, 0.35f, 0.58f, 0.86f);
    public static Vector4 TabHovered { get; internal set; } = new Vector4(0.26f, 0.59f, 0.98f, 0.80f);
    public static Vector4 TabActive { get; internal set; } = new Vector4(0.20f, 0.41f, 0.68f, 1.00f);
    public static Vector4 TabUnfocused { get; internal set; } = new Vector4(0.07f, 0.10f, 0.15f, 0.97f);
    public static Vector4 TabUnfocusedActive { get; internal set; } = new Vector4(0.14f, 0.26f, 0.42f, 1.00f);
    public static Vector4 DockingPreview { get; internal set; } = new Vector4(0.26f, 0.59f, 0.98f, 0.70f);
    public static Vector4 DockingEmptyBg { get; internal set; } = new Vector4(0.20f, 0.20f, 0.20f, 1.00f);
    public static Vector4 PlotLines { get; internal set; } = new Vector4(0.61f, 0.61f, 0.61f, 1.00f);
    public static Vector4 PlotLinesHovered { get; internal set; } = new Vector4(1.00f, 0.43f, 0.35f, 1.00f);
    public static Vector4 PlotHistogram { get; internal set; } = new Vector4(0.90f, 0.70f, 0.00f, 1.00f);
    public static Vector4 PlotHistogramHovered { get; internal set; } = new Vector4(1.00f, 0.60f, 0.00f, 1.00f);
    public static Vector4 TableHeaderBg { get; internal set; } = new Vector4(0.19f, 0.19f, 0.20f, 1.00f);
    public static Vector4 TableBorderStrong { get; internal set; } = new Vector4(0.31f, 0.31f, 0.35f, 1.00f);
    public static Vector4 TableBorderLight { get; internal set; } = new Vector4(0.23f, 0.23f, 0.25f, 1.00f);
    public static Vector4 TableRowBg { get; internal set; } = new Vector4(0.00f, 0.00f, 0.00f, 0.00f);
    public static Vector4 TableRowBgAlt { get; internal set; } = new Vector4(1.00f, 1.00f, 1.00f, 0.06f);
    public static Vector4 TextSelectedBg { get; internal set; } = new Vector4(0.26f, 0.59f, 0.98f, 0.35f);
    public static Vector4 DragDropTarget { get; internal set; } = new Vector4(1.00f, 1.00f, 0.00f, 0.90f);
    public static Vector4 NavHighlight { get; internal set; } = new Vector4(0.26f, 0.59f, 0.98f, 1.00f);
    public static Vector4 NavWindowingHighlight { get; internal set; } = new Vector4(1.00f, 1.00f, 1.00f, 0.70f);
    public static Vector4 NavWindowingDimBg { get; internal set; } = new Vector4(0.80f, 0.80f, 0.80f, 0.20f);
    public static Vector4 ModalWindowDimBg { get; internal set; } = new Vector4(0.80f, 0.80f, 0.80f, 0.35f);

    #endregion
}
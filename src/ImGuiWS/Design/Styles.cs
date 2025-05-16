using System.Numerics;
using ImGuiNET;

namespace ImGuiWS.Design;

public class Styles
{
           public float Alpha = 1.0f;
        public float DisabledAlpha = 0.6f;
        public Vector2 WindowPadding = new Vector2(8.0f, 8.0f);
        public float WindowRounding = 7.0f;
        public float WindowBorderSize = 1.0f;
        public Vector2 WindowMinSize = new Vector2(32.0f, 32.0f);
        public Vector2 WindowTitleAlign = new Vector2(0.0f, 0.5f);
        public ImGuiDir WindowMenuButtonPosition = ImGuiDir.Left;
        public float ChildRounding = 4.0f;
        public float ChildBorderSize = 1.0f;
        public float PopupRounding = 4.0f;
        public float PopupBorderSize = 1.0f;
        public Vector2 FramePadding = new Vector2(4.0f, 3.0f);
        public float FrameRounding = 3.0f;
        public float FrameBorderSize = 0.0f;
        public Vector2 ItemSpacing = new Vector2(8.0f, 4.0f);
        public Vector2 ItemInnerSpacing = new Vector2(4.0f, 4.0f);
        public Vector2 CellPadding = new Vector2(4.0f, 2.0f);
        public Vector2 TouchExtraPadding = new Vector2(0.0f, 0.0f);
        public float IndentSpacing = 21.0f;
        public float ColumnsMinSpacing = 6.0f;
        public float ScrollbarSize = 14.0f;
        public float ScrollbarRounding = 9.0f;
        public float GrabMinSize = 10.0f;
        public float GrabRounding = 3.0f;
        public float LogSliderDeadzone = 4.0f;
        public float TabRounding = 4.0f;
        public float TabBorderSize = 0.0f;
        public float TabMinWidthForCloseButton = 0.0f;
        public ImGuiDir ColorButtonPosition = ImGuiDir.Right;
        public Vector2 ButtonTextAlign = new Vector2(0.5f, 0.5f);
        public Vector2 SelectableTextAlign = new Vector2(0.0f, 0.0f);
        public Vector2 DisplayWindowPadding = new Vector2(19.0f, 19.0f);
        public Vector2 DisplaySafeAreaPadding = new Vector2(3.0f, 3.0f);
        public float MouseCursorScale = 1.0f;
        public bool AntiAliasedLines = true;
        public bool AntiAliasedLinesUseTex = true;
        public bool AntiAliasedFill = true;
        public float CurveTessellationTol = 1.25f;
        public float CircleTessellationMaxError = 0.30f;

        public void Apply()
        {
            var style = ImGui.GetStyle();
            style.Alpha = Alpha;
            style.DisabledAlpha = DisabledAlpha;
            style.WindowPadding = WindowPadding;
            style.WindowRounding = WindowRounding;
            style.WindowBorderSize = WindowBorderSize;
            style.WindowMinSize = WindowMinSize;
            style.WindowTitleAlign = WindowTitleAlign;
            style.WindowMenuButtonPosition = WindowMenuButtonPosition;
            style.ChildRounding = ChildRounding;
            style.ChildBorderSize = ChildBorderSize;
            style.PopupRounding = PopupRounding;
            style.PopupBorderSize = PopupBorderSize;
            style.FramePadding = FramePadding;
            style.FrameRounding = FrameRounding;
            style.FrameBorderSize = FrameBorderSize;
            style.ItemSpacing = ItemSpacing;
            style.ItemInnerSpacing = ItemInnerSpacing;
            style.CellPadding = CellPadding;
            style.TouchExtraPadding = TouchExtraPadding;
            style.IndentSpacing = IndentSpacing;
            style.ColumnsMinSpacing = ColumnsMinSpacing;
            style.ScrollbarSize = ScrollbarSize;
            style.ScrollbarRounding = ScrollbarRounding;
            style.GrabMinSize = GrabMinSize;
            style.GrabRounding = GrabRounding;
            style.LogSliderDeadzone = LogSliderDeadzone;
            style.TabRounding = TabRounding;
            style.TabBorderSize = TabBorderSize;
            style.TabMinWidthForCloseButton = TabMinWidthForCloseButton;
            style.ColorButtonPosition = ColorButtonPosition;
            style.ButtonTextAlign = ButtonTextAlign;
            style.SelectableTextAlign = SelectableTextAlign;
            style.DisplayWindowPadding = DisplayWindowPadding;
            style.DisplaySafeAreaPadding = DisplaySafeAreaPadding;
            style.MouseCursorScale = MouseCursorScale;
            style.AntiAliasedLines = AntiAliasedLines;
            style.AntiAliasedLinesUseTex = AntiAliasedLinesUseTex;
            style.AntiAliasedFill = AntiAliasedFill;
            style.CurveTessellationTol = CurveTessellationTol;
            style.CircleTessellationMaxError = CircleTessellationMaxError;
        } 
}
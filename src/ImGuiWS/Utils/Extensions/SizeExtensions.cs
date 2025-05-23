using System.Numerics;
using SixLabors.ImageSharp;

namespace ImGuiWS.Utils.Extensions;

public static class SizeExtensions {
    public static Vector2 ToVector2(this Size size) {
        return new Vector2(size.Width, size.Height);
    }

    public static Vector2 ToVector2(this System.Drawing.Size size) {
        return new Vector2(size.Width, size.Height);
    }
}
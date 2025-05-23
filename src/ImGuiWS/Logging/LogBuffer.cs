using Serilog.Events;

namespace ImGuiWS.Logging;

/// <summary>
///     Static Log Buffer for Serilog Events
/// </summary>
public static class LogBuffer {
    /// <summary>
    ///     Log Buffer with <see cref="LogEvent" />
    /// </summary>
    public static HashSet<LogEvent> Buffer { get; } = new();

    /// <summary>
    ///     Log Buffer containing the rendered Log Messages
    /// </summary>
    public static HashSet<String> RenderedBuffer { get; } = new();

    /// <summary>
    ///     Adds raw <see cref="LogEvent" /> to <see cref="Buffer" /> and
    ///     rendered Event to a<see cref="RenderedBuffer" />
    /// </summary>
    public static void Add(LogEvent logEvent) {
        Buffer.Add(logEvent);
        RenderedBuffer.Add(logEvent.RenderMessage());
    }
}
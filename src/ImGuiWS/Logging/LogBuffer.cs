using ImGuiWS.Utils.Extensions;
using Serilog.Events;

namespace ImGuiWS.Logging;

public static class LogBuffer
{
    public static HashSet<LogEvent> Buffer { get; private set; } = new();
    public static HashSet<string> RenderedBuffer { get; private set; } = new();
    
    public static void Add(LogEvent logEvent)
    {
        Buffer.Add(logEvent);
        RenderedBuffer.Add(logEvent.RenderMessage());
    }
}
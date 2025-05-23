using Serilog.Core;
using Serilog.Events;

namespace ImGuiWS.Logging;

/// <summary>
///     Sink to write <see cref="LogEvent" /> to <see cref="LogBuffer" />
/// </summary>
public class LogBufferSink : ILogEventSink {
    public void Emit(LogEvent logEvent) {
        LogBuffer.Add(logEvent);
    }
}
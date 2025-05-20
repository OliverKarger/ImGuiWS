using Serilog.Core;
using Serilog.Events;

namespace ImGuiWS.Logging;

public class LogBufferSink : ILogEventSink
{
    public void Emit(LogEvent logEvent)
    {
        LogBuffer.Add(logEvent); 
    }
}
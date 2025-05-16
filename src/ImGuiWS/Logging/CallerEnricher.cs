using System.Diagnostics;
using System.Reflection;
using Serilog.Core;
using Serilog.Events;

namespace ImGuiWS.Logging;

public class CallerEnricher : ILogEventEnricher
{
    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
    {
        // Skip Verbose Level for Performance Optimization
        if (logEvent.Level == LogEventLevel.Verbose) return;
        
        var stack = new StackTrace();

        MethodBase? method = null;
        for (int i = 0; i < stack.FrameCount; i++)
        {
            var frame = stack.GetFrame(i);
            var m = frame?.GetMethod();
            var declaringType = m?.DeclaringType;

            if (declaringType == null)
                continue;

            var ns = declaringType.Namespace;

            // Skip Serilog and System internals
            if (!ns?.StartsWith("Serilog") == true && !ns.StartsWith("System") && !ns.StartsWith("Microsoft"))
            {
                method = m;
                break;
            }
        }

        string className = method?.DeclaringType?.Name ?? "UnknownClass";
        string methodName = method?.Name ?? "UnknownMethod";

        logEvent.AddOrUpdateProperty(propertyFactory.CreateProperty("ClassName", className));
        logEvent.AddOrUpdateProperty(propertyFactory.CreateProperty("MethodName", methodName));
    }
}
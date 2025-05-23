using System.Diagnostics;
using System.Reflection;
using Serilog.Core;
using Serilog.Events;

namespace ImGuiWS.Logging;

/// <summary>
///     Enriches a Serilog <see cref="LogEvent" /> with Caller Method Name, File Name and Class Name
/// </summary>
public class CallerEnricher : ILogEventEnricher {
    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory) {
        // Skip Verbose Level for Performance Optimization
        if(logEvent.Level == LogEventLevel.Verbose) {
            return;
        }

        StackTrace stack = new();

        MethodBase? method = null;
        for(Int32 i = 0; i < stack.FrameCount; i++) {
            StackFrame? frame = stack.GetFrame(i);
            MethodBase? m = frame?.GetMethod();
            Type? declaringType = m?.DeclaringType;

            if(declaringType == null) {
                continue;
            }

            String? ns = declaringType.Namespace;

            // Skip Serilog and System internals
            if(!ns?.StartsWith("Serilog") == true && !ns.StartsWith("System") &&
               !ns.StartsWith("Microsoft")) {
                method = m;
                break;
            }
        }

        String className = method?.DeclaringType?.Name ?? "UnknownClass";
        String methodName = method?.Name ?? "UnknownMethod";

        logEvent.AddOrUpdateProperty(propertyFactory.CreateProperty("ClassName", className));
        logEvent.AddOrUpdateProperty(propertyFactory.CreateProperty("MethodName", methodName));
    }
}
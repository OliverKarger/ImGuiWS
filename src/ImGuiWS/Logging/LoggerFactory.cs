using System.Runtime.InteropServices;
using Serilog;
using Serilog.Enrichers.CallerInfo;

namespace ImGuiWS.Logging;

internal static class LoggerFactory
{
    public const string OutputTemplate =
        "[{Timestamp:HH:mm:ss} {Level:u3}] [{SourceFile}:{Namespace}.{Method}] {Message:lj}{NewLine}{Exception}";
    
    public static ILogger Create<T>() where T : class
    {
        LoggerConfiguration loggerConfiguration = new LoggerConfiguration();
        loggerConfiguration.Enrich.WithCallerInfo(
                includeFileInfo: true,
                assemblyPrefix: "ImGuiWS",
                filePathDepth: 1
            );
        loggerConfiguration.MinimumLevel.Debug();
        loggerConfiguration.WriteTo.Console(outputTemplate: OutputTemplate);
        return loggerConfiguration.CreateLogger().ForContext<T>();
    }
}
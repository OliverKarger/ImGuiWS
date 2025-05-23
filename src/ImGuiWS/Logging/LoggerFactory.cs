using Serilog;
using Serilog.Enrichers.CallerInfo;

namespace ImGuiWS.Logging;

/// <summary>
///     Factory Class for <see cref="ILogger" />
/// </summary>
internal static class LoggerFactory {
    public const String OutputTemplate =
        "[{Timestamp:HH:mm:ss} {Level:u3}] [{SourceFile}:{Namespace}.{Method}] [{SubContext}] {Message:lj}{NewLine}{Exception}";

    /// <summary>
    ///     Creates new <see cref="ILogger" /> for Context of Type <typeparamref name="T" />
    ///     with Optional SubContext <paramref name="subContext" />
    /// </summary>
    /// <param name="subContext">
    ///     Optional Sub Context
    /// </param>
    /// <typeparam name="T">
    ///     Primary Context Type
    /// </typeparam>
    /// <returns>
    ///     Instance of <see cref="ILogger" />
    /// </returns>
    public static ILogger Create<T>(String? subContext = null) where T : class {
        LoggerConfiguration loggerConfiguration = new();
        loggerConfiguration.Enrich.WithCallerInfo(
            true,
            "ImGuiWS",
            filePathDepth: 1
        );
        loggerConfiguration.MinimumLevel.Debug();
        loggerConfiguration.WriteTo.Console(outputTemplate: OutputTemplate);
        loggerConfiguration.WriteTo.Sink<LogBufferSink>();
        return loggerConfiguration.CreateLogger().ForContext("SubContext", subContext ?? String.Empty);
    }
}
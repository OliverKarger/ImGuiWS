namespace ImGuiWS.Exceptions;

/// <summary>
///     Exception for SDL2 Errors
/// </summary>
/// <param name="message">
///     SDL2 Error Message
/// </param>
public class SdlException(string message) : Exception(message) {}
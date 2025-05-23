namespace ImGuiWS.Exceptions;

public class NoDataException(String message, String obj) : Exception($"{obj}: {message}") { }
namespace ImGuiWS.Exceptions;

public class NoDataException(string message, string obj) : Exception($"{obj}: {message}")
{
    
}
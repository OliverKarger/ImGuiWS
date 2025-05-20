namespace ImGuiWS.Exceptions;

/// <summary>
///     Thrown if there is an attempt to create a Component whose ID is already present
///     in the current Context
/// </summary>
/// <param name="id">
///     ID of the Component
/// </param>
public class DuplicateIdException(string id) : Exception($"Component with Id {id} already exists.");
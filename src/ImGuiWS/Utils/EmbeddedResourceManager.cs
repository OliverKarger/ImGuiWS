using System.Reflection;

namespace ImGuiWS.Utils;

public static class EmbeddedResourceManager
{
    /// <summary>
    ///     Loads a Embedded Resource as byte-Array
    /// </summary>
    /// <param name="resourceName">
    ///     Name of the Resource
    /// </param>
    /// <typeparam name="T">
    ///     Assembly of the Type to load from 
    /// </typeparam>
    /// <returns></returns>
    public static byte[] GetEmbeddedResourceBytes<T>(string resourceName)
    {
        Assembly assembly = typeof(T).Assembly;
        Stream? resourceStream = assembly.GetManifestResourceStream(resourceName);
        if (resourceStream == null)
        {
            throw new InvalidOperationException($"Failed to load Resource Stream from Assembly {assembly.FullName}");
        }
        
        byte[] data = new byte[resourceStream.Length];
        resourceStream.ReadExactly(data);
        
        resourceStream.Close();
        return data;
    }
    
}
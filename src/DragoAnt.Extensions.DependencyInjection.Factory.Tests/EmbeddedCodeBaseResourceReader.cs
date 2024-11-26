using System.Reflection;

namespace DragoAnt.Extensions.DependencyInjection.Factory.Tests;

public static class EmbeddedCodeBaseResourceReader
{
    public static string ReadEmbeddedCode(string resourceName)
    {
        var assembly = Assembly.GetExecutingAssembly();
        using var stream = assembly.GetManifestResourceStream($"DragoAnt.Extensions.DependencyInjection.Factory.Tests.EmbeddedCodeBase.{resourceName}");
        if (stream == null)
        {
            throw new InvalidOperationException($"Embedded resource '{resourceName}' not found.");
        }
        
        using var reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }

    public static string ReadEmbeddedExampleModelCode(string modelFileName) 
        => ReadEmbeddedCode($"examples.Models.{modelFileName}.cs");
}
using System.Collections.Immutable;

namespace DragoAnt.Extensions.DependencyInjection;

internal readonly struct FactoryInterfaceModel(
    string name,
    INamedTypeSymbol? interfaceSymbol,
    ImmutableArray<MethodModel> methods,
    bool castParameters = false,
    bool allowNotSupportedMethods = false)
{
    public string Name { get; } = name;
    public ImmutableArray<MethodModel> Methods { get; } = methods;

    public bool CastParameters { get; } = castParameters;

    public bool AllowNotSupportedMethods { get; } = allowNotSupportedMethods;

    public void CollectNamespaces(ISet<string> namespaces)
    {
        interfaceSymbol?.CollectNamespaces(namespaces);
        foreach (var method in Methods)
        {
            method.CollectNamespaces(namespaces);
        }
    }
}
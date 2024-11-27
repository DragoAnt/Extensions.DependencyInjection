using System.Collections.Immutable;

namespace DragoAnt.Extensions.DependencyInjection;

internal readonly struct DependencyModel(
    INamedTypeSymbol instanceClassSymbol,
    ResolveDependencyServiceLifetime lifetime,
    ImmutableArray<INamedTypeSymbol> interfaces)
{
    public string InstanceClassName { get; } = instanceClassSymbol.Name;

    public IEnumerable<string> Interfaces => interfaces.Select(s => s.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat));

    public ResolveDependencyServiceLifetime Lifetime { get; } = lifetime;

    public void CollectNamespaces(ISet<string> namespaces)
    {
        instanceClassSymbol.CollectNamespaces(namespaces);
        foreach (var iface in interfaces)
        {
            iface.CollectNamespaces(namespaces);
        }
    }
}
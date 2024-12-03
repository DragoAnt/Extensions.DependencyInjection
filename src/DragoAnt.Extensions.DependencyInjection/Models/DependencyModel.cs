using System.Collections.Immutable;
using System.Runtime.CompilerServices;

namespace DragoAnt.Extensions.DependencyInjection;

internal readonly struct DependencyModel(
    bool itselfRegistration,
    ResolveDependencyLifetime lifetime,
    ISet<string> usings,
    string instanceClassName,
    ImmutableArray<string> interfaceNames,
    string? customFactoryMethod = null)
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static DependencyModel Create(
        bool itselfRegistration,
        ResolveDependencyLifetime lifetime,
        INamedTypeSymbol instanceClass,
        ImmutableArray<INamedTypeSymbol> interfaces,
        string? customFactoryMethod = null)
    {
        var usings = CollectNamespaces(instanceClass, interfaces);
        return new DependencyModel(itselfRegistration, lifetime, usings,
            instanceClass.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat),
            [..interfaces.Select(s => s.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat)).Distinct()],
            customFactoryMethod);
    }

    public ResolveDependencyLifetime Lifetime => lifetime;

    public bool ItselfRegistration { get; } = itselfRegistration;
    public string InstanceClassName { get; } = instanceClassName;
    public string? CustomFactoryMethod { get; } = customFactoryMethod;

    public ImmutableArray<string> Interfaces => interfaceNames;

    public void CollectNamespaces(ISet<string> namespaces) => namespaces.UnionWith(usings);

    private static ISet<string> CollectNamespaces(INamedTypeSymbol instanceClassSymbol, ImmutableArray<INamedTypeSymbol> interfaces)
    {
        var namespaces = new HashSet<string>();
        instanceClassSymbol.CollectNamespaces(namespaces);
        foreach (var iface in interfaces)
        {
            iface.CollectNamespaces(namespaces);
        }

        return namespaces;
    }
}
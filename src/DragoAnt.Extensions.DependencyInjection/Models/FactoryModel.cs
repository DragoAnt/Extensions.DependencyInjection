using System.Collections.Frozen;
using System.Collections.Immutable;

namespace DragoAnt.Extensions.DependencyInjection;

internal readonly struct FactoryModel(
    INamedTypeSymbol instanceClassSymbol,
    FactoryInterfaceModel? generatingInterface,
    ImmutableArray<FactoryInterfaceModel> factoryInterfaces,
    ResolveDependencyLifetime lifetime,
    ImmutableArray<MethodModel> constructors)
{
    public string InstanceClassName { get; } = instanceClassSymbol.Name;

    public string InstanceClassDefinition  
        => instanceClassSymbol.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);
    public string FactoryClassName => $"{InstanceClassName}Factory";
    private ImmutableArray<FactoryInterfaceModel> FactoryInterfaces => factoryInterfaces;

    public FactoryInterfaceModel? GeneratingInterface { get; } = generatingInterface;
    public ResolveDependencyLifetime Lifetime { get; } = lifetime;
    public ImmutableArray<MethodModel> Constructors { get; } = constructors;

    public void CollectNamespaces(ISet<string> namespaces)
    {
        instanceClassSymbol.CollectNamespaces(namespaces);
        foreach (var ctor in Constructors)
        {
            ctor.CollectNamespaces(namespaces);
        }

        foreach (var iface in FactoryInterfaces)
        {
            iface.CollectNamespaces(namespaces);
        }
    }

    public IEnumerable<FactoryInterfaceModel> GetInterfaces()
    {
        if (GeneratingInterface.HasValue)
        {
            yield return GeneratingInterface.Value;
        }

        foreach (var factoryInterface in FactoryInterfaces)
        {
            yield return factoryInterface;
        }
    }

    public DependencyModel CreateDependency()
    {
        //NOTE: We will already add usings during creating factory itself
        return new DependencyModel(false, Lifetime, FrozenSet<string>.Empty, FactoryClassName,
            [..GetInterfaces().Select(i => i.Name)]);
    }
}
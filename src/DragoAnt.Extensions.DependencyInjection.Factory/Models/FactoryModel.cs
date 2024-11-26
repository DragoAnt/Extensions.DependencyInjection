using System.Collections.Immutable;

namespace DragoAnt.Extensions.DependencyInjection.Factory;

internal readonly struct FactoryModel(
    INamedTypeSymbol instanceClassSymbol,
    FactoryInterfaceModel? generatingInterface,
    ImmutableArray<FactoryInterfaceModel> factoryInterfaces,
    ResolveFactoryServiceLifetime lifetime,
    ImmutableArray<MethodModel> constructors,
    Exception? exception = null)
{
    public bool IsInvalid => exception is not null;
    public string InstanceClassName { get; } = instanceClassSymbol.Name;
    public string FactoryClassName => $"{InstanceClassName}Factory";
    public ImmutableArray<FactoryInterfaceModel> FactoryInterfaces => factoryInterfaces;

    public string GetError()
    {
        if (exception is not null)
        {
            return $"Exception: {exception.Message}, InnerException: {exception.InnerException?.Message}";
        }

        return string.Empty;
    }

    public FactoryInterfaceModel? GeneratingInterface { get; } = generatingInterface;
    public ResolveFactoryServiceLifetime Lifetime { get; } = lifetime;
    public ImmutableArray<MethodModel> Constructors { get; } = constructors;

    public void CollectUsings(ISet<string> namespaces)
    {
        instanceClassSymbol.CollectNamespaces(namespaces);
        foreach (var parameter in Constructors)
        {
            parameter.CollectUsings(namespaces);
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
}
using System.Collections.Immutable;

namespace DragoAnt.Extensions.DependencyInjection.Factory;

internal readonly struct FactoryDeclaration(
    string instanceClassName,
    string classNamespace,
    SharedFactoryInterface sharedFactoryInterface,
    ResolveFactoryServiceLifetime lifetime,
    ImmutableArray<FactoryMethod> methods)
{
    public string InstanceClassName => instanceClassName;
    public string FactoryClassName => $"{instanceClassName}Factory";
    public string FactoryInterfaceName => $"I{instanceClassName}Factory";

    public bool HasSharedFactoryInterface => sharedFactoryInterface.Symbol is not null;

    public SharedFactoryInterface SharedFactoryInterface => sharedFactoryInterface;
    public string SharedFactoryInterfaceName => sharedFactoryInterface.GetSharedFactoryInterfaceName(InstanceClassName);

    public string GetError()
    {
        if (sharedFactoryInterface.Symbol is null)
        {
            return string.Empty;
        }

        var name = sharedFactoryInterface.Symbol.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);
        if (!name.EndsWith("<>"))
        {
            return $"#error Invalid shared factory interface type definition '{name}'";
        }

        return string.Empty;
    }

    public ResolveFactoryServiceLifetime Lifetime { get; } = lifetime;
    public ImmutableArray<FactoryMethod> Methods { get; } = methods;

    public void CollectUsings(ISet<string> namespaces)
    {
        namespaces.Add(classNamespace);
        foreach (var parameter in Methods)
        {
            parameter.CollectUsings(namespaces);
        }
    }

    public IEnumerable<string> GetImplementedInterfaces()
    {
        if (!sharedFactoryInterface.OnlySharedFactory)
        {
            yield return FactoryInterfaceName;
        }

        if (HasSharedFactoryInterface)
        {
            yield return SharedFactoryInterfaceName;
        }
    }
}
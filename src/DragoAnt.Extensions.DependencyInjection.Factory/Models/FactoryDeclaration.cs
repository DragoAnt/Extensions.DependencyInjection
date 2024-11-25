using System.Collections.Immutable;

namespace DragoAnt.Extensions.DependencyInjection.Factory;

internal readonly struct FactoryDeclaration(
    string instanceClassName,
    string classNamespace,
    ITypeSymbol? sharedFactoryInterfaceTypeDefinitionSymbol,
    ResolveFactoryServiceLifetime lifetime,
    ImmutableArray<FactoryMethod> methods)
{
    public string InstanceClassName => instanceClassName;
    public string FactoryClassName => $"{instanceClassName}Factory";
    public string FactoryInterfaceName => $"I{instanceClassName}Factory";

    public bool HasSharedFactoryInterface => sharedFactoryInterfaceTypeDefinitionSymbol is not null;

    public string SharedFactoryInterfaceName
    {
        get
        {
            if (sharedFactoryInterfaceTypeDefinitionSymbol is null)
            {
                return string.Empty;
            }

            var name = sharedFactoryInterfaceTypeDefinitionSymbol.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);
            return $"{name.Substring(0, name.Length - 2)}<{InstanceClassName}>";
        }
    }

    public string GetError()
    {
        if (sharedFactoryInterfaceTypeDefinitionSymbol is null)
        {
            return string.Empty;
        }

        var name = sharedFactoryInterfaceTypeDefinitionSymbol.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);
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
        yield return FactoryInterfaceName;
        if (HasSharedFactoryInterface)
        {
            yield return SharedFactoryInterfaceName;
        }
    }
}
using System.Collections.Immutable;

namespace DragoAnt.Extensions.DependencyInjection.Factory;

internal readonly struct FactoryDeclaration(
    string instanceClassName,
    string classNamespace,
    ResolveFactoryServiceLifetime lifetime,
    ImmutableArray<FactoryMethod> methods)
{
    public string InstanceClassName => instanceClassName;
    public string FactoryClassName => $"{instanceClassName}Factory";
    public string FactoryInterfaceName => $"I{instanceClassName}Factory";

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
}
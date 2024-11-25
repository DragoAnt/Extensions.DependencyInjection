using System.Collections.Immutable;

namespace DragoAnt.Extensions.DependencyInjection.Factory;

internal readonly struct FactoryMethod(ImmutableArray<MethodParameter> parameters)
{
    public ImmutableArray<MethodParameter> Parameters { get; } = parameters;

    public void CollectUsings(ISet<string> namespaces)
    {
        foreach (var parameter in Parameters)
        {
            parameter.CollectUsings(namespaces);
        }
    }

    public string GetParametersForSignature(bool forInterface) =>
        string.Join(", ", Parameters.Where(p => p.IsParameter).Select(p => p.ToSignature(forInterface)));

    public string GetParametersForCall(string providerFieldName) => string.Join(", ", Parameters.Select(p => p.ToCall(providerFieldName)));
}
using System.Collections.Immutable;

namespace DragoAnt.Extensions.DependencyInjection;

internal readonly struct MethodModel(
    string name,
    ImmutableArray<ITypeParameterSymbol> typeParameters,
    ITypeSymbol returnType,
    ImmutableArray<MethodParameterModel> parameters)
{
    public bool IsEmpty => ReturnType is null;
    public string Name { get; } = name + typeParameters.ToDisplayString();
    public ITypeSymbol ReturnType { get; } = returnType;
    public string ReturnTypeName => ReturnType.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);
    public ImmutableArray<MethodParameterModel> Parameters { get; } = parameters;

    public bool HasTypeParameterConstraints(bool interfaceImplementation)
        => typeParameters.Any(p => p.HasConstraints(interfaceImplementation));

    public IEnumerable<string> GetTypeParameterClauses(bool interfaceImplementation)
        => typeParameters.GetTypeParameterConstraintClauses(interfaceImplementation);

    public void CollectNamespaces(ISet<string> namespaces)
    {
        ReturnType.CollectNamespaces(namespaces);
        foreach (var parameter in Parameters)
        {
            parameter.CollectUsings(namespaces);
        }
    }

    public MethodParameterModel? GetEquivalentParameter(MethodParameterModel parameter, bool allowCastParameters)
    {
        var result = Parameters.FirstOrDefault(p => string.Equals(p.Name, parameter.Name, StringComparison.Ordinal));
        if (result.IsEmpty)
        {
            return null;
        }

        return allowCastParameters || MethodParameterModel.HasSameType(result, parameter.Type)
            ? result
            : null;
    }


    public string GetParametersForSignature(bool forInterface)
        => string.Join(", ", Parameters.Where(p => p.IsExplicitParameter).Select(p => p.ToSignature(forInterface)));

    public string GetCallCtorParameters(string providerFieldName, MethodModel executingMethod)
        => string.Join(", ", Parameters.Select(p => p.ToCall(providerFieldName, executingMethod)));
}
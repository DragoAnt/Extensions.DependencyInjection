namespace DragoAnt.Extensions.DependencyInjection.Factory;

internal readonly struct MethodParameterModel(IParameterSymbol parameter, bool forceExplicitParameter)
{
    private IParameterSymbol Parameter { get; } = parameter;

    public string Name => Parameter.Name;

    public ITypeSymbol Type => Parameter.Type;
    
    private string TypeName => Type.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);

    /// <summary>
    /// Is factory method parameter or not
    /// </summary>
    public bool IsExplicitParameter { get; } = forceExplicitParameter || GetIsExplicitParameter(parameter);

    public void CollectUsings(ISet<string> namespaces) => Parameter.Type.CollectNamespaces(namespaces);

    public string ToSignature(bool forInterface)
    {
        if (Parameter.HasExplicitDefaultValue && forInterface)
        {
            var explicitDefaultValue = Parameter.ExplicitDefaultValue switch
            {
                string v => $""""
                             """
                             {v}
                             """
                             """",
                bool v => v.ToString().ToLowerInvariant(),
                null => "default",
                { } v => v.ToString(),
            };
            return $"{TypeName} {Name} = {explicitDefaultValue}";
        }

        return $"{TypeName} {Name}";
    }

    public string ToCall(string providerFieldName, MethodModel? executingMethod)
    {
        var parameter = this;
        var castToType = Parameter.Type;
        if (executingMethod is not null)
        {
            parameter = executingMethod.Value.GetEquivalentParameter(parameter, true) ?? parameter;
        }

        return $"{GetCast(parameter, castToType)}{GetCallParam(parameter, providerFieldName)}";
    }

    private static string GetCast(MethodParameterModel parameter, ITypeSymbol? castToType)
        => castToType is null ||HasSameType(parameter, castToType)
            ? string.Empty
            : $"({castToType.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat)})";

    public static bool HasSameType(MethodParameterModel parameter, ITypeSymbol castToType) 
        => SymbolEqualityComparer.IncludeNullability.Equals(parameter.Type, castToType);

    private static string GetCallParam(MethodParameterModel parameter, string providerFieldName)
        => parameter.IsExplicitParameter ? parameter.Name : $"{providerFieldName}.GetRequiredService<{parameter.TypeName}>()";

    private static bool GetIsExplicitParameter(IParameterSymbol p)
    {
        var attributes = p.GetAttributes();
        // ReSharper disable SimplifyConditionalTernaryExpression
        return attributes.Any(attr => AttributeNames.ResolveFactoryParameter.IsMatchAttr(attr))
            ? true
            : attributes.Any(attr => AttributeNames.ResolveFactoryService.IsMatchAttr(attr))
                ? false
                : p.Type.IsImplicitParameter() && !p.Type.ToDisplayString().EndsWith("Factory");
        // ReSharper restore SimplifyConditionalTernaryExpression
    }
}
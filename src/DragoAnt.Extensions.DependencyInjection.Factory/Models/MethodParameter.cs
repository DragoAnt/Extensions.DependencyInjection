namespace DragoAnt.Extensions.DependencyInjection.Factory;

internal readonly struct MethodParameter(IParameterSymbol parameter)
{
    private IParameterSymbol Parameter { get; } = parameter;

    public string Name => Parameter.Name;
    private string Type => Parameter.Type.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);

    /// <summary>
    /// Is factory method parameter or not
    /// </summary>
    public bool IsParameter { get; } = GetIsParameter(parameter);

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
                null => "default",
                { } v => v.ToString(),
            };
            return $"{Type} {Name} = {explicitDefaultValue}";
        }

        return $"{Type} {Name}";
    }

    public string ToCall(string providerFieldName) => IsParameter ? Name : $"{providerFieldName}.GetRequiredService<{Type}>()";

    private static bool GetIsParameter(IParameterSymbol p)
    {
        var attributes = p.GetAttributes();
        // ReSharper disable SimplifyConditionalTernaryExpression
        return attributes.Any(attr => attr.AttributeClass?.Name == AttributeNames.ResolveFactoryParameter)
            ? true
            : attributes.Any(attr => attr.AttributeClass?.Name == AttributeNames.ResolveFactoryService)
                ? false
                : p.Type.IsImplicitParameter() && !p.Type.ToDisplayString().EndsWith("Factory");
        // ReSharper restore SimplifyConditionalTernaryExpression
    }
}
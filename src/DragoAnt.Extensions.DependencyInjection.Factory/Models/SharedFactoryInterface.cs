namespace DragoAnt.Extensions.DependencyInjection.Factory;

internal readonly struct SharedFactoryInterface(
    ITypeSymbol? symbol,
    bool onlySharedFactory,
    bool castParametersToSharedFactory,
    bool allowNotSupportedMethodsSharedFactory)
{
    public ITypeSymbol? Symbol { get; } = symbol;
    public bool OnlySharedFactory { get; } = onlySharedFactory;
    public bool CastParametersToSharedFactory { get; } = castParametersToSharedFactory;
    public bool AllowNotSupportedMethodsSharedFactory { get; } = allowNotSupportedMethodsSharedFactory;
    
    public string GetSharedFactoryInterfaceName(string className)
    {
        if (Symbol is null)
        {
            return string.Empty;
        }

        var name = Symbol.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);
        return $"{name.Substring(0, name.Length - 2)}<{className}>";
    }
}
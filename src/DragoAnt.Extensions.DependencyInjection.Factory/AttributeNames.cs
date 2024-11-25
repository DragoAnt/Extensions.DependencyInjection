namespace DragoAnt.Extensions.DependencyInjection.Factory;

internal static class AttributeNames
{
    public const string ResolveFactory = nameof(ResolveFactoryAttribute);
    public const string ResolveFactoryShort = "ResolveFactory";

    public const string ResolveFactoryIgnoreCtor = nameof(ResolveFactoryIgnoreCtorAttribute);
    public const string ResolveFactoryParameter = nameof(ResolveFactoryParameterAttribute);
    public const string ResolveFactoryService = nameof(ResolveFactoryServiceAttribute);
}
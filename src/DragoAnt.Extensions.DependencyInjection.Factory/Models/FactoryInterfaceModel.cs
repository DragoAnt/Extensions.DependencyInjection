using System.Collections.Immutable;

namespace DragoAnt.Extensions.DependencyInjection.Factory;

internal readonly struct FactoryInterfaceModel(string name, ImmutableArray<MethodModel> methods, bool castParameters = false, bool allowNotSupportedMethods = false)
{
    public string Name { get; } = name;
    public ImmutableArray<MethodModel> Methods { get; } = methods;

    public bool CastParameters { get; } = castParameters;

    public bool AllowNotSupportedMethods { get; } = allowNotSupportedMethods;
}
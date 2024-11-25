// This class imported to the project by DragoAnt.Extensions.DependencyInjection.Factory package

global using DragoAnt.Extensions.DependencyInjection.Factory;
using static System.AttributeTargets;

namespace DragoAnt.Extensions.DependencyInjection.Factory;

internal enum ResolveFactoryServiceLifetime
{
    Scoped = 0,
    Singleton = 1,
}

/// <summary>
/// Mark class with this attribute to generate factory and factory registration for Dependency Injection ServiceCollection.
/// </summary>
/// <param name="lifetime">Factory service lifetime.</param>
[AttributeUsage(Class)]
internal sealed class ResolveFactoryAttribute(
    ResolveFactoryServiceLifetime lifetime = ResolveFactoryServiceLifetime.Scoped) : Attribute
{
    public ResolveFactoryServiceLifetime Lifetime { get; } = lifetime;

    /// <summary>
    /// Shared factory generic interface type definition.(e.g. IFactory{T}).
    /// The interface contains Create methods with the same factory parameters as constructors.
    /// </summary>
    public Type? SharedFactoryInterfaceTypeDefinition { get; set; }

    /// <summary>
    /// Generate code only for shared factory. Skip generation and registration of class specific factory.
    /// Active only if <see cref="ResolveFactoryAttribute.SharedFactoryInterfaceTypeDefinition"/> is set.
    /// </summary>
    public bool OnlySharedFactory { get; set; }

    /// <summary>
    /// Cast parameters to shared factory method parameters. Mapped by name. Default is true.
    /// Active only if <see cref="ResolveFactoryAttribute.SharedFactoryInterfaceTypeDefinition"/> is set.
    /// </summary>
    public bool CastParametersToSharedFactory { get; set; } = true;

    /// <summary>
    ///If true and constructors can't be mapped by parameters to shared factory methods then generates throw <see cref="NotSupportedException"/> otherwise generates pragma error. 
    /// Active only if <see cref="ResolveFactoryAttribute.SharedFactoryInterfaceTypeDefinition"/> is set.
    /// </summary>
    public bool AllowNotSupportedMethodsSharedFactory { get; set; }
}

/// <summary>
/// Attribute to mark constrictor to be ignored during Factory code generation.
/// </summary>
[AttributeUsage(Constructor)]
internal sealed class ResolveFactoryIgnoreCtorAttribute : Attribute
{
}

/// <summary>
/// Attribute to mark parameter as factory parameter 
/// </summary>
[AttributeUsage(Parameter)]
internal sealed class ResolveFactoryParameterAttribute : Attribute
{
}

/// <summary>
/// Attribute to mark parameter as factory service 
/// </summary>
[AttributeUsage(Parameter)]
internal sealed class ResolveFactoryServiceAttribute : Attribute
{
}

/// <summary>
/// Attribute to mark referenced type to be an explicit factory parameter by default. 
/// </summary>
[AttributeUsage(Class | Interface)]
internal sealed class AsResolveFactoryParameterAttribute : Attribute
{
}
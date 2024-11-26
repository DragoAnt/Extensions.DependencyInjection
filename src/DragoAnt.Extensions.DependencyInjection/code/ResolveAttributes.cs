// This class imported to the project by DragoAnt.Extensions.DependencyInjection package

// ReSharper disable RedundantUsingDirective
// ReSharper disable once RedundantNullableDirective

#nullable enable

namespace DragoAnt.Extensions.DependencyInjection;

/// <summary>
/// Factory service lifetime. 
/// </summary>
internal enum ResolveFactoryServiceLifetime
{
    Scoped = 0,
    Singleton = 1,
}

/// <summary>
/// Mark class with this attribute to generate factory and factory registration for Dependency Injection ServiceCollection.
/// </summary>
/// <param name="lifetime">Factory service lifetime.</param>
[AttributeUsage(AttributeTargets.Class)]
internal sealed class ResolveFactoryAttribute(
    ResolveFactoryServiceLifetime lifetime = ResolveFactoryServiceLifetime.Scoped) : Attribute
{
    public ResolveFactoryServiceLifetime Lifetime { get; } = lifetime;

    /// <summary>
    /// Skip generation and registration of specific factory interface.
    /// </summary>
    public bool SkipGenerateInterface { get; set; }
}

/// <summary>
/// Mark class with this attribute to add implementation of factory contract to factory and registration for Dependency Injection ServiceCollection.
/// </summary>
/// <param name="interfaceTypeDefinition">Factory service contract.</param>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
internal sealed class ResolveFactoryContractAttribute(Type interfaceTypeDefinition) : Attribute
{
    /// <summary>
    /// Shared factory generic interface type definition.(e.g. IFactory{T}).
    /// The interface contains Create methods with the same factory parameters as constructors.
    /// </summary>
    public Type? InterfaceTypeDefinition { get; } = interfaceTypeDefinition;

    /// <summary>
    /// Cast parameters to shared factory method parameters. Mapped by name. Default is true.
    /// </summary>
    public bool CastParameters { get; set; } = true;

    /// <summary>
    ///If true and constructors can't be mapped by parameters to shared factory methods then generates throw <see cref="NotSupportedException"/> otherwise generates pragma error. 
    /// </summary>
    public bool AllowNotSupportedMethods { get; set; }
}

/// <summary>
/// Attribute to mark constrictor to be ignored during Factory code generation.
/// </summary>
[AttributeUsage(AttributeTargets.Constructor)]
internal sealed class ResolveFactoryIgnoreCtorAttribute : Attribute
{
}

/// <summary>
/// Attribute to mark parameter as factory parameter 
/// </summary>
[AttributeUsage(AttributeTargets.Parameter)]
internal sealed class ResolveFactoryParameterAttribute : Attribute
{
}

/// <summary>
/// Attribute to mark parameter as factory service 
/// </summary>
[AttributeUsage(AttributeTargets.Parameter)]
internal sealed class ResolveFactoryServiceAttribute : Attribute
{
}

/// <summary>
/// Attribute to mark referenced type to be an explicit factory parameter by default. 
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface)]
internal sealed class AsResolveFactoryParameterAttribute : Attribute
{
}

/// <summary>
/// Attribute to mark referenced type to be an explicit factory service by default. 
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface)]
internal sealed class AsResolveFactoryServiceAttribute : Attribute
{
}

/// <summary>
/// Service lifetime.
/// </summary>
internal enum ResolveDependencyServiceLifetime
{
    Transient = 0,
    Scoped = 2,
    Singleton = 3,
}

/// <summary>
/// Mark class with this attribute to generate factory and factory registration for Dependency Injection ServiceCollection.
/// </summary>
/// <param name="lifetime">Factory service lifetime.</param>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface)]
internal sealed class ResolveDependencyAttribute(
    ResolveDependencyServiceLifetime lifetime = ResolveDependencyServiceLifetime.Scoped) : Attribute
{
    public ResolveDependencyServiceLifetime Lifetime { get; } = lifetime;

    /// <summary>
    /// Service key for keyed service registration.
    /// </summary>
    public string? Key { get; set; }
}
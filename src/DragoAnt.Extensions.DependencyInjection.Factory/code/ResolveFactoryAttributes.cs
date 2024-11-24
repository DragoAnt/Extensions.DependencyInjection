// This class imported to the project by DragoAnt.Extensions.DependencyInjection.Factory package

global using DragoAnt.Extensions.DependencyInjection.Factory;

namespace DragoAnt.Extensions.DependencyInjection.Factory;

public enum ResolveFactoryServiceLifetime
{
    Scoped = 0,
    Singleton = 1,
}

/// <summary>
/// Mark class with this attribute to generate factory and factory registration for Dependency Injection ServiceCollection.
/// </summary>
/// <param name="lifetime">Factory service lifetime.</param>
[AttributeUsage(AttributeTargets.Class)]
public sealed class ResolveFactoryAttribute(ResolveFactoryServiceLifetime lifetime = ResolveFactoryServiceLifetime.Scoped) : Attribute
{
    public ResolveFactoryServiceLifetime Lifetime { get; } = lifetime;
}

/// <summary>
/// Attribute to mark constrictor to be ignored during Factory code generation.
/// </summary>
[AttributeUsage(AttributeTargets.Constructor)]
public sealed class ResolveFactoryIgnoreCtorAttribute : Attribute
{
}

/// <summary>
/// Attribute to mark parameter as factory parameter 
/// </summary>
[AttributeUsage(AttributeTargets.Parameter)]
public sealed class ResolveFactoryParameterAttribute : Attribute
{
}

/// <summary>
/// Attribute to mark parameter as factory service 
/// </summary>
[AttributeUsage(AttributeTargets.Parameter)]
public sealed class ResolveFactoryServiceAttribute : Attribute
{
}
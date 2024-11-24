﻿// This class imported to the project by DragoAnt.Extensions.DependencyInjection.Factory package

global using DragoAnt.Extensions.DependencyInjection.Factory;

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
[AttributeUsage(AttributeTargets.Class)]
internal sealed class ResolveFactoryAttribute(ResolveFactoryServiceLifetime lifetime = ResolveFactoryServiceLifetime.Scoped) : Attribute
{
    public ResolveFactoryServiceLifetime Lifetime { get; } = lifetime;
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
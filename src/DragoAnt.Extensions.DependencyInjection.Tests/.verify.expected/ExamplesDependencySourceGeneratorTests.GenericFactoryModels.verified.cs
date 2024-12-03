﻿// This file generated by DragoAnt.Extensions.DependencyInjection source generator.
// More details: https://github.com/DragoAnt/Extensions.DependencyInjection



#nullable enable

using DragoAnt.Extensions.DependencyInjection.Example.Models;
using DragoAnt.Extensions.DependencyInjection.Example.Options;
using DragoAnt.Extensions.DependencyInjection.Example.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using static Microsoft.Extensions.DependencyInjection.ServiceLifetime;

[assembly:ResolveAssembly("TestAssembly.TestAssemblyDependencyExtensions.AddTestAssemblyDependencies")]

namespace TestAssembly;

/// <summary>
/// Dependency injection registration extensions.
/// </summary>
public static partial class TestAssemblyDependencyExtensions
{
    /// <summary>
    /// AddTestAssemblyDependencies dependencies and factories registration extension.
    /// </summary>
    /// <remarks>Generated by <a href="https://github.com/DragoAnt/Extensions.DependencyInjection">DragoAnt.Extensions.DependencyInjection</a> source generator.</remarks>
    public static void AddTestAssemblyDependencies(this IServiceCollection services)
    {
        Func<IServiceProvider,object> factory;

        services.Add(new ServiceDescriptor(typeof(GenericViewModelFactory), typeof(GenericViewModelFactory), Scoped));
        factory = static p => p.GetRequiredService<GenericViewModelFactory>();
        services.Add(new ServiceDescriptor(typeof(IGenericViewModelFactory), factory, Scoped));
        services.Add(new ServiceDescriptor(typeof(IGenericFactory), factory, Scoped));
    }

}

/// <summary>
/// Factory contract for <see cref="GenericViewModel{T, T2}"/>.
/// </summary>
public interface IGenericViewModelFactory
{
    GenericViewModel<T, T2> Create<T, T2>(T exportPath, T2? exportPath2)
        where T : class
        where T2 : class, T, global::DragoAnt.Extensions.DependencyInjection.Example.Models.IGenericModel, new()
    ;
    GenericViewModel<T, T2> Create<T, T2>(string exportPath, T customCodePath, T2 testT2, AppOptions? options = default)
        where T : class
        where T2 : class, T, global::DragoAnt.Extensions.DependencyInjection.Example.Models.IGenericModel, new()
    ;
}

/// <summary>
/// Factory implementation for <see cref="GenericViewModel{T, T2}"/>.
/// </summary>
internal sealed class GenericViewModelFactory : IGenericViewModelFactory, IGenericFactory
{
    private readonly IServiceProvider _provider;

    public GenericViewModelFactory(IServiceProvider provider)
    {
        _provider = provider;        
    }

    GenericViewModel<T, T2> IGenericViewModelFactory.Create<T, T2>(T exportPath, T2? exportPath2) 
         where T : class
        where T2 : class
        => new GenericViewModel<T, T2>(exportPath, exportPath2);
    GenericViewModel<T, T2> IGenericViewModelFactory.Create<T, T2>(string exportPath, T customCodePath, T2 testT2, AppOptions? options) 
         where T : class
        where T2 : class
        => new GenericViewModel<T, T2>(exportPath, customCodePath, testT2, _provider.GetRequiredService<ITestService>(), _provider.GetRequiredService<ISingletonViewModelFactory>(), options);
    IGenericViewModel IGenericFactory.Create<T, T2>(T exportPath, T2? exportPath2) 
         where T : class
        where T2 : class
        => new GenericViewModel<T, T2>((T)exportPath, (T2?)exportPath2);
}

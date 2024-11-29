﻿// This file generated by DragoAnt.Extensions.DependencyInjection source generator.
// More details: https://github.com/DragoAnt/Extensions.DependencyInjection



#nullable enable

using DragoAnt.Extensions.DependencyInjection.Example.Models;
using DragoAnt.Extensions.DependencyInjection.Example.Options;
using DragoAnt.Extensions.DependencyInjection.Example.Services;
using Microsoft.Extensions.DependencyInjection;
using System;

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
        services.AddScoped<IPartialFactory<PartialViewModel>, PartialViewModelFactory>();
    }

}

/// <summary>
/// Factory implementation for <see cref="PartialViewModel"/>.
/// </summary>
internal sealed class PartialViewModelFactory : IPartialFactory<PartialViewModel>
{
    private readonly IServiceProvider _provider;

    public PartialViewModelFactory(IServiceProvider provider)
    {
        _provider = provider;        
    }

    PartialViewModel IPartialFactory<PartialViewModel>.Create() 
         => throw new System.NotSupportedException();
    PartialViewModel IPartialFactory<PartialViewModel>.Create(string exportPath) 
         => new PartialViewModel(exportPath);
}

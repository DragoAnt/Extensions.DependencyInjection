﻿using System.Collections.Immutable;

namespace DragoAnt.Extensions.DependencyInjection.Templates;

internal sealed class ResolveDependenciesData(
    string extensionsClassName,
    string methodCodeName,
    string ns,
    bool alwaysGenerateAddDependenciesMethod,
    bool customDependenciesEnabled,
    ImmutableArray<string> errors,
    ImmutableArray<DependencyModel> dependencies,
    ImmutableArray<FactoryModel> factories)
{
    public ImmutableArray<string> Errors => errors;
    public string MethodCodeName => methodCodeName;
    public string Namespace => ns;
    public ImmutableArray<FactoryModel> Factories => factories;
    public ImmutableArray<DependencyModel> Dependencies => dependencies;
    public bool AlwaysGenerateAddDependenciesMethod => alwaysGenerateAddDependenciesMethod;
    public object ExtensionsClassName => extensionsClassName;

    public bool CustomDependenciesEnabled => customDependenciesEnabled;

    public IEnumerable<string> GetUsings(params string[] includeNamespaces)
    {
        var usingsSet = new SortedSet<string>(includeNamespaces);

        foreach (var factory in Factories)
        {
            factory.CollectNamespaces(usingsSet);
        }

        foreach (var dependency in Dependencies)
        {
            dependency.CollectNamespaces(usingsSet);
        }

        return usingsSet;
    }
}
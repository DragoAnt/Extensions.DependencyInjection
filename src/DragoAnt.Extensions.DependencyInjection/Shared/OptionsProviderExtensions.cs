using Microsoft.CodeAnalysis.Diagnostics;

namespace DragoAnt.Extensions.DependencyInjection;

public static class OptionsProviderExtensions
{
    public static 
        (string rootNamespace, 
        bool alwaysGenerateMethod, bool customDependenciesEnabled, 
        string extensionsClassName, string methodName, string fullMethodName) GetOptionsProviderParams(
            this AnalyzerConfigOptionsProvider optionsProvider,
            Compilation compilation)
    {
        if (!optionsProvider.GlobalOptions.TryGetBuildProperty("RootNamespace", out var rootNamespace) || rootNamespace is null)
        {
            rootNamespace = compilation.AssemblyName;
            if (rootNamespace is null || string.IsNullOrEmpty(rootNamespace))
            {
                throw new InvalidOperationException(
                    "RootNamespace PropertyGroup is required. Open csproj file and add <RootNamespace> PropertyGroup to your project.");
            }
        }

        if (!optionsProvider.GlobalOptions.TryGetBuildProperty(BuildProperties.MethodSuffix_SkippedNamePartsCount,
                out var countOfPartsVal) ||
            countOfPartsVal is null ||
            !int.TryParse(countOfPartsVal, out var skippedParts))
        {
            skippedParts = 0;
        }

        if (!optionsProvider.GlobalOptions.TryGetBuildProperty(BuildProperties.MethodSuffix, out var methodSuffix) ||
            methodSuffix is null ||
            string.IsNullOrWhiteSpace(methodSuffix))
        {
            methodSuffix = GetDefaultMethodSuffix(rootNamespace, skippedParts);
        }

        if (!optionsProvider.GlobalOptions.TryGetBuildProperty(BuildProperties.AlwaysGenerateMethod,
                out var alwaysGenerateMethodVal) ||
            alwaysGenerateMethodVal is null ||
            !bool.TryParse(alwaysGenerateMethodVal, out var alwaysGenerateMethod))
        {
            alwaysGenerateMethod = true;
        }

        if (!optionsProvider.GlobalOptions.TryGetBuildProperty(BuildProperties.CustomDependencies,
                out var customDependenciesEnabledVal) ||
            alwaysGenerateMethodVal is null ||
            !bool.TryParse(customDependenciesEnabledVal, out var customDependenciesEnabled))
        {
            customDependenciesEnabled = false;
        }


        var extensionsClassName = $"{methodSuffix}DependencyExtensions";
        var methodName = $"Add{methodSuffix}Dependencies";
        var fullMethodName = $"{rootNamespace}.{extensionsClassName}.{methodName}";
        return (rootNamespace, alwaysGenerateMethod, customDependenciesEnabled, extensionsClassName, methodName, fullMethodName);
    }

    public static string GetDefaultMethodSuffix(string rootNamespace, int skippedParts)
    {
        var parts = rootNamespace.Split('.');
        skippedParts = Math.Min(parts.Length - 1, skippedParts);

        var methodSuffix = parts.Skip(skippedParts).Aggregate(new StringBuilder(),
            (current, part) =>
            {
                current.Append(part);
                return current;
            }).ToString();

        return methodSuffix;
    }
}
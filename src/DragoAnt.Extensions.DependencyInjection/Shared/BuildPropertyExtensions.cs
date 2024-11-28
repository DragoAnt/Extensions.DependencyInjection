using Microsoft.CodeAnalysis.Diagnostics;

namespace DragoAnt.Extensions.DependencyInjection;

internal static class BuildPropertyExtensions
{
    public static bool TryGetBuildProperty(this AnalyzerConfigOptions options, string key, out string? value) =>
        options.TryGetValue($"build_property.{key}", out value);
}
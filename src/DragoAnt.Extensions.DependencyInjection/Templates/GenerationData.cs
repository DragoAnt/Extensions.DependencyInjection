using System.Collections.Immutable;

namespace DragoAnt.Extensions.DependencyInjection.Templates;

internal sealed class GenerationData(
    string methodCodeName,
    string ns,
    ImmutableArray<string> errors,
    ImmutableArray<DependencyModel> dependencies,
    ImmutableArray<FactoryModel> factories)
{
    public ImmutableArray<string> Errors => errors;
    public string MethodCodeName => methodCodeName;
    public string Namespace => ns;
    public ImmutableArray<FactoryModel> Factories => factories;
    public ImmutableArray<DependencyModel> Dependencies => dependencies;

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
using System.Collections.Immutable;

namespace DragoAnt.Extensions.DependencyInjection.Factory.Templates;

internal sealed class GenerationData
{
    public GenerationData(string methodCodeName, string ns, ImmutableArray<FactoryModel> factories)
    {
        Namespace = ns;
        Factories = factories;
        MethodCodeName = methodCodeName;
    }

    public string MethodCodeName { get; }
    public string Namespace { get; }
    public ImmutableArray<FactoryModel> Factories { get; }

    public IEnumerable<string> GetUsings(params string[] usings)
    {
        var usingsSet = new SortedSet<string>(usings);

        foreach (var factory in Factories)
        {
            factory.CollectUsings(usingsSet);
        }

        return usingsSet;
    }
}
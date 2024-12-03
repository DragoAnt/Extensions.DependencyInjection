namespace DragoAnt.Extensions.DependencyInjection.Referenced;

internal sealed class ResolveReferencedDependenciesData(
    string ns,
    bool alwaysGenerateOwnDependenciesMethod,
    string[] ownMethodNames,
    string[] referencedMethodNames)
{
    public string Namespace => ns;

    public bool AlwaysGenerateOwnDependenciesMethod { get; } = alwaysGenerateOwnDependenciesMethod;

    public IEnumerable<string> GetUsings(params string[] includeNamespaces)
    {
        var usingsSet = new SortedSet<string>(includeNamespaces);
        return usingsSet;
    }

    public IEnumerable<string> GetOwnDependenciesMethodsNames() => ownMethodNames;

    public IEnumerable<string> GetReferencedDependenciesMethodsNames() => referencedMethodNames;
}
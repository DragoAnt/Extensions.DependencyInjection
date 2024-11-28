namespace DragoAnt.Extensions.DependencyInjection.Templates;

internal sealed class ResolveReferencedDependenciesData(
    string ns,
    string[] fullMethodNames)
{
    public string Namespace => ns;

    public IEnumerable<string> GetUsings(params string[] includeNamespaces)
    {
        var usingsSet = new SortedSet<string>(includeNamespaces);

        return usingsSet;
    }

    public IEnumerable<string> GetRegistrationMethodsNames() => fullMethodNames;
}
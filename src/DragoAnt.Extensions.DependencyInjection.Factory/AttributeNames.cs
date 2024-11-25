using System.Text.RegularExpressions;
using static System.Text.RegularExpressions.RegexOptions;

namespace DragoAnt.Extensions.DependencyInjection.Factory;

internal static class AttributeNames
{
    public static readonly Regex ResolveFactory = new("^ResolveFactory(Attribute)?$", Compiled | CultureInvariant);
    public static readonly Regex ResolveFactoryContract = new("^ResolveFactoryContract(Attribute)?$", Compiled | CultureInvariant);

    public static readonly Regex ResolveFactoryIgnoreCtor = new("^ResolveFactoryIgnoreCtor(Attribute)?$", Compiled | CultureInvariant);
    public static readonly Regex ResolveFactoryParameter = new("^ResolveFactoryParameter(Attribute)?$", Compiled | CultureInvariant);
    public static readonly Regex ResolveFactoryService = new("^ResolveFactoryService(Attribute)?$", Compiled | CultureInvariant);
    public static readonly Regex AsResolveFactoryParameter = new("^AsResolveFactoryParameter(Attribute)?$", Compiled | CultureInvariant);

    public static bool IsMatchAttr(this Regex regex, AttributeData attribute)
    {
        var attributeName = attribute.AttributeClass?.Name;
        if (attributeName is null || string.IsNullOrEmpty(attributeName))
        {
            return false;
        }

        return regex.IsMatch(attributeName);
    }
}
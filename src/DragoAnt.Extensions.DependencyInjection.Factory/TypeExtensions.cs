using System.Collections.Immutable;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace DragoAnt.Extensions.DependencyInjection.Factory;

internal static class TypeExtensions
{
    public static bool TryGetOption(this AnalyzerConfigOptions options, string key, out string? value) =>
        options.TryGetValue($"build_property.{key}", out value);

    public static bool HasResolveFactoryAttribute(this ClassDeclarationSyntax classDecl)
    {
        for (var i = 0; i < classDecl.AttributeLists.Count; i++)
        {
            var al = classDecl.AttributeLists[i];
            for (var j = 0; j < al.Attributes.Count; j++)
            {
                var attr = al.Attributes[j];
                var name = attr.Name.ToString();
                if (name is AttributeNames.ResolveFactory or AttributeNames.ResolveFactoryShort)
                {
                    return true;
                }
            }
        }

        return false;
    }

    public static FactoryDeclaration? GetFactory(this GeneratorSyntaxContext context)
    {
        var classSyntax = (ClassDeclarationSyntax)context.Node;
        var symbol = context.SemanticModel.GetDeclaredSymbol(classSyntax);

        return GetFactory(symbol);
    }

    private static FactoryDeclaration? GetFactory(this ISymbol? symbol)
    {
        if (!(symbol is INamedTypeSymbol classSymbol &&
              classSymbol.GetAttributes().FirstOrDefault(attr => attr.AttributeClass?.Name == AttributeNames.ResolveFactory) is { } resolveFactoryAttr))
        {
            return null;
        }

        var lifetime = resolveFactoryAttr.ConstructorArguments.FirstOrDefault().Value?.ToString() is { } lifetimeStr &&
                       Enum.TryParse<ResolveFactoryServiceLifetime>(lifetimeStr, out var lifetimeValue)
            ? lifetimeValue
            : ResolveFactoryServiceLifetime.Scoped;


        var methods = classSymbol.Constructors
            .Where(ctor => !ctor.GetAttributes().Any(attr => attr.AttributeClass?.Name == AttributeNames.ResolveFactoryIgnoreCtor))
            .Select(GetFactoryMethod).ToImmutableArray();

        if (methods.Length == 0)
        {
            return null;
        }

        //TODO: Check for similar method signatures
        return new FactoryDeclaration(classSymbol.Name, classSymbol.ContainingNamespace.ToDisplayString(), lifetime, methods);
    }

    private static FactoryMethod GetFactoryMethod(IMethodSymbol ctor)
    {
        var parameters = ctor.Parameters.Select(p => new MethodParameter(p)).ToImmutableArray();
        return new FactoryMethod(parameters);
    }

    public static bool IsImplicitParameter(this ITypeSymbol type)
        => type.SpecialType is not SpecialType.None ||
           type.TypeKind is not TypeKind.Class and not TypeKind.Interface;

    public static void CollectNamespaces(this ITypeSymbol symbol, ISet<string> namespaces)
    {
        if (symbol.ContainingNamespace.IsGlobalNamespace)
        {
            return;
        }

        namespaces.Add(symbol.ContainingNamespace.ToDisplayString());

        // If generic type, recursively collect namespaces of type arguments
        if (symbol is INamedTypeSymbol namedTypeSymbol)
        {
            foreach (var typeArg in namedTypeSymbol.TypeArguments)
            {
                CollectNamespaces(typeArg, namespaces);
            }
        }
    }
}
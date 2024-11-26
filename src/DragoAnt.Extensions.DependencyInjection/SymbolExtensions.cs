using System.Runtime.CompilerServices;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace DragoAnt.Extensions.DependencyInjection;

public static class SymbolExtensions
{
    public static IEnumerable<AttributeData> GetClassAttributes(this ClassDeclarationSyntax classNode, SemanticModel semanticModel, bool wholeHierarchy = true)
    {
        var baseTypeSyntax = classNode.BaseList?.Types.FirstOrDefault();
        if (baseTypeSyntax == null)
        {
            return [];
        }

        return semanticModel.GetDeclaredSymbol(classNode) is { } typeSymbol
            ? GetClassAttributes(typeSymbol, wholeHierarchy)
            : [];
    }

    public static IEnumerable<AttributeData> GetClassAttributes(this INamedTypeSymbol typeSymbol, bool wholeHierarchy = true)
    {
        do
        {
            foreach (var attr in typeSymbol.GetAttributes())
            {
                yield return attr;
            }

            if (typeSymbol.BaseType is { SpecialType: not SpecialType.System_Object } baseTypeSymbol)
            {
                typeSymbol = baseTypeSymbol;
                continue;
            }

            break;
            // ReSharper disable once LoopVariableIsNeverChangedInsideLoop
        } while (wholeHierarchy);
    }

    public static IEnumerable<(AttributeData Attr, INamedTypeSymbol Type)> GetAllAttributes(this INamedTypeSymbol typeSymbol)
    {
        foreach (var attr in typeSymbol.GetAttributes())
        {
            yield return (attr, typeSymbol);
        }

        if (typeSymbol.BaseType is { SpecialType: not SpecialType.System_Object } baseTypeSymbol)
        {
            foreach (var attr in baseTypeSymbol.GetAllAttributes())
            {
                yield return attr;
            }
        }

        foreach (var symbolInterface in typeSymbol.Interfaces)
        {
            foreach (var attr in symbolInterface.GetAllAttributes())
            {
                yield return attr;
            }
        }
    }

    public static void CollectNamespaces(this ITypeSymbol symbol, ISet<string> namespaces)
    {
        symbol.ContainingNamespace.CollectNamespaces(namespaces);

        // If generic type, recursively collect namespaces of type arguments
        if (symbol is INamedTypeSymbol namedTypeSymbol)
        {
            foreach (var typeArg in namedTypeSymbol.TypeArguments)
            {
                CollectNamespaces(typeArg, namespaces);
            }
        }
    }

    private static void CollectNamespaces(this INamespaceSymbol? namespaceSymbol, ISet<string> namespaces)
    {
        if (namespaceSymbol is null || namespaceSymbol.IsGlobalNamespace)
        {
            return;
        }

        foreach (var ns in namespaceSymbol.ConstituentNamespaces)
        {
            namespaces.Add(ns.ToDisplayString());
        }
    }

    public static bool GetBoolNamedArgumentValue(this AttributeData resolveFactoryAttr, string name, bool defaultValue = false)
    {
        return GetNamedArgumentValue(resolveFactoryAttr, name) as bool? ?? defaultValue;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static object? GetNamedArgumentValue(this AttributeData resolveFactoryAttr, string argumentName)
    {
        return resolveFactoryAttr.NamedArguments
            .FirstOrDefault(p => p.Key == argumentName)
            .Value.Value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T? GetNamedArgumentValue<T>(this AttributeData resolveFactoryAttr, string argumentName)
        where T : class
    {
        return resolveFactoryAttr.GetNamedArgumentValue(argumentName) as T;
    }

    /// <summary>
    /// Checks is class has ResolveFactory attribute explicitly or not. 
    /// </summary>
    /// <param name="classDecl">Class syntax.</param>
    /// <returns></returns>
    public static bool HasResolveFactoryAttribute(this ClassDeclarationSyntax classDecl)
    {
        for (var i = 0; i < classDecl.AttributeLists.Count; i++)
        {
            var al = classDecl.AttributeLists[i];
            for (var j = 0; j < al.Attributes.Count; j++)
            {
                var attr = al.Attributes[j];
                var name = attr.Name.ToString();
                if (AttributeNames.ResolveFactory.IsMatch(name))
                {
                    return true;
                }
            }
        }

        return false;
    }
}
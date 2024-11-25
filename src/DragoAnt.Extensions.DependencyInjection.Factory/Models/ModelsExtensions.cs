using System.Collections.Immutable;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace DragoAnt.Extensions.DependencyInjection.Factory;

internal static class ModelsExtensions
{
    public static bool TryGetOption(this AnalyzerConfigOptions options, string key, out string? value) =>
        options.TryGetValue($"build_property.{key}", out value);


    public static FactoryDeclaration? GetFactory(this GeneratorSyntaxContext context)
        => GetFactoryDeclaration((ClassDeclarationSyntax)context.Node, context.SemanticModel);

    public static FactoryDeclaration? GetFactoryDeclaration(this ClassDeclarationSyntax classSyntax, SemanticModel semanticModel)
    {
        var declaredSymbol = semanticModel.GetDeclaredSymbol(classSyntax);
        if (declaredSymbol is null || declaredSymbol.IsAbstract)
        {
            return null;
        }

        if (!(declaredSymbol is INamedTypeSymbol classSymbol &&
              GetResolveFactoryAttr(classSymbol) is { } resolveFactoryAttr))
        {
            return null;
        }

        var lifetime = resolveFactoryAttr.ConstructorArguments
                           .FirstOrDefault().Value?.ToString() is { } lifetimeStr &&
                       Enum.TryParse<ResolveFactoryServiceLifetime>(lifetimeStr, out var lifetimeValue)
            ? lifetimeValue
            : ResolveFactoryServiceLifetime.Scoped;

        var sharedFactoryInterfaceTypeSymbol =
            resolveFactoryAttr.GetNamedArgumentValue<ITypeSymbol>(nameof(ResolveFactoryAttribute.SharedFactoryInterfaceTypeDefinition));

        var onlySharedFactory = false;
        var castParametersToSharedFactory = false;
        var allowNotSupportedMethodsSharedFactory = false;

        if (sharedFactoryInterfaceTypeSymbol is not null)
        {
            onlySharedFactory =
                resolveFactoryAttr.GetBoolNamedArgumentValue(nameof(ResolveFactoryAttribute.OnlySharedFactory));

            castParametersToSharedFactory =
                resolveFactoryAttr.GetBoolNamedArgumentValue(nameof(ResolveFactoryAttribute.CastParametersToSharedFactory), true);


            allowNotSupportedMethodsSharedFactory =
                resolveFactoryAttr.GetBoolNamedArgumentValue(nameof(ResolveFactoryAttribute.AllowNotSupportedMethodsSharedFactory));
        }

        var methods = classSymbol.Constructors
            .Where(ctor => !ctor.GetAttributes().Any(attr => AttributeNames.ResolveFactoryIgnoreCtor.IsMatchAttr(attr)))
            .Select(GetFactoryMethod).ToImmutableArray();

        if (methods.Length == 0)
        {
            return null;
        }

        //TODO: Check for similar method signatures

        return new FactoryDeclaration(
            classSymbol.Name,
            classSymbol.ContainingNamespace.ToDisplayString(),
            new(sharedFactoryInterfaceTypeSymbol, onlySharedFactory, castParametersToSharedFactory, allowNotSupportedMethodsSharedFactory),
            lifetime,
            methods);
    }

    private static AttributeData? GetResolveFactoryAttr(INamedTypeSymbol classSymbol)
    {
        var attributes = classSymbol.GetAttributes();
        return attributes.FirstOrDefault(attr => AttributeNames.ResolveFactory.IsMatchAttr(attr));
    }

    private static FactoryMethod GetFactoryMethod(IMethodSymbol ctor)
    {
        var parameters = ctor.Parameters.Select(p => new MethodParameter(p)).ToImmutableArray();
        return new FactoryMethod(parameters);
    }

    public static bool IsImplicitParameter(this ITypeSymbol type)
        => type.SpecialType is not SpecialType.None ||
           type.TypeKind is not TypeKind.Class and not TypeKind.Interface ||
           IsTypeMarkedAsParameter(type);

    private static bool IsTypeMarkedAsParameter(ITypeSymbol typeSymbol)
    {
        if (typeSymbol is INamedTypeSymbol namedTypeSymbol)
        {
            return namedTypeSymbol.GetAllAttributes().Any(attr => AttributeNames.AsResolveFactoryParameter.IsMatchAttr(attr));
        }

        return false;
    }
}
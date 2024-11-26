using System.Collections.Immutable;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace DragoAnt.Extensions.DependencyInjection.Factory;

internal static class ModelsExtensions
{
    public static bool TryGetOption(this AnalyzerConfigOptions options, string key, out string? value) =>
        options.TryGetValue($"build_property.{key}", out value);


    public static FactoryModel? GetFactory(this GeneratorSyntaxContext context)
    {
        try
        {
            return GetFactory((ClassDeclarationSyntax)context.Node, context.SemanticModel);
        }
        catch (Exception e)
        {
            return new FactoryModel(null!, null, [], ResolveFactoryServiceLifetime.Scoped, [],
                new FactoryGeneratorException($"Failed to construct factory for class '{context.Node}'", e));
        }
    }

    public static FactoryModel? GetFactory(this ClassDeclarationSyntax classSyntax, SemanticModel semanticModel)
    {
        var declaredSymbol = semanticModel.GetDeclaredSymbol(classSyntax);
        if (declaredSymbol is null || declaredSymbol.IsAbstract)
        {
            return null;
        }

        if (declaredSymbol is not INamedTypeSymbol classSymbol)
        {
            return null;
        }

        var attributes = GetResolveFactoryAttributes(classSymbol);
        var resolveFactoryAttr = attributes.FirstOrDefault(attr => AttributeNames.ResolveFactory.IsMatchAttr(attr));
        if (resolveFactoryAttr is null)
        {
            return null;
        }

        var lifetime = resolveFactoryAttr.ConstructorArguments
                           .FirstOrDefault().Value?.ToString() is { } lifetimeStr &&
                       Enum.TryParse<ResolveFactoryServiceLifetime>(lifetimeStr, out var lifetimeValue)
            ? lifetimeValue
            : ResolveFactoryServiceLifetime.Scoped;

        var skipGenerateInterface =
            resolveFactoryAttr.GetBoolNamedArgumentValue(nameof(ResolveFactoryAttribute.SkipGenerateInterface));

        var ctors = classSymbol.Constructors
            .Where(ctor => !ctor.GetAttributes().Any(attr => !AttributeNames.ResolveFactoryIgnoreCtor.IsMatchAttr(attr)))
            .Select(m => GetFactoryMethod(m, null, classSymbol, false)).ToImmutableArray();

        FactoryInterfaceModel? generatingInterface = null;
        var className = classSymbol.Name;

        if (!skipGenerateInterface)
        {
            generatingInterface = new FactoryInterfaceModel($"I{className}Factory",
                [..ctors.Select(m => new MethodModel("Create", m.ReturnType, [..m.Parameters.Where(p => p.IsExplicitParameter)]))]);
        }

        var factoryInterfaces = GetContractInterfaces(attributes, classSymbol).ToImmutableArray();
        if (ctors.Length == 0)
        {
            return null;
        }

        //TODO: Check for similar method signatures
        return new FactoryModel(
            classSymbol,
            generatingInterface,
            factoryInterfaces,
            lifetime,
            ctors);
    }

    private static IEnumerable<FactoryInterfaceModel> GetContractInterfaces(ImmutableArray<AttributeData> attributes, INamedTypeSymbol className)
    {
        foreach (var contractAttr in attributes.Where(attr => AttributeNames.ResolveFactoryContract.IsMatchAttr(attr)))
        {
            if (contractAttr.ConstructorArguments.FirstOrDefault().Value is not INamedTypeSymbol interfaceSymbol)
            {
                //TODO: Add error
                continue;
            }

            var castParameters =
                contractAttr.GetBoolNamedArgumentValue(nameof(ResolveFactoryContractAttribute.CastParameters), true);

            var allowNotSupportedMethods =
                contractAttr.GetBoolNamedArgumentValue(nameof(ResolveFactoryContractAttribute.AllowNotSupportedMethods));

            var interfaceName = interfaceSymbol.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);

            Func<ITypeSymbol, ITypeSymbol>? typeMap = null;
            if (interfaceSymbol.IsUnboundGenericType)
            {
                if (interfaceSymbol.ConstructedFrom.TypeArguments.Length != 1)
                {
                    //TODO: Add error;
                    continue;
                }

                var typeArg = interfaceSymbol.ConstructedFrom.TypeArguments[0];
                typeMap = t => typeArg.Name == t.Name ? className : t;

                interfaceName =
                    $"{interfaceName.Substring(0, interfaceName.Length - 2)}<{className.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat)}>";
            }

            var methods = interfaceSymbol.ConstructedFrom.GetMembers().OfType<IMethodSymbol>().Where(m => m.MethodKind == MethodKind.Ordinary)
                .Select(m => GetFactoryMethod(m, null, null, true, typeMap)).ToImmutableArray();


            yield return new FactoryInterfaceModel(interfaceName, methods, castParameters, allowNotSupportedMethods);
        }
    }

    private static ImmutableArray<AttributeData> GetResolveFactoryAttributes(INamedTypeSymbol classSymbol)
    {
        var attributes = classSymbol.GetClassAttributes();
        return
        [
            ..attributes.Where(attr => AttributeNames.ResolveFactory.IsMatchAttr(attr) ||
                                       AttributeNames.ResolveFactoryContract.IsMatchAttr(attr)),
        ];
    }

    private static MethodModel GetFactoryMethod(
        this IMethodSymbol method,
        string? name,
        ITypeSymbol? returnType,
        bool forceExplicitParameter,
        Func<ITypeSymbol, ITypeSymbol>? typeMap = null)
    {
        typeMap ??= t => t;
        name ??= method.Name;
        returnType ??= typeMap(method.ReturnType);
        var parameters = method.Parameters.Select(p => new MethodParameterModel(p, forceExplicitParameter)).ToImmutableArray();

        return new MethodModel(name, returnType, parameters);
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

    public static MethodModel? GetEquivalentConstructorMethod(this MethodModel method, ImmutableArray<MethodModel> ctors, bool allowCastParameters)
    {
        foreach (var ctor in ctors)
        {
            var found = true;
            var ctorExplicitParameters = ctor.Parameters.Where(p => p.IsExplicitParameter).ToImmutableArray();

            if (ctorExplicitParameters.Length != method.Parameters.Length)
            {
                continue;
            }

            foreach (var ctorParameter in ctorExplicitParameters)
            {
                var parameter = method.GetEquivalentParameter(ctorParameter, allowCastParameters);
                if (parameter is not null)
                {
                    continue;
                }

                found = false;
                break;
            }

            if (found)
            {
                return ctor;
            }
        }

        return null;
    }
}
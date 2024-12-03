using System.Collections.Immutable;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.Accessibility;

namespace DragoAnt.Extensions.DependencyInjection;

internal static class ModelsExtensions
{
    public static DependencyItem? GetDependencyItem(this GeneratorSyntaxContext context)
    {
        try
        {
            return GetDependencyItem((ClassDeclarationSyntax)context.Node, context.SemanticModel);
        }
        catch (Exception e)
        {
            return new DependencyItem(null, null,
                new DependencyGeneratorException($"Failed to construct dependency for class '{context.Node}'", e));
        }
    }

    public static DependencyItem? GetDependencyItem(this ClassDeclarationSyntax classSyntax, SemanticModel semanticModel)
    {
        var declaredSymbol = semanticModel.GetDeclaredSymbol(classSyntax);
        if (declaredSymbol is null ||
            declaredSymbol.IsAbstract ||
            declaredSymbol.DeclaredAccessibility == Private ||
            declaredSymbol.IsStatic)
        {
            return null;
        }

        if (declaredSymbol is not INamedTypeSymbol classSymbol)
        {
            return null;
        }

        var attributes = GetResolveAttributes(classSymbol);
        FactoryModel? factory = null;
        if (attributes.FirstOrDefault(attr => AttributeNames.ResolveFactory.IsMatchAttr(attr.Attr)) is { Attr: { } resolveFactoryAttr })
        {
            factory = GetFactory(resolveFactoryAttr, classSymbol, attributes);
        }

        var dependency = GetDependency(classSymbol, attributes);

        if (dependency is null && factory is null)
        {
            return null;
        }

        return new DependencyItem(dependency, factory);
    }

    private static DependencyModel? GetDependency(INamedTypeSymbol classSymbol, ImmutableArray<(AttributeData Attr, INamedTypeSymbol Type)> attributes)
    {
        var ignoreDependency = classSymbol.GetAttributes().FirstOrDefault(attr => AttributeNames.ResolveDependencyIgnore.IsMatchAttr(attr));
        if (ignoreDependency is not null)
        {
            return null;
        }

        var dependenciesAttributes = attributes.Where(attr => AttributeNames.ResolveDependency.IsMatchAttr(attr.Attr)).ToImmutableArray();
        if (dependenciesAttributes.Length == 0)
        {
            return null;
        }

        var lifetime = ParseAttributes(classSymbol, dependenciesAttributes, out var interfaces, out var itselfAttribute);

        var customFactoryMethod = itselfAttribute?.NamedArguments
            .FirstOrDefault(arg => arg.Key == nameof(ResolveDependencyAttribute.CustomFactoryMethodName))
            .Value.Value as string;

        return DependencyModel.Create(itselfAttribute is not null, lifetime, classSymbol, [..interfaces], customFactoryMethod);
    }

    private static ResolveDependencyLifetime ParseAttributes(
        INamedTypeSymbol classSymbol,
        ImmutableArray<(AttributeData Attr, INamedTypeSymbol Type)> dependenciesAttributes,
        out List<INamedTypeSymbol> interfaces,
        out AttributeData? itselfAttribute)
    {
        ResolveDependencyLifetime classLifetime = default;
        ResolveDependencyLifetime lifetime = default;

        interfaces = [];
        itselfAttribute = null;

        foreach (var (attr, type) in dependenciesAttributes)
        {
            var attrLifetime = attr.ConstructorArguments
                                   .FirstOrDefault().Value?.ToString() is { } lifetimeStr &&
                               Enum.TryParse<ResolveDependencyLifetime>(lifetimeStr, out var lifetimeValue)
                ? lifetimeValue
                : default;

            if (attrLifetime > lifetime)
            {
                lifetime = attrLifetime;
            }

            if (SymbolEqualityComparer.Default.Equals(classSymbol, type))
            {
                itselfAttribute = attr;
                classLifetime = attrLifetime;
            }
            else
            {
                interfaces.Add(type);
            }
        }

        if (classLifetime != default)
        {
            lifetime = classLifetime;
        }

        if (lifetime == default)
        {
            lifetime = ResolveDependencyScoped;
        }

        return lifetime;
    }

    private static FactoryModel GetFactory(
        AttributeData resolveFactoryAttr,
        INamedTypeSymbol classSymbol,
        ImmutableArray<(AttributeData Attr, INamedTypeSymbol Type)> attributes)
    {
        var lifetime = resolveFactoryAttr.ConstructorArguments
                           .FirstOrDefault().Value?.ToString() is { } lifetimeStr &&
                       Enum.TryParse<ResolveDependencyLifetime>(lifetimeStr, out var lifetimeValue)
            ? lifetimeValue
            : ResolveDependencyScoped;

        var skipGenerateInterface =
            resolveFactoryAttr.GetBoolNamedArgumentValue(nameof(ResolveFactoryAttribute.SkipGenerateInterface));

        var ctors = classSymbol.Constructors
            .Where(ctor =>
                ctor.MethodKind == MethodKind.Constructor &&
                ctor.DeclaredAccessibility is Public or Internal &&
                !ctor.GetAttributes().Any(attr => !AttributeNames.ResolveFactoryIgnoreCtor.IsMatchAttr(attr)))
            .Select(m => GetMethodModel(m, null, classSymbol, false)).ToImmutableArray();

        FactoryInterfaceModel? generatingInterface = null;
        var className = classSymbol.Name;

        if (!skipGenerateInterface)
        {
            generatingInterface = new FactoryInterfaceModel($"I{className}Factory", null,
                [..ctors.Select(m => new MethodModel("Create", classSymbol.TypeParameters, classSymbol, [..m.Parameters.Where(p => p.IsExplicitParameter)]))]);
        }

        var factoryInterfaces = GetContractInterfaces(attributes, classSymbol).ToImmutableArray();
        if (ctors.Length == 0)
        {
            throw new DependencyGeneratorException("No constructors were found for factory.", null!);
        }

        return new FactoryModel(classSymbol, generatingInterface, factoryInterfaces, lifetime, ctors);
    }

    private static IEnumerable<FactoryInterfaceModel> GetContractInterfaces(
        ImmutableArray<(AttributeData Attr, INamedTypeSymbol Type)> attributes,
        INamedTypeSymbol className)
    {
        HashSet<string> interfaceNames = new(StringComparer.Ordinal);

        foreach (var (contractAttr, _) in attributes.Where(data => AttributeNames.ResolveFactoryContract.IsMatchAttr(data.Attr)))
        {
            if (contractAttr.ConstructorArguments.FirstOrDefault().Value is not INamedTypeSymbol interfaceSymbol)
            {
                //TODO: Add error
                continue;
            }

            if (!interfaceNames.Add(interfaceSymbol.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat)))
            {
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
                .Select(m => GetMethodModel(m, null, null, true, typeMap)).ToImmutableArray();


            yield return new FactoryInterfaceModel(interfaceName, interfaceSymbol, methods, castParameters, allowNotSupportedMethods);
        }
    }

    private static ImmutableArray<(AttributeData Attr, INamedTypeSymbol Type)> GetResolveAttributes(INamedTypeSymbol classSymbol)
    {
        var attributes = classSymbol.GetAllAttributes();
        return
        [
            ..attributes.Where(data =>
                AttributeNames.ResolveDependency.IsMatchAttr(data.Attr) ||
                AttributeNames.ResolveFactory.IsMatchAttr(data.Attr) ||
                AttributeNames.ResolveFactoryContract.IsMatchAttr(data.Attr)),
        ];
    }

    private static MethodModel GetMethodModel(
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

        return new MethodModel(name, method.TypeParameters, returnType, parameters);
    }

    public static bool IsImplicitParameter(this ITypeSymbol type)
        => type.IsTypeMarkedAs() switch
        {
            ResolveParameterType.Service => false,
            ResolveParameterType.ExplicitParameter => true,
            _ => type.SpecialType is not SpecialType.None ||
                 type.TypeKind is not TypeKind.Class and not TypeKind.Interface,
        };

    private static ResolveParameterType IsTypeMarkedAs(this ITypeSymbol typeSymbol)
    {
        if (typeSymbol is INamedTypeSymbol namedTypeSymbol)
        {
            foreach (var attr in namedTypeSymbol.GetAllAttributes())
            {
                if (AttributeNames.AsResolveFactoryParameter.IsMatchAttr(attr.Attr))
                {
                    return ResolveParameterType.ExplicitParameter;
                }

                if (AttributeNames.AsResolveFactoryService.IsMatchAttr(attr.Attr))
                {
                    return ResolveParameterType.Service;
                }
            }
        }

        return default;
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

    public static string ToServiceLifetime(this ResolveDependencyLifetime lifetime)
        => lifetime switch
        {
            ResolveDependencyScoped => "Scoped",
            ResolveDependencySingleton => "Singleton",
            _ => throw new NotSupportedException($"Lifetime '{lifetime}' is not supported."),
        };
}
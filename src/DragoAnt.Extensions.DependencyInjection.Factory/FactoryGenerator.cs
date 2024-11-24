using System.Collections.Immutable;
using DragoAnt.Extensions.DependencyInjection.Factory.Templates;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;
using static Microsoft.CodeAnalysis.CSharp.SyntaxKind;

namespace DragoAnt.Extensions.DependencyInjection.Factory;

[Generator]
public class FactoryGenerator : IIncrementalGenerator
{
    private const string ResolveFactoryDependencyInjectionMethodNameProjectProperty = "ResolveFactoryDependencyInjectionMethodName";

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var classDeclarations =
            context.SyntaxProvider.CreateSyntaxProvider(
                    predicate: static (s, _) => IsResolveFactoryMarkedClass(s),
                    transform: static (ctx, _) => ctx.GetFactory())
                .Where(static m => m is not null)
                .Select(static (f, _) => f!.Value);

        var combined = classDeclarations.Collect().Combine(context.AnalyzerConfigOptionsProvider);

        context.RegisterSourceOutput(
            combined,
            (ctx, data) =>
            {
                var (factories, optionsProvider) = data;

                // Retrieve additional settings from csproj
                if (!optionsProvider.GlobalOptions.TryGetOption(ResolveFactoryDependencyInjectionMethodNameProjectProperty, out var registerMethodName) ||
                    registerMethodName is null)
                {
                    registerMethodName = "NotSetMethodName";
                }

                if (!optionsProvider.GlobalOptions.TryGetOption("RootNamespace", out var rootNamespace) || rootNamespace is null)
                {
                    rootNamespace = "NotSetRootNamespace";
                }

                var generatedCode = GenerateFactories(new(registerMethodName, rootNamespace, factories));

                ctx.AddSource($"{registerMethodName}Factories.g.cs", SourceText.From(generatedCode, Encoding.UTF8));
            });
    }

    private static string GenerateFactories(GenerationData data)
        => new ResolveFactoriesTemplate
        {
            Data = data
        }.TransformText();


    private static bool IsResolveFactoryMarkedClass(SyntaxNode syntaxNode)
        => syntaxNode is ClassDeclarationSyntax classDecl &&
           !classDecl.Modifiers.Any(PrivateKeyword) &&
           (classDecl.Modifiers.Any(InternalKeyword) || classDecl.Modifiers.Any(PublicKeyword)) &&
           classDecl.HasResolveFactoryAttribute();
}

internal static class AttributeNames
{
    public const string ResolveFactory = nameof(ResolveFactoryAttribute);
    public const string ResolveFactoryShort = "ResolveFactory";

    public const string ResolveFactoryIgnoreCtor = nameof(ResolveFactoryIgnoreCtorAttribute);
    public const string ResolveFactoryParameter = nameof(ResolveFactoryParameterAttribute);
    public const string ResolveFactoryService = nameof(ResolveFactoryServiceAttribute);
}

internal readonly struct FactoryDeclaration(
    string instanceClassName,
    string classNamespace,
    ResolveFactoryServiceLifetime lifetime,
    ImmutableArray<FactoryMethod> methods)
{
    public string InstanceClassName => instanceClassName;
    public string FactoryClassName => $"{instanceClassName}Factory";
    public string FactoryInterfaceName => $"I{instanceClassName}Factory";

    public ResolveFactoryServiceLifetime Lifetime { get; } = lifetime;
    public ImmutableArray<FactoryMethod> Methods { get; } = methods;

    public void CollectUsings(ISet<string> namespaces)
    {
        namespaces.Add(classNamespace);
        foreach (var parameter in Methods)
        {
            parameter.CollectUsings(namespaces);
        }
    }
}

internal readonly struct FactoryMethod(ImmutableArray<MethodParameter> parameters)
{
    public ImmutableArray<MethodParameter> Parameters { get; } = parameters;

    public void CollectUsings(ISet<string> namespaces)
    {
        foreach (var parameter in Parameters)
        {
            parameter.CollectUsings(namespaces);
        }
    }

    public string GetParametersForSignature(bool forInterface) =>
        string.Join(", ", Parameters.Where(p => p.IsParameter).Select(p => p.ToSignature(forInterface)));

    public string GetParametersForCall(string providerFieldName) => string.Join(", ", Parameters.Select(p => p.ToCall(providerFieldName)));
}

internal readonly struct MethodParameter(IParameterSymbol parameter)
{
    private IParameterSymbol Parameter { get; } = parameter;

    public string Name => Parameter.Name;
    private string Type => Parameter.Type.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);

    /// <summary>
    /// Is factory method parameter or not
    /// </summary>
    public bool IsParameter { get; } = GetIsParameter(parameter);

    public void CollectUsings(ISet<string> namespaces) => Parameter.Type.CollectNamespaces(namespaces);

    public string ToSignature(bool forInterface)
    {
        if (Parameter.HasExplicitDefaultValue && forInterface)
        {
            var explicitDefaultValue = Parameter.ExplicitDefaultValue switch
            {
                string v => $""""
                             """
                             {v}
                             """
                             """",
                null => "default",
                { } v => v.ToString(),
            };
            return $"{Type} {Name} = {explicitDefaultValue}";
        }

        return $"{Type} {Name}";
    }

    public string ToCall(string providerFieldName) => IsParameter ? Name : $"{providerFieldName}.GetRequiredService<{Type}>()";

    private static bool GetIsParameter(IParameterSymbol p)
    {
        var attributes = p.GetAttributes();
        // ReSharper disable SimplifyConditionalTernaryExpression
        return attributes.Any(attr => attr.AttributeClass?.Name == AttributeNames.ResolveFactoryParameter)
            ? true
            : attributes.Any(attr => attr.AttributeClass?.Name == AttributeNames.ResolveFactoryService)
                ? false
                : p.Type.IsImplicitParameter();
        // ReSharper restore SimplifyConditionalTernaryExpression
    }
}

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

    public static FactoryDeclaration? GetFactory(this GeneratorExecutionContext context, ClassDeclarationSyntax classInfo)
    {
        var semanticModel = context.Compilation.GetSemanticModel(classInfo.SyntaxTree);
        var symbol = semanticModel.GetDeclaredSymbol(classInfo);

        return GetFactory(symbol);
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

    private static IEnumerable<string> GetUsingsFromTypeSymbol(this ITypeSymbol typeSymbol)
    {
        var namespaces = new HashSet<string>();
        CollectNamespaces(typeSymbol, namespaces);
        return namespaces;
    }

    public static void CollectNamespaces(this ITypeSymbol symbol, ISet<string> namespaces)
    {
        if (namespaces.Contains(symbol.ContainingNamespace.ToDisplayString()))
            return;

        // Add the namespace of the current symbol
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

    // public static bool IsNullablePrimitive(this ITypeSymbol type)
    // {
    //     return type.OriginalDefinition.SpecialType == SpecialType.System_Nullable_T &&
    //            type is INamedTypeSymbol named &&
    //            named.TypeArguments[0].IsPrimitive();
    // }
    //
    // public static bool IsPrimitiveCollection(this ITypeSymbol type)
    // {
    //     if (type is INamedTypeSymbol namedType)
    //     {
    //         return namedType.Name == "IEnumerable" && namedType.TypeArguments.Any(t => t.IsPrimitive());
    //     }
    //
    //     return false;
    // }
}
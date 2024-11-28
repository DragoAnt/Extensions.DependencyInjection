using System.Collections.Immutable;
using DragoAnt.Extensions.DependencyInjection.Templates;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using static Microsoft.CodeAnalysis.CSharp.SyntaxKind;

// ReSharper disable InconsistentNaming

namespace DragoAnt.Extensions.DependencyInjection;

[Generator]
public class DependencyGenerator : IIncrementalGenerator
{
    private static class BuildProperties
    {
        /// <summary>
        /// Suffix for Add_Suffix_Dependencies name
        /// </summary>
        public const string MethodSuffix = "DragoAnt_MethodSuffix";

        /// <summary>
        /// For suffix convention. Count of removing name parts at RootNamespace. Usually used for cut the first part - company name.  
        /// </summary>
        public const string MethodSuffix_SkippedNamePartsCount = "DragoAnt_MethodSuffix_SkippedNamePartsCount";
    }


    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var classDeclarations =
            context.SyntaxProvider.CreateSyntaxProvider(
                    predicate: static (s, _) => IsNotPrivateOrAbstractOrStaticClass(s),
                    transform: static (ctx, _) => ctx.GetDependencyItem())
                .Where(static m => m is not null)
                .Select(static (f, _) => f!.Value);

        var combined = classDeclarations.Collect().Combine(context.AnalyzerConfigOptionsProvider);

        context.RegisterSourceOutput(
            combined,
            (ctx, data) =>
            {
                var (items, optionsProvider) = data;

                if (!optionsProvider.GlobalOptions.TryGetBuildProperty("RootNamespace", out var rootNamespace) || rootNamespace is null)
                {
                    throw new InvalidOperationException(
                        "RootNamespace PropertyGroup is required. Open csproj file and add <RootNamespace> PropertyGroup to your project.");
                }

                if (!optionsProvider.GlobalOptions.TryGetBuildProperty(BuildProperties.MethodSuffix_SkippedNamePartsCount,
                        out var countOfPartsVal) ||
                    countOfPartsVal is null ||
                    !int.TryParse(countOfPartsVal, out var skippedParts))
                {
                    skippedParts = 0;
                }

                if (!optionsProvider.GlobalOptions.TryGetBuildProperty(BuildProperties.MethodSuffix, out var methodSuffix) ||
                    methodSuffix is null ||
                    string.IsNullOrWhiteSpace(methodSuffix))
                {
                    methodSuffix = GetDefaultMethodSuffix(rootNamespace, skippedParts);
                }


                string generatedCode;
                try
                {
                    var errors = items.Where(i => i.IsInvalid).Select(i => i.GetError()).ToImmutableArray();
                    //NOTE: Distinct for partial classes
                    var dependencies = items.Where(i => i is { IsInvalid: false, Dependency: not null })
                        .Select(i => i.Dependency!.Value).Distinct(new DependencyModelEqualityComparer()).ToImmutableArray();
                    var factories = items.Where(i => i is { IsInvalid: false, Factory: not null })
                        .Select(i => i.Factory!.Value).Distinct(new FactoryModelEqualityComparer()).ToImmutableArray();

                    generatedCode = new ResolveDependenciesTemplate
                    {
                        Data = new(methodSuffix, rootNamespace, errors, dependencies, factories)
                    }.TransformText();
                }
                catch (Exception e)
                {
                    generatedCode = e.ToString();
                }

                ctx.AddSource($"{methodSuffix}Dependencies.g.cs", SourceText.From(generatedCode, Encoding.UTF8));
            });
    }

    public static string GetDefaultMethodSuffix(string rootNamespace, int skippedParts)
    {
        var parts = rootNamespace.Split('.');
        skippedParts = Math.Min(parts.Length - 1, skippedParts);

        var methodSuffix = parts.Skip(skippedParts).Aggregate(new StringBuilder(),
            (current, part) =>
            {
                current.Append(part);
                return current;
            }).ToString();

        return methodSuffix;
    }

    private sealed class FactoryModelEqualityComparer : IEqualityComparer<FactoryModel>
    {
        public bool Equals(FactoryModel x, FactoryModel y) => x.FactoryClassName == y.FactoryClassName;

        public int GetHashCode(FactoryModel obj) => obj.FactoryClassName.GetHashCode();
    }

    private sealed class DependencyModelEqualityComparer : IEqualityComparer<DependencyModel>
    {
        public bool Equals(DependencyModel x, DependencyModel y) => x.InstanceClassName == y.InstanceClassName;

        public int GetHashCode(DependencyModel obj) => obj.InstanceClassName.GetHashCode();
    }

    private static bool IsNotPrivateOrAbstractOrStaticClass(SyntaxNode syntaxNode)
        => syntaxNode is ClassDeclarationSyntax classDecl &&
           !classDecl.Modifiers.Any(PrivateKeyword) &&
           !classDecl.Modifiers.Any(AbstractKeyword) &&
           !classDecl.Modifiers.Any(StaticKeyword);
}
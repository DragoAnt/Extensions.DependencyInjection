using System.Collections.Immutable;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using static Microsoft.CodeAnalysis.CSharp.SyntaxKind;

namespace DragoAnt.Extensions.DependencyInjection;

[Generator]
public class DependencyGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var classDeclarations =
            context.SyntaxProvider.CreateSyntaxProvider(
                    predicate: static (s, _) => IsNotPrivateOrAbstractOrStaticClass(s),
                    transform: static (ctx, _) => ctx.GetDependencyItem())
                .Where(static m => m is not null)
                .Select(static (f, _) => f!.Value);

        var combined = classDeclarations.Collect().Combine(context.AnalyzerConfigOptionsProvider).Combine(context.CompilationProvider);

        context.RegisterSourceOutput(combined, (ctx, nextProvider) =>
        {
            var (data, compilation) = nextProvider;
            var (items, optionsProvider) = data;
            var (rootNamespace, alwaysGenerateMethod, customDependenciesEnabled, extensionsClassName, methodName, fullMethodName)
                = optionsProvider.GetOptionsProviderParams(compilation);

            string generatedCode;
            try
            {
                var errors = items.Where(i => i.IsInvalid).Select(i => i.GetError()).ToImmutableArray();

                var factories = items.Where(i => i is { IsInvalid: false, Factory: not null })
                    .Select(i => i.Factory!.Value).Distinct(new FactoryModelEqualityComparer()).ToImmutableArray();

                //NOTE: Distinct for partial classes
                var dependencies = items.Where(i => i is { IsInvalid: false, Dependency: not null })
                    .Select(i => i.Dependency!.Value).Distinct(new DependencyModelEqualityComparer())
                    .Concat(factories.Select(f => f.CreateDependency()))
                    .ToImmutableArray();

                generatedCode = new DependencyGeneratorTemplate
                {
                    Data = new(fullMethodName, extensionsClassName, methodName, rootNamespace,
                        alwaysGenerateMethod, customDependenciesEnabled,
                        errors, dependencies, factories)
                }.TransformText();
            }
            catch (Exception e)
            {
                generatedCode = e.ToString();
            }


            ctx.AddSource($"{extensionsClassName}.g.cs", SourceText.From(generatedCode, Encoding.UTF8));
        });
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
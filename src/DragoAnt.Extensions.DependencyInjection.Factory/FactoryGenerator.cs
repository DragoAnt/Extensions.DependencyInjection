using DragoAnt.Extensions.DependencyInjection.Factory.Templates;
using Microsoft.CodeAnalysis.CSharp.Syntax;
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
                    predicate: static (s, _) => IsNotPrivateClass(s),
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

                var resolveFactoryDebug = 
                    optionsProvider.GlobalOptions.TryGetOption("ResolveFactoryDebug", out var resolveFactoryDebugValue) && resolveFactoryDebugValue is not null;

                string generatedCode;
                try
                {
                    //NOTE: Distinct for partial classes
                    var inputFactories = factories.Distinct(new FactoryModelEqualityComparer());

                    generatedCode = new ResolveFactoriesTemplate
                    {
                        Data = new(registerMethodName, rootNamespace, resolveFactoryDebug, [..inputFactories])
                    }.TransformText();
                }
                catch (Exception e)
                {
                    generatedCode = e.ToString();
                }

                ctx.AddSource($"{registerMethodName}Factories.g.cs", SourceText.From(generatedCode, Encoding.UTF8));
            });
    }

    private sealed class FactoryModelEqualityComparer : IEqualityComparer<FactoryModel>
    {
        public bool Equals(FactoryModel x, FactoryModel y) => x.FactoryClassName == y.FactoryClassName;

        public int GetHashCode(FactoryModel obj) => obj.FactoryClassName.GetHashCode();
    }

    private static bool IsNotPrivateClass(SyntaxNode syntaxNode)
        => syntaxNode is ClassDeclarationSyntax classDecl && !classDecl.Modifiers.Any(PrivateKeyword);
}
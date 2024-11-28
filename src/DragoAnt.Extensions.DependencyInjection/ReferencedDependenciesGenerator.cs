using DragoAnt.Extensions.DependencyInjection.Templates;
using Microsoft.CodeAnalysis.Text;

namespace DragoAnt.Extensions.DependencyInjection;

[Generator]
public class ReferencedDependenciesGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var combined = context.CompilationProvider.Combine(context.AnalyzerConfigOptionsProvider);

        // Register source generation logic
        context.RegisterSourceOutput(combined, (spc, data) =>
        {
            var (compilation, optionsProvider) = data;
            if (!optionsProvider.GlobalOptions.TryGetBuildProperty("RootNamespace", out var rootNamespace) || rootNamespace is null)
            {
                rootNamespace = compilation.AssemblyName;
                if (rootNamespace is null || string.IsNullOrEmpty(rootNamespace))
                {
                    throw new InvalidOperationException(
                        "RootNamespace PropertyGroup is required. Open csproj file and add <RootNamespace> PropertyGroup to your project.");
                }
            }

            var attributes = compilation.Assembly.Modules
                .SelectMany(module => module.ReferencedAssemblySymbols)
                .SelectMany((asm, _) => asm.GetAttributes());

            var fullMethodNames = attributes
                .Where(attr => attr.AttributeClass?.Name == nameof(ResolveAssemblyAttribute))
                .Select(attr => attr.ConstructorArguments.FirstOrDefault().Value as string)
                .Where(symbol => symbol != null)
                .OfType<string>()
                .OrderBy(v => v)
                .ToArray();

            string generatedCode;
            try
            {
                generatedCode = new ResolveReferencedDependenciesTemplate
                {
                    Data = new(rootNamespace, fullMethodNames)
                }.TransformText();
            }
            catch (Exception e)
            {
                generatedCode = e.ToString();
            }

            spc.AddSource("Referenced.DependenciesExtensions.g.cs", SourceText.From(generatedCode, Encoding.UTF8));
        });
    }
}
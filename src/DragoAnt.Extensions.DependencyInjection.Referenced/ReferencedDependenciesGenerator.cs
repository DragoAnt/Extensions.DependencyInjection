using Microsoft.CodeAnalysis.Text;

namespace DragoAnt.Extensions.DependencyInjection.Referenced;

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

            var (rootNamespace, alwaysGenerateOwnDependenciesMethod, _, _, _, fullMethodName)
                = optionsProvider.GetOptionsProviderParams(compilation);

            string generatedCode;
            try
            {
                var referencedDependenciesMethodNames = GetDependenciesFullMethodNames(
                    compilation.Assembly.Modules
                        .SelectMany(module => module.ReferencedAssemblySymbols)
                        .SelectMany((asm, _) => asm.GetAttributes()));

                generatedCode = new ResolveReferencedDependenciesTemplate
                {
                    Data = new(rootNamespace, alwaysGenerateOwnDependenciesMethod, [fullMethodName], referencedDependenciesMethodNames)
                }.TransformText();
            }
            catch (Exception e)
            {
                generatedCode = e.ToString();
            }

            spc.AddSource("Referenced.DependenciesExtensions.g.cs", SourceText.From(generatedCode, Encoding.UTF8));
        });
    }

    private static string[] GetDependenciesFullMethodNames(IEnumerable<AttributeData> attributes)
    {
        var referencedFullMethodNames = attributes
            .Where(attr => attr.AttributeClass?.Name == nameof(ResolveAssemblyAttribute))
            .Select(attr => attr.ConstructorArguments.FirstOrDefault().Value as string)
            .Where(symbol => symbol != null)
            .OfType<string>()
            .OrderBy(v => v)
            .ToArray();
        return referencedFullMethodNames;
    }
}
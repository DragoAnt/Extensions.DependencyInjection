using DragoAnt.Extensions.DependencyInjection.Example;
using DragoAnt.Extensions.DependencyInjection.Referenced;
using static Microsoft.CodeAnalysis.CSharp.CSharpSyntaxTree;

namespace DragoAnt.Extensions.DependencyInjection.Tests;

public abstract class BaseReferencedDependenciesSourceGeneratorTests
{
    private static readonly SyntaxTree[] CommonSyntaxTrees
        =
        [
            ParseText(ReadEmbeddedCode("code.ResolveAttributes.cs")),
        ];

    protected string RunFactoryGenerator(params string[] inputSource)
    {
        // Create a CSharp syntax tree from the source code

        var inputSyntaxTrees = inputSource.Select(s => ParseText(s));

        // Create a compilation
        var compilation = CSharpCompilation.Create(
            assemblyName: "TestAssembly",
            syntaxTrees: [..CommonSyntaxTrees, ..inputSyntaxTrees],
            references:
            [
                MetadataReference.CreateFromFile(typeof(object).Assembly.Location), // mscorlib
                MetadataReference.CreateFromFile(typeof(ModelTests).Assembly.Location), // Test-specific types
                MetadataReference.CreateFromFile(typeof(System.Runtime.GCSettings).Assembly.Location) // System.Runtime
            ],
            options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
        );

        // Create an instance of your generator
        var generator = new ReferencedDependenciesGenerator();

        // Create a generator driver
        var driver = CSharpGeneratorDriver.Create(generator);

        // Act: Run the generator
        var generatorDriver = driver.RunGenerators(compilation);

        // Get the results
        var results = generatorDriver.GetRunResult();

        // Assert: Verify the generated output
        Assert.Single(results.GeneratedTrees);
        var generatedCode = results.GeneratedTrees[0].ToString();

        return generatedCode;
    }
}
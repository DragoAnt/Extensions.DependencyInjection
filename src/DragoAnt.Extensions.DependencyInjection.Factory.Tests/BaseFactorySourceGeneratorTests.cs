using static Microsoft.CodeAnalysis.CSharp.CSharpSyntaxTree;

namespace DragoAnt.Extensions.DependencyInjection.Factory.Tests;

public abstract class BaseFactorySourceGeneratorTests
{
    private static readonly SyntaxTree[] CommonSyntaxTrees
        =
        [
            ParseText(ReadEmbeddedCode("code.ResolveFactoryAttributes.cs")),
            ParseText(ReadEmbeddedCode("examples.GlobalUsings.cs")),
            ParseText(ReadEmbeddedCode("examples.Options.Options.cs")),
            ParseText(ReadEmbeddedCode("examples.Services.Services.cs")),
        ];

    protected string RunFactoryGenerator(string inputSource)
    {
        // Create a CSharp syntax tree from the source code

        var syntaxTree = ParseText(inputSource);

        // Create a compilation
        var compilation = CSharpCompilation.Create(
            assemblyName: "TestAssembly",
            syntaxTrees: [..CommonSyntaxTrees, syntaxTree],
            references:
            [
                MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(ExamplesFactorySourceGeneratorTests).Assembly.Location),
            ],
            options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
        );

        // Create an instance of your generator
        var generator = new FactoryGenerator();

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
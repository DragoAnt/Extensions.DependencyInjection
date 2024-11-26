using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace DragoAnt.Extensions.DependencyInjection.Factory.Tests;

public class FactorySourceGeneratorTests
{
    [Fact]
    public void TestGenerator_OutputIsCorrect()
    {
        // Arrange: Define the source code to be used as input
        //language=csharp
        var inputSource = """
                          
                          using System;
                          
                          public interface ITestService {}
                          
                          [ResolveFactory]
                          public class SampleClass
                          {
                              public SampleClass(ITestService service) { }
                          }
                          
                          """;

        // Create a CSharp syntax tree from the source code
        var syntaxTree = CSharpSyntaxTree.ParseText(inputSource);

        // Create a compilation
        var compilation = CSharpCompilation.Create(
            assemblyName: "TestAssembly",
            syntaxTrees: [syntaxTree],
            references:
            [
                MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(FactorySourceGeneratorTests).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(ResolveFactoryAttribute).Assembly.Location),
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

        // Compare the generated code with the expected output
        var expectedOutput = @"
            partial class SampleClass
            {
                public static SampleClass Resolve(IServiceProvider provider, string name, int age)
                {
                    return new SampleClass(name, age);
                }
            }
        ";
        Assert.Equal(expectedOutput.Trim(), generatedCode.Trim());
    }
}
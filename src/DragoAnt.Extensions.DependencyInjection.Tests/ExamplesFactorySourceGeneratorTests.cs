namespace DragoAnt.Extensions.DependencyInjection.Tests;

public class ExamplesFactorySourceGeneratorTests : BaseFactorySourceGeneratorTests
{
    [Fact]
    public void SimpleModels()
    {
        //TODO: Use Verify
        var generatedCode = RunFactoryGeneratorExamples("SimpleModels");
    }

    [Fact]
    public void CommonFactoryInterfaceModels()
    {
        //TODO: Use Verify
        var generatedCode = RunFactoryGeneratorExamples("CommonFactoryInterfaceModels");
    }

    [Fact]
    public void InheritedCommonFactoryInterfaceModels()
    {
        //TODO: Use Verify
        var generatedCode = RunFactoryGeneratorExamples("InheritedCommonFactoryInterfaceModels");
    }

    [Fact]
    public void PartialModels()
    {
        //TODO: Use Verify
        var generatedCode = RunFactoryGeneratorExamples("PartialModels", "Partial2Models");
    }

    private string RunFactoryGeneratorExamples(params string[] exampleCodeNames)
        => RunFactoryGenerator(exampleCodeNames.Select(ReadEmbeddedExampleModelCode).ToArray());
}
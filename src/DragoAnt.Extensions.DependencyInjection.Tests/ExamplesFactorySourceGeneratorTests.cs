namespace DragoAnt.Extensions.DependencyInjection.Tests;

public class ExamplesFactorySourceGeneratorTests : BaseFactorySourceGeneratorTests
{
    private readonly VerifySettings _settings;

    public ExamplesFactorySourceGeneratorTests()
    {
        _settings = new VerifySettings();
        _settings.UseDirectory(".verify.expected");
    }


    [Fact]
    public Task SimpleModels() => RunAndVerifyFactoryGeneratorExamples("SimpleModels");

    [Fact]
    public Task CommonFactoryInterfaceModels() => RunAndVerifyFactoryGeneratorExamples("CommonFactoryInterfaceModels");

    [Fact]
    public Task InheritedCommonFactoryInterfaceModels() => RunAndVerifyFactoryGeneratorExamples("InheritedCommonFactoryInterfaceModels");

    [Fact]
    public Task PartialModels() => RunAndVerifyFactoryGeneratorExamples("PartialModels", "Partial2Models");


    [Fact]
    public Task DependencyModels() => RunAndVerifyFactoryGeneratorExamples("DependencyModels");


    private Task RunAndVerifyFactoryGeneratorExamples(params string[] exampleCodeNames)
    {
        var generatedCode = RunFactoryGenerator(exampleCodeNames.Select(ReadEmbeddedExampleModelCode).ToArray());
        return Verify(generatedCode, "cs", _settings);
    }
}
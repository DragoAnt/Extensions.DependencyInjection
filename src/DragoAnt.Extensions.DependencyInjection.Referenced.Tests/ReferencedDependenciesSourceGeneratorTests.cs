namespace DragoAnt.Extensions.DependencyInjection.Tests;

public class ReferencedDependenciesSourceGeneratorTests : BaseReferencedDependenciesSourceGeneratorTests
{
    private readonly VerifySettings _settings;

    public ReferencedDependenciesSourceGeneratorTests()
    {
        _settings = new VerifySettings();
        _settings.UseDirectory(".verify.expected");
    }
    
    [Fact]
    public Task ReferencedMethod() => RunAndVerifyFactoryGeneratorExamples(
        """
        [assembly:ResolveAssembly("DragoAnt.Extensions.DependencyInjection.Referenced.Example.ReferencedExampleDependencyExtensions.AddReferencedExampleDependencies")]
        
        public class Test {}
        """);
    
    private Task RunAndVerifyFactoryGeneratorExamples(params string[] exampleCodeSources)
    {
        var generatedCode = RunFactoryGenerator(exampleCodeSources);
        return Verify(generatedCode, "cs", _settings);
    }
}
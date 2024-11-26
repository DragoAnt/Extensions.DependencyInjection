namespace DragoAnt.Extensions.DependencyInjection.Factory.Tests;

public class ExamplesFactorySourceGeneratorTests : BaseFactorySourceGeneratorTests
{
    [Fact]
    public void SimpleModels()
    {
        var inputSource = ReadEmbeddedExampleModelCode("SimpleModels");

        //TODO: Use Verify
        var generatedCode = RunFactoryGenerator(inputSource);
    }
    
    [Fact]
    public void CommonFactoryInterfaceModels()
    {
        var inputSource = ReadEmbeddedExampleModelCode("CommonFactoryInterfaceModels");

        //TODO: Use Verify
        var generatedCode = RunFactoryGenerator(inputSource);
    }
    
    [Fact]
    public void InheritedCommonFactoryInterfaceModels()
    {
        var inputSource = ReadEmbeddedExampleModelCode("InheritedCommonFactoryInterfaceModels");

        //TODO: Use Verify
        var generatedCode = RunFactoryGenerator(inputSource);
    }

    
}
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

namespace DragoAnt.Extensions.DependencyInjection.Factory.Example;

public class ModelTests
{
    private readonly ServiceProvider _serviceProvider;

    public ModelTests()
    {
        var services = new ServiceCollection();

        services.AddSingleton<TestService>();
        services.AddScoped<ITestService>(provider => provider.GetRequiredService<TestService>());

        services.AddExampleFactories();
        _serviceProvider = services.BuildServiceProvider();
    }

    [Fact]
    public void TestScoped()
    {
        using var scope = _serviceProvider.CreateScope();
        var options = new AppOptions();
        
        var model = scope.ServiceProvider.GetRequiredService<IViewModelFactory>().Create("testPath", "testCodePath", options);
        model.Should().NotBeNull();
        model.ExportPath.Should().Be("testPath");
    }

    [Fact]
    public void TestSingleton()
    {
        var model = _serviceProvider.GetRequiredService<ISingletonViewModelFactory>().Create(11, "testPath");

        model.Should().NotBeNull();
        model.Length.Should().Be(11);
        model.ExportPath.Should().Be("testPath");
    }
    
    [Fact]
    public void TestSharedFactoryInterface()
    {
        using var scope = _serviceProvider.CreateScope();
        var options = new AppOptions();
        
        var model = scope.ServiceProvider.GetRequiredService<ICommonFactory<CommonViewModel>>().Create(10);
        model.Should().NotBeNull();
        model.Length.Should().Be(10);
        
        var model2 = scope.ServiceProvider.GetRequiredService<ICommonFactory<CommonViewModelDoubled>>().Create(10);
        model2.Should().NotBeNull();
        model2.Length.Should().Be(20);
    }
}
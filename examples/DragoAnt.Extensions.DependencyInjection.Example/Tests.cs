using DragoAnt.Extensions.DependencyInjection.Example.Models;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

namespace DragoAnt.Extensions.DependencyInjection.Example;

internal sealed class IGenericModelWrapper<T>(GenericModel<T> model) : IGenericModel<T>
{
    Guid IGenericModel2<T>.Method2() => model.Method2();
    Guid IGenericModel<T>.Method1() => model.Method1();
}

internal sealed class IGenericModelWrapper2<T>(GenericModel<T> model) : IGenericModel2<T>
{
    Guid IGenericModel2<T>.Method2() => model.Method2();
}

public class ModelTests
{
    private readonly ServiceProvider _serviceProvider;

    public ModelTests()
    {
        IServiceCollection services = new ServiceCollection();

        services.AddExampleDependencies();

        Func<IServiceProvider, object> factory;

        factory = static p => p.GetRequiredService<IBaseInterface>();

        services.Add(new(typeof(IBaseInterface), factory, ServiceLifetime.Scoped));

        //TODO: Move wrappers to generation
        // services.AddScoped(typeof(GenericModel<>)); // Register the implementation.
        // services.AddScoped(typeof(IGenericModel<>), typeof(IGenericModelWrapper<>));
        // services.AddScoped(typeof(IGenericModel2<>), typeof(IGenericModelWrapper2<>));

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

    [Fact]
    public void TestInheritedCommonFactoryInterface()
    {
        using var scope = _serviceProvider.CreateScope();

        var model = scope.ServiceProvider.GetRequiredService<IInheritedCommonFactory<InheritedCommonViewModel>>().Create(10);
        model.Should().NotBeNull();
        model.Length.Should().Be(10);

        var model2 = scope.ServiceProvider.GetRequiredService<IInheritedCommonFactory<InheritedCommonViewModelDoubled>>().Create(10);
        model2.Should().NotBeNull();
        model2.Length.Should().Be(20);
    }

    [Fact]
    public void HierarchyFactoryInterface()
    {
        using var scope = _serviceProvider.CreateScope();

        var simpleViewModelFactory = scope.ServiceProvider.GetRequiredService<IFactory<SimpleViewModel>>();

        var model = simpleViewModelFactory.Create();
        model.Should().NotBeNull();

        var model2 = simpleViewModelFactory.Create(new SimpleModel());
        model2.Should().NotBeNull();

        var complexViewModelFactory = scope.ServiceProvider.GetRequiredService<IFactory<ComplexViewModel>>();

        var model3 = complexViewModelFactory.Create(new ComplexModel(10));
        model3.Should().NotBeNull();

        var act = () => complexViewModelFactory.Create(new SimpleModel());
        act.Should().Throw<InvalidCastException>();

        var act2 = () => complexViewModelFactory.Create();
        act2.Should().Throw<NotSupportedException>();
    }

    [Fact]
    public void GenericFactoryInterface()
    {
        using var scope = _serviceProvider.CreateScope();

        var genericViewModelFactory = scope.ServiceProvider.GetRequiredService<IGenericViewModelFactory>();

        var model = genericViewModelFactory.Create<IGenericModel, GenericModel>(new GenericModel(), new());
        model.Should().NotBeNull();

        var customFactory = scope.ServiceProvider.GetRequiredService<IGenericFactory>();
        var model2 = genericViewModelFactory.Create<IGenericModel, GenericModel>(new GenericModel(), new());
        model2.Should().NotBeNull();
    }

    [Fact(Skip = "Need implementation")]
    public void TestGenericModels()
    {
        //TODO: Move wrappers to generation

        using var scope = _serviceProvider.CreateScope();

        var model = scope.ServiceProvider.GetRequiredService<IGenericModel<int>>();
        model.Should().NotBeNull();

        var model2 = scope.ServiceProvider.GetRequiredService<IGenericModel2<int>>();
        model.Should().NotBeNull();

        model.Method2().Should().Be(model2.Method2());

        var model3 = scope.ServiceProvider.GetRequiredService<IMultiGenericModel<int, string>>();
        model3.Should().NotBeNull();
    }

    [Fact]
    public void TestHandGenericModels()
    {
        using var scope = _serviceProvider.CreateScope();

        var model = scope.ServiceProvider.GetRequiredService<IHandGenericModel<int>>();
        model.Should().NotBeNull();

        var model2 = scope.ServiceProvider.GetRequiredService<HandGenericModel<int>>();
        model2.Should().NotBeNull();

        model.Method1().Should().Be(model2.Method1());
    }

    [Fact]
    public void Dependencies()
    {
        //TODO: Tests for Dependencies
    }
}
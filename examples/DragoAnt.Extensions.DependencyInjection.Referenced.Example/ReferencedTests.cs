﻿using DragoAnt.Extensions.DependencyInjection.Example;
using DragoAnt.Extensions.DependencyInjection.Example.Models;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

namespace DragoAnt.Extensions.DependencyInjection.Referenced.Example;

public class ReferencedTests
{
    private readonly ServiceProvider _serviceProvider;

    public ReferencedTests()
    {
        var services = new ServiceCollection();

        services.AddDependencies();
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
    public void OwnDependencies()
    {
        using var scope = _serviceProvider.CreateScope();

        var model = scope.ServiceProvider.GetRequiredService<IOwnModel>();
        model.Should().NotBeNull();

        var model2 = _serviceProvider.GetRequiredService<SelfOwnModel>();
        model2.Should().NotBeNull();
    }
}
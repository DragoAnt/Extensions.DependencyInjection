﻿namespace DragoAnt.Extensions.DependencyInjection.Example.Models;

[AsResolveFactoryParameter]
public interface IModel
{
}

public class SimpleModel : IModel
{
}

public class ComplexModel : IModel
{
    public int Value { get; }

    public ComplexModel(int value)
    {
        Value = value;
    }
}

public interface IFactory<out T>
    where T : IViewModel
{
    T Create();
    T Create(IModel model);
}

public interface IAdvFactory<out T>
    where T : IViewModel
{
    T Create();
    T Create(IModel model);
}

public interface IViewModel
{
}

[ResolveFactory(SkipGenerateInterface = true)]
[ResolveFactoryContract(typeof(IFactory<>), AllowNotSupportedMethods = true)]
[ResolveFactoryContract(typeof(IAdvFactory<>), AllowNotSupportedMethods = true)]
public abstract class SuperViewModel<TModel> : IViewModel
    where TModel : IModel
{
    protected SuperViewModel(TModel model)
    {
        Model = model;
    }

    public TModel Model { get; }
}

public abstract class BaseViewModel<TModel> : SuperViewModel<TModel>
    where TModel : IModel
{
    protected BaseViewModel(TModel model)
        : base(model)
    {
    }
}

public sealed class SimpleViewModel : BaseViewModel<SimpleModel>
{
    public SimpleViewModel()
        : this(new SimpleModel())
    {
    }

    public SimpleViewModel(SimpleModel model)
        : base(model)
    {
    }
}

public sealed class ComplexViewModel : BaseViewModel<ComplexModel>
{
    public ComplexViewModel(ComplexModel model)
        : base(model)
    {
    }
}
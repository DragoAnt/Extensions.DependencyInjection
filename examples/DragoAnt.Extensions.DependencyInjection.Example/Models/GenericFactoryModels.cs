namespace DragoAnt.Extensions.DependencyInjection.Example.Models;

[ResolveFactory]
[ResolveFactoryContract(typeof(IGenericFactory))]
public sealed class GenericViewModel<T, T2> : IGenericViewModel
    where T : class
    where T2 : class, T, IGenericModel, new()
{
    public GenericViewModel(
        T exportPath,
        T2? exportPath2)
    {
    }

    public GenericViewModel(
        string exportPath,
        T customCodePath,
        T2 testT2,
        ITestService serviceInterface,
        ISingletonViewModelFactory singletonViewModelFactory,
        [ResolveFactoryParameter] AppOptions? options = null)
    {
        ExportPath = exportPath;
        //...
    }

    public string ExportPath { get; }
}

public interface IGenericViewModel
{
}

public interface IGenericFactory
{
    IGenericViewModel Create<T, T2>(T exportPath, T2? exportPath2)
        where T : class
        where T2 : class, T, IGenericModel, new();
}

public interface IGenericModel
{
}

public class GenericModel : IGenericModel
{
}
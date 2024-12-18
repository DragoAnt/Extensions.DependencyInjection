namespace DragoAnt.Extensions.DependencyInjection.Example.Models;

//TODO: Implement wrappers and uncomment
//[ResolveDependency]
internal sealed class GenericModel<T> : IGenericModel<T>
{
    private readonly Guid _guid = Guid.NewGuid();

    public Guid Method2() => _guid;

    public Guid Method1() => _guid;
}

//TODO: Implement wrappers and uncomment
//[ResolveDependency]
public interface IGenericModel<T> : IGenericModel2<T>
{
    Guid Method1();
}

//TODO: Implement wrappers and uncomment
//[ResolveDependency]
public interface IGenericModel2<T>
{
    Guid Method2();
}

internal sealed class MultiGenericModel<T, T2> : IMultiGenericModel<T, T2>
{
}

[ResolveDependency]
public interface IMultiGenericModel<T, T2> // : IGenericModel<T> // this is illegal because of different count of type arguments
{
}
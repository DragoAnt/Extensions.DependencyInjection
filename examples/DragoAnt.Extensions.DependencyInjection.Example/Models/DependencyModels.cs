namespace DragoAnt.Extensions.DependencyInjection.Example.Models;

public abstract class DepModelBase
{
    public Guid GetGuid() => Guid.NewGuid();
}

[ResolveDependency]
public sealed partial class ScopedDepModel : DepModelBase
{
}

partial class ScopedDepModel
{
}

[ResolveDependency(ResolveDependencySingleton)]
public sealed partial class SingletonDepModel : DepModelBase
{
}

[ResolveDependency]
public abstract class BaseHierarchyDepModel : DepModelBase, IHierarchyDepModel
{
}

[ResolveDependency(ResolveDependencySingleton)]
public interface IHierarchyDepModel
{
}

public class HierarchyDepModel : BaseHierarchyDepModel
{
}

[ResolveDependency(ResolveDependencySingleton)]
public class SingletonHierarchyDepModel : BaseHierarchyDepModel
{
}

public class GenericInterfaceDepModel : BaseHierarchyDepModel, IGenericInterfaceDepModel<int>
{
    public int Get() => 1;
}

[ResolveDependency(ResolveDependencySingleton)]
public interface IGenericInterfaceDepModel<out T>
{
    T Get();
}
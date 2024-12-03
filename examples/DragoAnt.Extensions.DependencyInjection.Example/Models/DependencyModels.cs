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
public abstract class BaseHierarchyDepModel : DepModelBase, IHierarchyDepModel, ILevel1Interface
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

public partial class GenericInterfaceDepModel : BaseHierarchyDepModel, IGenericInterfaceDepModel<int>
{
    public int Get() => 1;
}

partial class GenericInterfaceDepModel
{
}

[ResolveDependency(ResolveDependencySingleton)]
public interface IGenericInterfaceDepModel<out T> : ILevel1Interface
{
    T Get();
}

[ResolveDependency(ResolveDependencySingleton)]
public interface ILevel1Interface : IBaseInterface
{
}

[ResolveDependency(ResolveDependencySingleton)]
public interface IBaseInterface
{
}

[ResolveDependency]
public class SelfRegistration
{
}

[ResolveDependency(CustomFactoryMethodName = "GetSelfCustomFactoryRegistration")]
public class SelfCustomFactoryRegistration
{
}

[ResolveFactory]
[ResolveDependency(CustomFactoryMethodName = "GetDbContext")]
public class DbContext(string connString) : IBaseInterface
{
    public string ConnString { get; } = connString;
}
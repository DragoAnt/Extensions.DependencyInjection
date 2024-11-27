using static DragoAnt.Extensions.DependencyInjection.ResolveDependencyServiceLifetime;

namespace DragoAnt.Extensions.DependencyInjection.Example.Models;

public abstract class DepModelBase
{
   public Guid GetGuid() => Guid.NewGuid(); 
}

[ResolveDependency]
public sealed partial class ScopedDepModel: DepModelBase
{
}

partial class ScopedDepModel
{
}

[ResolveDependency(Singleton)]
public sealed partial class SingletonDepModel:DepModelBase
{
    
}

[ResolveDependency(Transient)]
public sealed partial class TransientDepModel:DepModelBase
{
}

[ResolveDependency(Transient)]
public abstract class BaseHierarchyDepModel : DepModelBase, IHierarchyDepModel
{
}

[ResolveDependency(Singleton)]
public interface IHierarchyDepModel
{
}

public class HierarchyDepModel : BaseHierarchyDepModel
{
}

[ResolveDependency(Singleton)]
public class SingletonHierarchyDepModel : BaseHierarchyDepModel
{
}
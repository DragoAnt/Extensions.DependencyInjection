namespace DragoAnt.Extensions.DependencyInjection.Referenced.Example;

public class OwnModel : IOwnModel
{
}

[ResolveDependency]
public interface IOwnModel
{
}

[ResolveDependency(ResolveDependencySingleton)]
public class SelfOwnModel
{
}
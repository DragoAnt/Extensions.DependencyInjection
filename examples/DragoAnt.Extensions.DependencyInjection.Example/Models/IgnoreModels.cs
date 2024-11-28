namespace DragoAnt.Extensions.DependencyInjection.Example.Models;

[ResolveDependencyIgnore]
public class IgnoreViewModel2 : IViewModel2
{
}

public class ViewModel2 : IViewModel2
{
}

[ResolveDependency]
public interface IViewModel2
{
}
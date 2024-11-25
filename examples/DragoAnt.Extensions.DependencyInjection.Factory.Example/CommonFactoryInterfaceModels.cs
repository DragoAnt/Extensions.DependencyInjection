namespace DragoAnt.Extensions.DependencyInjection.Factory.Example;

public interface ICommonFactory<out T>
{
    T Create(int defaultLen, double defaultDblLen);
    T Create(int length, int defaultLen = 100, double defaultDblLen = 10.01);
}

[ResolveFactory(SharedFactoryInterfaceTypeDefinition = typeof(ICommonFactory<>))]
public sealed class CommonViewModel
{
    public int Length { get; }
    public TestService Service { get; }


    public CommonViewModel(
        TestService service,
        int defaultLen = 100,
        double defaultDblLen = 10.01)
        : this(0, service, defaultLen, defaultDblLen)
    {
    }

    public CommonViewModel(
        int length,
        TestService service,
        int defaultLen = 100,
        double defaultDblLen = 10.01)
    {
        Length = length;
        Service = service;
    }
}

[ResolveFactory(SharedFactoryInterfaceTypeDefinition = typeof(ICommonFactory<>))]
public sealed class CommonViewModelDoubled
{
    public int Length { get; }
    public TestService Service { get; }


    public CommonViewModelDoubled(
        TestService service,
        int defaultLen = 100,
        double defaultDblLen = 10.01)
        : this(0, service, defaultLen, defaultDblLen)
    {
    }

    public CommonViewModelDoubled(
        int length,
        TestService service,
        int defaultLen = 100,
        double defaultDblLen = 10.01)
    {
        Length = length * 2;
        Service = service;
    }
}
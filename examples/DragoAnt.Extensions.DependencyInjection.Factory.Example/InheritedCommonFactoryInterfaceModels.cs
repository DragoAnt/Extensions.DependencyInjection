namespace DragoAnt.Extensions.DependencyInjection.Factory.Example;

public interface IInheritedCommonFactory<out T>
    where T : InheritedCommonViewModelBase
{
    T Create(int defaultLen, double defaultDblLen);
    T Create(int length, int defaultLen = 100, double defaultDblLen = 10.01);
}

public sealed class InheritedCommonViewModel : InheritedCommonViewModelBase
{
    public InheritedCommonViewModel(TestService service, int defaultLen = 100, double defaultDblLen = 10.01)
        : base(service, defaultLen, defaultDblLen)
    {
    }

    public InheritedCommonViewModel(int length, TestService service, int defaultLen = 100, double defaultDblLen = 10.01)
        : base(length, service, defaultLen, defaultDblLen)
    {
    }
}

[ResolveFactory]
[ResolveFactoryContract(typeof(IInheritedCommonFactory<>))]
public abstract class InheritedCommonViewModelBase
{
    public int Length { get; }
    public TestService Service { get; }


    public InheritedCommonViewModelBase(
        TestService service,
        int defaultLen = 100,
        double defaultDblLen = 10.01)
        : this(0, service, defaultLen, defaultDblLen)
    {
    }

    public InheritedCommonViewModelBase(
        int length,
        TestService service,
        int defaultLen = 100,
        double defaultDblLen = 10.01)
    {
        Length = length;
        Service = service;
    }
}

public sealed class InheritedCommonViewModelDoubled : InheritedCommonViewModelBase
{
    public InheritedCommonViewModelDoubled(TestService service, int defaultLen = 100, double defaultDblLen = 10.01)
        : base(service, defaultLen, defaultDblLen)
    {
    }

    public InheritedCommonViewModelDoubled(int length, TestService service, int defaultLen = 100, double defaultDblLen = 10.01)
        : base(length * 2, service, defaultLen, defaultDblLen)
    {
    }
}
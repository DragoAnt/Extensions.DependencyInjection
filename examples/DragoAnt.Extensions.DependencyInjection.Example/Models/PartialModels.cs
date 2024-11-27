namespace DragoAnt.Extensions.DependencyInjection.Example.Models;

public interface IPartialFactory<out T>
{
    T Create();
    T Create(string exportPath);
}

[ResolveFactory(SkipGenerateInterface = true)]
[ResolveFactoryContract(typeof(IPartialFactory<>), AllowNotSupportedMethods = true)]
public sealed partial class PartialViewModel
{
    public PartialViewModel(
        string exportPath)
    {
    }

    public PartialViewModel(
        string exportPath,
        string? exportPath2,
        int i,
        int? i2,
        Guid guid,
        Guid? guid2,
        DateTime dt,
        DateTime? dt2,
        DateTimeOffset dto,
        DateTimeOffset? dto2,
        DateTimeOffset? dto3,
        ITestService serviceInterface,
        TestService serviceClass,
        IEnumerable<ITestService> serviceInterfaces,
        IEnumerable<TestService> serviceClasses,
        ISingletonViewModelFactory singletonViewModelFactory,
        bool test = false,
        int defaultLen = 100,
        double defaultDblLen = 10.01,
        [ResolveFactoryParameter] AppOptions? options = null)
    {
    }
}

partial class PartialViewModel
{
}
namespace DragoAnt.Extensions.DependencyInjection.Factory.Example;

[ResolveFactory]
public sealed class ViewModel
{
    public ViewModel(
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
        ExportPath = exportPath;
        //...
    }

    public ViewModel(
        string exportPath,
        string customCodePath,
        ITestService serviceInterface,
        ISingletonViewModelFactory singletonViewModelFactory,
        [ResolveFactoryParameter] AppOptions? options = null)
    {
        ExportPath = exportPath;
        //...
    }

    public string ExportPath { get; }
}

[ResolveFactory(ResolveFactoryServiceLifetime.Singleton)]
public sealed class SingletonViewModel
{
    public int Length { get; }
    public string ExportPath { get; }
    public TestService Service { get; }

    public SingletonViewModel(
        int length,
        string exportPath,
        TestService service)
    {
        Length = length;
        ExportPath = exportPath;
        Service = service;
    }
}
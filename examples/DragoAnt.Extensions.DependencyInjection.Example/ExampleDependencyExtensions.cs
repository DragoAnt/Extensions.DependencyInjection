using Microsoft.Extensions.DependencyInjection;

namespace DragoAnt.Extensions.DependencyInjection.Example;

partial class ExampleDependencyExtensions
{
    private static partial void AddCustomDependencies(IServiceCollection services)
    {
        services.AddSingleton<TestService>();
        services.AddScoped<ITestService>(provider => provider.GetRequiredService<TestService>());
    }
}
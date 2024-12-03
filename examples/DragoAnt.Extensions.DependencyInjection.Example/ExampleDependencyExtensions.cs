using DragoAnt.Extensions.DependencyInjection.Example.Models;
using Microsoft.Extensions.DependencyInjection;

namespace DragoAnt.Extensions.DependencyInjection.Example;

partial class ExampleDependencyExtensions
{
    private static partial void AddCustomDependencies(IServiceCollection services)
    {
        services.AddSingleton<TestService>();
        services.AddScoped<ITestService>(provider => provider.GetRequiredService<TestService>());
    }

    private static partial SelfCustomFactoryRegistration GetSelfCustomFactoryRegistration(IServiceProvider provider)
    {
        throw new NotImplementedException();
    }

    private static partial DbContext GetDbContext(IServiceProvider provider)
    {
        return provider.GetRequiredService<IDbContextFactory>().Create("Conn string");
    }
}
using Microsoft.Extensions.DependencyInjection;
using static Microsoft.Extensions.DependencyInjection.ServiceLifetime;

namespace DragoAnt.Extensions.DependencyInjection.Example.Models;

[ResolveDependency]
internal sealed class HandGenericModel<T>
{
    private readonly Guid _guid = Guid.NewGuid();

    public Guid Method1() => _guid;
}

[ResolveDependency]
public interface IHandGenericModel<T>
{
    Guid Method1();
}

internal class IHandGenericModelWrapper<T>(HandGenericModel<T> model) : IHandGenericModel<T>
{
    Guid IHandGenericModel<T>.Method1() => model.Method1();
}
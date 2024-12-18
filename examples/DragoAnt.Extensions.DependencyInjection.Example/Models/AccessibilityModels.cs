namespace DragoAnt.Extensions.DependencyInjection.Example.Models;

[ResolveFactory]
internal sealed class InternalViewModel
{
    public InternalViewModel(string test)
    {
    }
}

[ResolveFactory]
public sealed class PublicViewModel
{
    public PublicViewModel(string test)
    {
    }
}
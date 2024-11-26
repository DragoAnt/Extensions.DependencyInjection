namespace DragoAnt.Extensions.DependencyInjection;

internal readonly struct DependencyItem(DependencyModel? dependency, FactoryModel? factory, Exception? exception = null)
{
    public DependencyModel? Dependency { get; } = dependency;
    public FactoryModel? Factory { get; } = factory;

    public bool IsInvalid => exception is not null;

    public string GetError()
    {
        if (exception is not null)
        {
            return $"Exception: {exception.Message}, InnerException: {exception.InnerException?.Message}";
        }

        return string.Empty;
    }
}
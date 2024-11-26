namespace DragoAnt.Extensions.DependencyInjection.Factory;

internal sealed class FactoryGeneratorException : Exception
{
    public FactoryGeneratorException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
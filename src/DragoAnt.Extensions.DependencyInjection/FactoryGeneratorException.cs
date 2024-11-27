namespace DragoAnt.Extensions.DependencyInjection;

internal sealed class FactoryGeneratorException : Exception
{
    public FactoryGeneratorException(string message, Exception? innerException = null)
        : base(message, innerException)
    {
    }
}
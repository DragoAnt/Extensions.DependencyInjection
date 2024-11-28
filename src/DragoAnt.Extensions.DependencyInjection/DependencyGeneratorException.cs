namespace DragoAnt.Extensions.DependencyInjection;

internal sealed class DependencyGeneratorException : Exception
{
    public DependencyGeneratorException(string message, Exception? innerException = null)
        : base(message, innerException)
    {
    }
}
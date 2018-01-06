namespace Pigeon.Fluent.Transport
{
    public interface ISenderSetup
    {
        ISenderSetup For<TRequest>();
    }
}

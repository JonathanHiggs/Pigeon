namespace Pigeon.Fluent.Transport
{
    public interface ISubscriberSetup
    {
        ISubscriberSetup Handles<TTopic>();
    }
}

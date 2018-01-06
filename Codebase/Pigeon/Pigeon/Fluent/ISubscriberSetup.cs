namespace Pigeon.Fluent
{
    public interface ISubscriberSetup
    {
        ISubscriberSetup Handles<TTopic>();
    }
}

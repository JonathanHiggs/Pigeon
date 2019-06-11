namespace Pigeon.Topics
{
    public interface IDITopicDispatcher : ITopicDispatcher
    {
        void Register<TTopic, THandler>()
            where THandler : ITopicHandler<TTopic>;
    }
}

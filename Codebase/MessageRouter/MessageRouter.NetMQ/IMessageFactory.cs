using MessageRouter.Packages;
using NetMQ;

namespace MessageRouter.NetMQ
{
    public interface IMessageFactory
    {
        NetMQMessage CreateTopicMessage(Package package);
        Package ExtractTopicPackage(NetMQMessage message);
        NetMQMessage CreateRequestMessage(Package package);
        Package ExtractResponsePackage(NetMQMessage message);
        Package ExtractRequestPackage(NetMQMessage message);
        NetMQMessage CreateResponseMessage(Package package, NetMQMessage requestMessage);
        bool ValidRequestMessage(NetMQMessage requestMessage);
    }
}
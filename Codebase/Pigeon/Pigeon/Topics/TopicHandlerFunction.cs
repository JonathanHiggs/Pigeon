using System.Threading.Tasks;

namespace Pigeon.Topics
{
    /// <summary>
    /// Delegate for handling topic messages
    /// </summary>
    /// <param name="message">Topic message</param>
    public delegate void TopicHandlerFunction(object message);
}

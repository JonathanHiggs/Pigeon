namespace MessageRouter.Topics
{
    /// <summary>
    /// Delegate for handling topic messages
    /// </summary>
    /// <param name="message">Topic message</param>
    internal delegate void TopicHandlerFunction(object message);
}

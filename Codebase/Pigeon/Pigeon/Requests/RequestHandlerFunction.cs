namespace Pigeon.Requests
{
    /// <summary>
    /// Delegate for handling responses to requests
    /// </summary>
    /// <param name="request">Request message</param>
    /// <returns>Response message</returns>
    internal delegate object RequestHandlerFunction(object request);
}

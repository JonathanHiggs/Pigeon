namespace Pigeon.Requests
{
    /// <summary>
    /// Type safe delegate for handling responses to requests
    /// </summary>
    /// <typeparam name="TRequest">Type of request object</typeparam>
    /// <typeparam name="TResponse">Type of response object</typeparam>
    /// <param name="request">Request object</param>
    /// <returns>Response object</returns>
    public delegate TResponse RequestHandlerDelegate<TRequest, TResponse>(TRequest request);
}

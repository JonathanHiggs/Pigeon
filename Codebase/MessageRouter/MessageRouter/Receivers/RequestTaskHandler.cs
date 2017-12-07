using System;

namespace MessageRouter.Receivers
{
    /// <summary>
    /// Represents the method that will handle a <see cref="IReceiver"/>s RequestReceived event
    /// </summary>
    /// <param name="raisingReceiver">The source <see cref="IReceiver"/></param>
    /// <param name="requestTask">The <see cref="RequestTask"/> that combines the incoming request data and a response handler</param>
    public delegate void RequestTaskHandler(IReceiver raisingReceiver, RequestTask requestTask);
}
using Pigeon.Receivers;

namespace Pigeon.Web
{
    /// <summary>
    /// Interface that encapsulates a Web <see cref="IReceiver"/> that is able to bind to <see cref="IAddress"/>es to listen
    /// for and respond to incoming http requests
    /// </summary>
    public interface IWebReceiver : IReceiver
    {
        /// <summary>
        /// Gets the <see cref="WebTaskHandler"/> delegate that the <see cref="IWebReceiver"/> calls upon receiving an incoming
        /// http request
        /// </summary>
        WebTaskHandler Handler { get; }
    }
}

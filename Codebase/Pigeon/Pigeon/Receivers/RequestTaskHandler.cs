using System.Threading.Tasks;

namespace Pigeon.Receivers
{
    /// <summary>
    /// A method that can perform the handling of incoming request messages
    /// </summary>
    /// <param name="requestTask">The <see cref="RequestTask"/> that combines the incoming request data and a response handler</param>
    public delegate void RequestTaskHandler(ref RequestTask requestTask);
}
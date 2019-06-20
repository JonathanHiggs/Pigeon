using System.Threading.Tasks;

namespace Pigeon.Receivers
{
    public delegate Task AsyncRequestTaskHandler(IReceiver receiver, AsyncRequestTask requestTask);
}

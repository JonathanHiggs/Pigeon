using MessageRouter.Addresses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageRouter.Senders
{
    /// <summary>
    /// Manages the state of <see cref="ISender"/>s and resolves a sender for a given request object type
    /// </summary>
    public interface ISenderMonitor
    {
        void Add(ISender sender);


        /// <summary>
        /// Starts the <see cref="SenderMonitor"/> running
        /// </summary>
        void Start();


        /// <summary>
        /// Stops the <see cref="SenderMonitor"/> running and disconnects <see cref="ISender"/>s
        /// </summary>
        void Stop();
    }
}

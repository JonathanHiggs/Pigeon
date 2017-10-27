using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageRouter.Senders
{
    /// <summary>
    /// Manages the state of <see cref="ISender"/>s
    /// </summary>
    public interface ISenderManager
    {
        /// <summary>
        /// Adds a 
        /// </summary>
        /// <typeparam name="TSend"></typeparam>
        /// <param name="address"></param>
        void Add<TSend>(IAddress address);


        /// <summary>
        /// Resolves a <see cref="ISender"/> for the type of the request with the configured routing
        /// </summary>
        /// <typeparam name="TSend">Request type</typeparam>
        /// <returns>Sender for the request type</returns>
        ISender SenderFor<TSend>();

        
        /// <summary>
        /// Initializes connections to remotes
        /// </summary>
        void Start();

        
        /// <summary>
        /// Terminates connections to remotes
        /// </summary>
        void Stop();
    }
}

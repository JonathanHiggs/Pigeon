using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessageRouter.Addresses;

namespace MessageRouter
{
    public interface IEndPoint
    {
        /// <summary>
        /// Gets an enumerable of <see cref="IAddress"/> that the receiver is listening to
        /// </summary>
        IEnumerable<IAddress> Addresses { get; }
    }
}

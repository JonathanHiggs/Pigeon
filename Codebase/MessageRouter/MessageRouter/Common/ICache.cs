using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageRouter.Common
{
    /// <summary>
    /// Common interface for <see cref="IConnection"/> caches
    /// </summary>
    /// <typeparam name="TConnection"></typeparam>
    public interface ICache<TConnection> where TConnection : IConnection
    { }
}

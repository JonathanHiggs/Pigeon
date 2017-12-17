using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageRouter.Common
{
    /// <summary>
    /// Common interface for <see cref="IEndPoint"/> caches
    /// </summary>
    /// <typeparam name="TEndPoint"></typeparam>
    public interface ICache<TEndPoint> where TEndPoint : IEndPoint
    { }
}

namespace Pigeon.Common
{
    /// <summary>
    /// Common interface for <see cref="IConnection"/> caches
    /// </summary>
    /// <typeparam name="TConnection"></typeparam>
    public interface ICache<TConnection> where TConnection : IConnection
    { }
}
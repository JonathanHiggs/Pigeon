namespace Pigeon.Fluent
{
    public interface IRouterBuilder
    {
        Router Build();
        Router BuildAndStart();
    }
}

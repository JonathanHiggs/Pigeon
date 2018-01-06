namespace Pigeon.Unity
{
    public interface INamedBuilder<TBuilder>
    {
        TBuilder WithName(string name);
    }
}

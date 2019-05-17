using Pigeon.Contrib.Meta.Describe.v1_0;
using Pigeon.Fluent;

namespace Pigeon.Contrib
{
    public static class BuilderExtensions
    {
        public static IHandlerBuilder WithContribHandlers(this IHandlerBuilder handlerBuilder)
        {
            handlerBuilder.WithHandlers(c => 
            {
                c.WithRequestHandler<DescribeRouter, RouterDescription, DescribeRouterHandler>();
            });

            return handlerBuilder;
        }
    }
}

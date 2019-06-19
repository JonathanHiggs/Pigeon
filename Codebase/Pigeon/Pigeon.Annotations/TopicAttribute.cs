using System;

namespace Pigeon.Annotations
{
    /// <summary>
    /// Specifies the annotated type as a pubsub topic
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class TopicAttribute : Attribute
    { }
}

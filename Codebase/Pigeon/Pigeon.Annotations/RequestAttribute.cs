using System;

namespace Pigeon.Annotations
{
    /// <summary>
    /// Specifies the response type associated with the annotated request type
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class RequestAttribute : Attribute
    {
        /// <summary>
        /// Gets the associated response type
        /// </summary>
        public Type ResponseType { get; set; }
    }
}

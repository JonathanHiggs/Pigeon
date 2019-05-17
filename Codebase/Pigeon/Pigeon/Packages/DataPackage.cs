using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pigeon.Packages
{
    /// <summary>
    /// <see cref="Package"/> derivative for returning response data
    /// </summary>
    /// <typeparam name="T">Type of data payload</typeparam>
    [Serializable]
    [ImmutableObject(true)]
    public class DataPackage<T> : Package
        where T : class
    {
        /// <summary>
        /// Stores a readonly reference to the package data
        /// </summary>
        public readonly T Data;


        /// <summary>
        /// Initializes an instance of <see cref="DataPackage{T}"/>
        /// </summary>
        /// <param name="id">Package indentifer</param>
        /// <param name="data">Package body and data</param>
        public DataPackage(IPackageId id, T data)
            : base(id)
        {
            Data = data ?? throw new ArgumentNullException(nameof(data));
        }


        /// <summary>
        /// Gets the package body
        /// </summary>
        public override object Body => Data;
    }
}

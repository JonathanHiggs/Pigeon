﻿using System;
using System.ComponentModel;

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
        /// Stores a read-only reference to the package data
        /// </summary>
        public readonly T Data;


        /// <summary>
        /// Initializes an instance of <see cref="DataPackage{T}"/>
        /// </summary>
        /// <param name="id">Package identifier</param>
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

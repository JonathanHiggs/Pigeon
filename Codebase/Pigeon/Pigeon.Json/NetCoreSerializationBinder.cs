﻿using System;
using System.Collections.Concurrent;
using System.Text.RegularExpressions;

using Newtonsoft.Json.Serialization;

namespace Pigeon.Json
{
    /// <summary>
    /// Rebinds .Net Core bindings to .Net Framework bindings
    /// </summary>
    public class NetCoreSerializationBinder : DefaultSerializationBinder
    {
        private static readonly Regex regex = new Regex(
            @"System\.Private\.CoreLib(, Version=[\d\.]+)?(, Culture=[\w-]+)(, PublicKeyToken=[\w\d]+)?");

        private static readonly ConcurrentDictionary<Type, (string assembly, string type)> cache =
            new ConcurrentDictionary<Type, (string, string)>();


        /// <summary>
        /// Rebinds .Net Core bindings to .Net Framework bindings
        /// </summary>
        /// <param name="serializedType">The type of the object the formatter creates a new instance of</param>
        /// <param name="assemblyName">Specifies the System.Reflection.Assembly name of the serialized object</param>
        /// <param name="typeName">Specifies the System.Type name of the serialized object</param>
        public override void BindToName(Type serializedType, out string assemblyName, out string typeName)
        {
            base.BindToName(serializedType, out assemblyName, out typeName);

            if (cache.TryGetValue(serializedType, out var name))
            {
                assemblyName = name.assembly;
                typeName = name.type;
            }
            else
            {
                if (assemblyName.Contains("System.Private.CoreLib"))
                    assemblyName = regex.Replace(assemblyName, "mscorlib");

                if (typeName.Contains("System.Private.CoreLib"))
                    typeName = regex.Replace(typeName, "mscorlib");

                cache.TryAdd(serializedType, (assemblyName, typeName));
            }
        }
    }
}

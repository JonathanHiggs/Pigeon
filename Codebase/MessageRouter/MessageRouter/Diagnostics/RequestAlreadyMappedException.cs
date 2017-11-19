using MessageRouter.Addresses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageRouter.Diagnostics
{
    [Serializable]
    public class RequestAlreadyMappedException : MessageRouterException
    {
        private readonly Type newRequestType;
        private readonly IAddress newAddress;
        private readonly Type existingRequestType;
        private readonly IAddress existingAddress;


        public Type NewRequestType => newRequestType;


        public IAddress NewAddress => newAddress;


        public Type ExistingRequestType => existingRequestType;


        public IAddress ExistingAddress => existingAddress;

        
        public RequestAlreadyMappedException(Type newRequestType, IAddress newAddress, Type existingRequestType, IAddress existingAddress)
            : this(newRequestType, newAddress, existingRequestType, existingAddress, $"New mapping {newRequestType.Name}->{newAddress.ToString()} already registered with {existingRequestType.Name}->{existingAddress.ToString()}", null)
        { }


        public RequestAlreadyMappedException(Type newRequestType, IAddress newAddress, Type existingRequestType, IAddress existingAddress, string message) 
            : this(newRequestType, newAddress, existingRequestType, existingAddress, message, null)
        { }


        public RequestAlreadyMappedException(Type newRequestType, IAddress newAddress, Type existingRequestType, IAddress existingAddress, string message, Exception inner) 
            : base(message, inner)
        {
            this.newRequestType = newRequestType;
            this.newAddress = newAddress;
            this.existingRequestType = existingRequestType;
            this.existingAddress = existingAddress;
        }


        protected RequestAlreadyMappedException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}

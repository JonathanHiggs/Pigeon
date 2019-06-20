﻿using System;
using System.ComponentModel;

using ExampleContracts.Models;
using ExampleContracts.Responses;

using Pigeon.Annotations;

namespace ExampleContracts.Requests
{
    [Serializable]
    [ImmutableObject(true)]
    [Request(ResponseType = typeof(Response<User>))]
    public class UserDisconecting
    {
        public UserDisconecting(User user) =>
            User = user;


        public User User { get; }
    }
}

using System;
using System.Net;
using System.Text.RegularExpressions;

using Pigeon.Serialization;

namespace Pigeon.Web
{
    public abstract class WebRouting
    {
        public abstract Regex UrlPattern { get; }


        public bool Matches(HttpListenerRequest request) =>
            UrlPattern.IsMatch(request.Url.LocalPath);


        public abstract Type RequestType { get; }


        public abstract object ExtractRequest(HttpListenerRequest request, ISerializer serializer);
    }
}
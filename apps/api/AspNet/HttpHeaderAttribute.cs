using System;
using Microsoft.AspNetCore.Mvc.ActionConstraints;

namespace Amphora.Api.AspNet
{
    public class HttpHeaderAttribute : Attribute, IActionConstraint
    {
        public string Header { get; set; }
        public string Value { get; set; }

        public HttpHeaderAttribute(string header, string value)
        {
            Header = header;
            Value = value;
        }

        public virtual bool Accept(ActionConstraintContext context)
        {
            if (context.RouteContext.HttpContext.Request.Headers.TryGetValue(Header, out var value))
            {
                return value[0] == Value;
            }

            return false;
        }

        public int Order => 0;
    }
}
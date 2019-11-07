using System;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Amphora.Api.AspNet
{
    public class AddJsonContentType : Attribute, IResourceFilter
    {
        public void OnResourceExecuting(ResourceExecutingContext context)
        {
            context.HttpContext.Request.Headers["Content-Type"] = "application/json";

        }

        public void OnResourceExecuted(ResourceExecutedContext context)
        {
        }
    }
}
namespace Amphora.Api.AspNet
{
    public class ApiMajorVersionAttribute : HttpHeaderAttribute
    {
        public ApiMajorVersionAttribute(int majorVersion) : base(ApiVersion.HeaderName, majorVersion.ToString())
        {
        }

        public override bool Accept(Microsoft.AspNetCore.Mvc.ActionConstraints.ActionConstraintContext context)
        {
            if (context.RouteContext.HttpContext.Request.Headers.ContainsKey(ApiVersion.HeaderName))
            {
                return base.Accept(context);
            }
            else
            {
                // be forgiving to client who don't pass the version
                return true;
            }
        }
    }
}

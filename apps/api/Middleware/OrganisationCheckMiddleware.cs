using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Amphora.Api.Middleware
{
    public class OrganisationCheckMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<OrganisationCheckMiddleware> logger;

        public OrganisationCheckMiddleware(RequestDelegate next, ILogger<OrganisationCheckMiddleware> logger)
        {
            _next = next;
            this.logger = logger;
        }

        private const string CreateOrgPath = "/Organisations/Create";
        private const string JoinOrgPath = "/Organisations/Join";
        private const string RequestJoinOrgPath = "/Organisations/RequestToJoin";
        private const string FindOrgPath = "/Organisations";
        private const string OrgDetailPath = "/Organisations/Detail";
        private const string ConfirmEmailPath = "/Profiles/Account/ConfirmEmail";
        private const string LocationSearchPath = "/Market/LocationSearch";
        private const string QueryString = "?message=You must belong to an Organisation to continue";
        public async Task Invoke(HttpContext httpContext, IUserService userService, IOrganisationService organisationService)
        {
            var user = await userService.ReadUserModelAsync(httpContext.User);
            if (user != null // don't redirect if there's no user
                 && user.OrganisationId == null // only redirect if user doesn't have an org
                 && !string.Equals(CreateOrgPath, httpContext.Request.Path) // don't redirect if we're there already
                 && !string.Equals(JoinOrgPath, httpContext.Request.Path) // and don't redirect if we're trying to join
                 && !string.Equals(RequestJoinOrgPath, httpContext.Request.Path) // and don't redirect if we're trying to join another
                 && !string.Equals(FindOrgPath, httpContext.Request.Path) // and don't redirect if we're trying to find our org
                 && !string.Equals(OrgDetailPath, httpContext.Request.Path) // and don't redirect if we're trying to find our org
                 && !string.Equals(ConfirmEmailPath, httpContext.Request.Path) // and don't redirect if we're trying confirm email
                 && !string.Equals(LocationSearchPath, httpContext.Request.Path) // and don't redirect the search bar for locations
                 && !httpContext.Request.Path.StartsWithSegments("/api")) // don't redirect API calls
            {
                logger.LogInformation($"Redirecting {user.UserName} to {CreateOrgPath}");
                httpContext.Response.Redirect($"{CreateOrgPath}{QueryString}");
            }

            await _next(httpContext);
        }
    }
}
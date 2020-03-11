using System;
using System.Linq;
using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Common.Contracts;
using Amphora.Common.Extensions;
using Amphora.Common.Models.Users;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Amphora.Api.Middleware
{
    public class OrganisationCheckMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<OrganisationCheckMiddleware> logger;

        public OrganisationCheckMiddleware(RequestDelegate next,
                                           ILogger<OrganisationCheckMiddleware> logger)
        {
            _next = next;
            this.logger = logger;
        }

        private const string CreateOrgPath = "/Organisations/Create";

        private static readonly string[] AcceptablePaths =
        {
            CreateOrgPath,
            "/Organisations/Join",
            "/Organisations/RequestToJoin",
            "/Organisations/Detail",
            "/Organisations",
            "/Profiles/Account/ConfirmEmail",
            "/Market/LocationSearch",
            "/Identity"
        };

        private const string QueryString = "?message=You must belong to an Organisation to continue";
        public async Task Invoke(HttpContext httpContext, IUserService userService, IOrganisationService organisationService)
        {
            var organisationId = httpContext.User.GetOrganisationId();
            var name = httpContext.User.GetUserName();
            var userId = httpContext.User.GetUserId();

            if (string.IsNullOrEmpty(organisationId))
            {
                // organisation ID may exist
                var userDetails = await userService.UserStore.ReadAsync(userId);
                if (!string.IsNullOrEmpty(userDetails.OrganisationId))
                {
                    organisationId = userDetails.OrganisationId;
                }
            }

            if (httpContext.User.Identity?.IsAuthenticated == true // don't redirect if there's no user
                 && string.IsNullOrEmpty(organisationId) // only redirect if user doesn't have an org
                 && !AcceptablePaths.Any(_ => string.Equals(httpContext.Request.Path, _, comparisonType: StringComparison.OrdinalIgnoreCase))
                 && !httpContext.Request.Path.StartsWithSegments("/api")) // don't redirect API calls
            {
                logger.LogInformation($"Redirecting {name} to {CreateOrgPath}");
                httpContext.Response.Redirect($"{CreateOrgPath}{QueryString}");
            }

            await _next(httpContext);
        }
    }
}
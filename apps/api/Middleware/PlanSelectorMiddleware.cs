using System;
using System.Linq;
using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Common.Contracts;
using Amphora.Common.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Amphora.Api.Middleware
{
    public class PlanSelectorMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<PlanSelectorMiddleware> logger;

        public PlanSelectorMiddleware(RequestDelegate next,
                                           ILogger<PlanSelectorMiddleware> logger)
        {
            _next = next;
            this.logger = logger;
        }

        private const string CreateOrgPath = "/Organisations/Create";
        private const string JoinOrgPath = "/Organisations/Account/Join";
        private const string SelectPlanPath = "/Organisations/Account/SelectPlan";

        private static readonly string[] AcceptablePaths =
        {
            CreateOrgPath,
            JoinOrgPath,
            SelectPlanPath,
            "/Organisations",
            "/Organisations/Detail",
            "/Organisations/RequestToJoin",
            "/Profiles/Account/ConfirmEmail",
            "/Market/LocationSearch",
        };

        public async Task Invoke(HttpContext httpContext,
                                 IUserDataService userDataService,
                                 IInvitationService invitationService,
                                 IOrganisationService organisationService)
        {
            var name = httpContext.User.GetUserName();
            var userId = httpContext.User.GetUserId();

            // organisation ID may exist
            var userDetailsRes = await userDataService.ReadAsync(httpContext.User, userId);

            var userDetails = userDetailsRes.Entity;

            var organisationId = userDetailsRes.Succeeded ? userDetailsRes.Entity?.OrganisationId : null;

            if (httpContext.User.Identity?.IsAuthenticated == true // don't redirect if there's no user
                 && string.IsNullOrEmpty(organisationId) // only redirect if user doesn't have an org
                 && !AcceptablePaths.Any(_ => string.Equals(httpContext.Request.Path, _, comparisonType: StringComparison.OrdinalIgnoreCase))
                 && !httpContext.Request.Path.StartsWithSegments("/api")) // don't redirect API calls
            {
                // check for invitation
                var inviteRes = await invitationService.GetMyInvitations(httpContext.User);

                if (inviteRes.Succeeded && inviteRes.Entity != null && inviteRes.Entity.Any())
                {
                    var invitation = inviteRes.Entity.FirstOrDefault();
                    // now navigate to accept that invite.
                    logger.LogInformation("Found invitation! Redirecting...");
                    httpContext.Response.Redirect($"{JoinOrgPath}?id={invitation.TargetOrganisationId}");
                }
                else
                {
                    logger.LogInformation($"No invitation. Redirecting {name} to {SelectPlanPath}");
                    httpContext.Response.Redirect($"{SelectPlanPath}");
                }
            }

            await _next(httpContext);
        }
    }
}
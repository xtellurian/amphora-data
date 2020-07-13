using System;
using System.Linq;
using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Common.Contracts;
using Amphora.Common.Extensions;
using Amphora.Common.Models.Organisations;
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
        private const string JoinOrgPath = "/Organisations/Join";
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

            if (httpContext.User.Identity?.IsAuthenticated == true)
            {
                // if we are authenticated
                if (string.IsNullOrEmpty(organisationId))
                {
                    var inviteRes = await invitationService.GetMyInvitations(httpContext.User);

                    if (inviteRes.Succeeded && inviteRes.Entity != null && inviteRes.Entity.Any())
                    {
                        // there's an invitation.
                        var invitation = inviteRes.Entity.FirstOrDefault();
                        // now navigate to accept that invite.
                        logger.LogInformation("Found invitation! Redirecting...");
                        if (httpContext.Request.Path != JoinOrgPath)
                        {
                            httpContext.Response.Redirect($"{JoinOrgPath}?id={invitation.TargetOrganisationId}");
                        }
                    }
                    else
                    {
                        logger.LogInformation($"No invitation. Redirecting {name} to {SelectPlanPath}");
                        // actually le's just create an org here too.
                        var org = OrganisationModel.Autogenerate($"{userDetails.UserName}'s Organisation");
                        var createRes = await organisationService.CreateAsync(httpContext.User, org);
                        // httpContext.Response.Redirect($"{SelectPlanPath}");
                    }
                }
            }

            // if (httpContext.User.Identity?.IsAuthenticated == true // don't redirect if there's no user
            //      && string.IsNullOrEmpty(organisationId) // only redirect if user doesn't have an org
            //      && !AcceptablePaths.Any(_ => string.Equals(httpContext.Request.Path, _, comparisonType: StringComparison.OrdinalIgnoreCase))
            //      && !httpContext.Request.Path.StartsWithSegments("/api")) // don't redirect API calls
            // {
            //     // check for invitation

            // }

            await _next(httpContext);
        }
    }
}
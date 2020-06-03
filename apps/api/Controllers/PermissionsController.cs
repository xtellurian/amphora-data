using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Amphora.Api.AspNet;
using Amphora.Api.Contracts;
using Amphora.Api.Models.Dtos.AccessControls;
using Amphora.Common.Contracts;
using Amphora.Common.Models.Permissions;
using Microsoft.AspNetCore.Mvc;

namespace Amphora.Api.Controllers.Amphorae
{
    [ApiController]
    [SkipStatusCodePages]
    [CommonAuthorize]
    public class PermissionController : Controller
    {
        private readonly IAmphoraeService amphoraeService;
        private readonly IPermissionService permissionService;

        public PermissionController(IAmphoraeService amphoraeService, IPermissionService permissionService)
        {
            this.amphoraeService = amphoraeService;
            this.permissionService = permissionService;
        }

        /// <summary>
        /// The permission level for each object id in the request.
        /// </summary>
        /// <param name="req">A request object containing the list of ids to check.</param>
        /// <returns>A dictionary with permission levels associated to the ids.</returns>
        [Produces(typeof(PermissionsResponse))]
        [HttpPost("api/permissions")]
        public async Task<IActionResult> GetPermissions(PermissionsRequest req)
        {
            if (req is null || req.AccessQueries is null || req.AccessQueries.Count == 0)
            {
                return BadRequest("You must provide some access queries");
            }

            var results = new List<AccessLevelResponse>();
            foreach (var q in req.AccessQueries)
            {
                results.Add(await GetAccessLevelAsync(q));
            }

            return Ok(new PermissionsResponse(results));
        }

        private async Task<AccessLevelResponse> GetAccessLevelAsync(AccessLevelQuery query)
        {
            if (query == null)
            {
                return new AccessLevelResponse(null, false);
            }

            AccessLevels level = (AccessLevels)Enum.ToObject(typeof(AccessLevels), query.AccessLevel);
            var readRes = await amphoraeService.ReadAsync(User, query.AmphoraId);
            if (readRes.Failed || readRes.Entity == null)
            {
                return new AccessLevelResponse(query, false);
            }

            return new AccessLevelResponse(query, await permissionService.IsAuthorizedAsync(User, readRes.Entity, level));
        }
    }
}
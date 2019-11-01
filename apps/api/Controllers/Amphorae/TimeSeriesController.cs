using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Api.Filters;
using Amphora.Common.Models.Permissions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.TimeSeriesInsights.Models;
using Microsoft.Extensions.Logging;

namespace Amphora.Api.Controllers.Amphorae
{
    [ApiController]
    [SkipStatusCodePages]
    public class TimeSeriesController : Controller
    {
        private ITsiService tsiService;
        private readonly IPermissionService permissionService;
        private readonly IAmphoraeService amphoraeService;
        private readonly IUserService userService;
        private ILogger<TimeSeriesController> logger;

        public TimeSeriesController(ITsiService tsiService,
                                            IPermissionService permissionService,
                                            IAmphoraeService amphoraeService,
                                            IUserService userService,
                                            ILogger<TimeSeriesController> logger)
        {
            this.tsiService = tsiService;
            this.permissionService = permissionService;
            this.amphoraeService = amphoraeService;
            this.userService = userService;
            this.logger = logger;
        }

        /// <summary>
        /// Updates the details of an Amphora by Id
        /// </summary>
        /// <param name="query">Time Series query. See https://github.com/microsoft/tsiclient/blob/master/docs/Server.md#functions</param>  
        [HttpPost("api/timeseries/query")]
        [Authorize(AuthenticationSchemes = "Identity.Application" + "," + JwtBearerDefaults.AuthenticationScheme)]
        [Produces(typeof(QueryResultPage))]
        [ProducesResponseType(403)]
        [AddJsonContentType]
        public async Task<IActionResult> QueryTimeSeries(QueryRequest query)
        {
            var user = await userService.ReadUserModelAsync(User);
            var ids = new List<string>();
            if (query.AggregateSeries?.TimeSeriesId != null)
            {
                ids.AddRange(query.AggregateSeries?.TimeSeriesId.Select(_ => _ as string).Where(_ => _ != null));
            }
            if (query.GetEvents?.TimeSeriesId != null)
            {
                ids.AddRange(query.GetEvents?.TimeSeriesId.Select(_ => _ as string).Where(_ => _ != null));
            }
            if (query.GetSeries?.TimeSeriesId != null)
            {
                ids.AddRange(query.GetSeries?.TimeSeriesId.Select(_ => _ as string).Where(_ => _ != null));
            }

            foreach (var id in ids)
            {
                var res = await amphoraeService.ReadAsync(User, id);
                if (res.Succeeded && await permissionService.IsAuthorizedAsync(user, res.Entity, AccessLevels.ReadContents))
                {
                    // all good
                }
                else if (res.WasForbidden)
                {
                    return StatusCode(403);
                }
            }

            try
            {
                var response = await tsiService.RunQueryAsync(query);
                return new OkObjectResult(response);
            }
            catch (System.Exception ex)
            {
                logger.LogError(ex.Message, ex);
                return BadRequest(ex.Message);
            }
        }
    }
}
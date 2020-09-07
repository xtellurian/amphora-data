using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amphora.Api.AspNet;
using Amphora.Api.Contracts;
using Amphora.Api.Models.Dtos;
using Amphora.Common.Contracts;
using Amphora.Common.Models.Permissions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.TimeSeriesInsights.Models;
using Microsoft.Extensions.Logging;

namespace Amphora.Api.Controllers.Amphorae
{
    [ApiMajorVersion(0)]
    [ApiController]
    [SkipStatusCodePages]
    public class TimeSeriesController : Controller
    {
        private ITsiService tsiService;
        private readonly IPermissionService permissionService;
        private readonly IAmphoraeService amphoraeService;
        private readonly IUserDataService userDataService;
        private ILogger<TimeSeriesController> logger;

        public TimeSeriesController(ITsiService tsiService,
                                            IPermissionService permissionService,
                                            IAmphoraeService amphoraeService,
                                            IUserDataService userDataService,
                                            ILogger<TimeSeriesController> logger)
        {
            this.tsiService = tsiService;
            this.permissionService = permissionService;
            this.amphoraeService = amphoraeService;
            this.userDataService = userDataService;
            this.logger = logger;
        }

        /// <summary>
        /// Updates the details of an Amphora by Id.
        /// </summary>
        /// <param name="query">Time Series query. See https://github.com/microsoft/tsiclient/blob/master/docs/Server.md#functions .</param>
        /// <returns>A Query Result.</returns>
        [HttpPost("api/timeseries/query")]
        [CommonAuthorize]
        [Produces(typeof(QueryResultPage))]
        [ProducesResponseType(403)]
        [ProducesBadRequest]
        [AddJsonContentType]
        public async Task<IActionResult> QueryTimeSeries(QueryRequest query)
        {
            var userReadRes = await userDataService.ReadAsync(User);
            if (!userReadRes.Succeeded)
            {
                return Unauthorized();
            }

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
                if (res.Failed)
                {
                    return NotFound($"Amphora({id}) not found");
                }

                if (!await permissionService.IsAuthorizedAsync(userReadRes.Entity, res.Entity, AccessLevels.ReadContents))
                {
                    return StatusCode(403);
                }
                else
                {
                    logger.LogInformation("Granting user access to signals");
                }
            }

            try
            {
                var response = await tsiService.RunQueryAsync(query);
                return new OkObjectResult(response);
            }
            catch (TsiErrorException tsiEx)
            {
                logger.LogError(tsiEx.Body.Error.Message);
                return BadRequest(new Response($"The upstream service responsed: {tsiEx.Body.Error.Message}"));
            }
            catch (System.Exception ex)
            {
                logger.LogError(ex.Message, ex);
                return BadRequest(new Response(ex.Message));
            }
        }
    }
}
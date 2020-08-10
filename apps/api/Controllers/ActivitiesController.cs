using System.Linq;
using System.Threading.Tasks;
using Amphora.Api.AspNet;
using Amphora.Api.Contracts;
using Amphora.Api.Extensions;
using Amphora.Api.Models.Dtos;
using Amphora.Api.Models.Dtos.Activities;
using Amphora.Common.Contracts;
using Amphora.Common.Models;
using Amphora.Common.Models.Activities;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;

namespace Amphora.Api.Controllers
{
    [ApiMajorVersion(0)]
    [ApiController]
    [SkipStatusCodePages]
    [Route("api/activities")]
    [OpenApiTag("Activities")]
    public class ActivitiesController : EntityController
    {
        private readonly IActivityService activityService;
        private readonly IActivityRunService runService;
        private readonly IAmphoraeService amphoraeService;
        private readonly IMapper mapper;

        public ActivitiesController(IActivityService activityService,
                                    IActivityRunService runService,
                                    IAmphoraeService amphoraeService,
                                    IMapper mapper)
        {
            this.activityService = activityService;
            this.runService = runService;
            this.amphoraeService = amphoraeService;
            this.mapper = mapper;
        }

        /// <summary>
        /// Creates a new activity.
        /// </summary>
        /// <param name="activity">Metadata of the new activity.</param>
        /// <returns>The Activity information.</returns>
        [HttpPost]
        [CommonAuthorize]
        [Produces(typeof(Activity))]
        [ProducesBadRequest]
        [ValidateModel]
        public async Task<IActionResult> CreateActivity([FromBody] CreateActivity activity)
        {
            var model = mapper.Map<ActivityModel>(activity);
            var createRes = await activityService.CreateAsync(User, model);
            if (createRes.Succeeded)
            {
                var result = mapper.Map<Activity>(createRes.Entity);
                return Ok(result);
            }
            else
            {
                return Handle(createRes);
            }
        }

        /// <summary>
        /// Gets the metadata of an activity.
        /// </summary>
        /// <param name="id">The activity Id.</param>
        /// <returns>The Activity information.</returns>
        [HttpGet("{id}")]
        [CommonAuthorize]
        [Produces(typeof(Activity))]
        [ProducesBadRequest]
        public async Task<IActionResult> ReadActivity(string id)
        {
            var readRes = await activityService.ReadAsync(User, id);
            if (readRes.Succeeded)
            {
                var result = mapper.Map<Activity>(readRes.Entity);
                return Ok(result);
            }
            else
            {
                return Handle(readRes);
            }
        }

        /// <summary>
        /// Deletes an activity.
        /// </summary>
        /// <param name="id">The activity Id.</param>
        /// <returns>The Activity information.</returns>
        [HttpDelete("{id}")]
        [Produces(typeof(Response))]
        [ProducesBadRequest]
        [CommonAuthorize]
        public async Task<IActionResult> DeleteActivity(string id)
        {
            var readRes = await activityService.ReadAsync(User, id);
            if (readRes.Succeeded)
            {
                var deleteRes = await activityService.DeleteAsync(User, readRes.Entity);
                if (deleteRes.Succeeded)
                {
                    return Ok(new Response($"Deleted Activity({id})"));
                }
                else
                {
                    return Handle(deleteRes);
                }
            }
            else
            {
                return Handle(readRes);
            }
        }

        /// <summary>
        /// Starts a new run of an activity.
        /// </summary>
        /// <param name="id">The activity id in which to start a run.</param>
        /// <returns>The Run information.</returns>
        [HttpPost("{id}/Runs")]
        [Produces(typeof(Run))]
        [ProducesBadRequest]
        [CommonAuthorize]
        public async Task<IActionResult> StartRun(string id)
        {
            var readRes = await activityService.ReadAsync(User, id);
            if (readRes.Succeeded)
            {
                EntityOperationResult<ActivityRunModel> createRunRes;
                if (Request.TryReadClientVersion(out var versionInfo))
                {
                    createRunRes = await runService.StartRunAsync(User, readRes.Entity, versionInfo);
                }
                else
                {
                    createRunRes = await runService.StartRunAsync(User, readRes.Entity);
                }

                if (createRunRes.Succeeded)
                {
                    var dto = mapper.Map<Run>(createRunRes.Entity);
                    return Ok(dto);
                }
                else
                {
                    return Handle(createRunRes);
                }
            }
            else
            {
                return Handle(readRes);
            }
        }

        /// <summary>
        /// Updates and completes a run.
        /// </summary>
        /// <param name="id">The activity Id.</param>
        /// <param name="runId">The run Id.</param>
        /// <param name="update">Information about the update.</param>
        /// <returns>The Activity information.</returns>
        [HttpPost("{id}/Runs/{runId}")]
        [Produces(typeof(Run))]
        [ProducesBadRequest]
        [CommonAuthorize]
        public async Task<IActionResult> UpdateRun(string id, string runId, UpdateRun update)
        {
            var activityReadRes = await activityService.ReadAsync(User, id);
            if (activityReadRes.Succeeded)
            {
                var activity = activityReadRes.Entity;
                var run = activity.Runs.FirstOrDefault(_ => _.Id == runId);
                if (run == null)
                {
                    return BadRequest($"Run({runId}) not found in activity {id}");
                }
                else if (run.Success == true || run.EndTime != null)
                {
                    return BadRequest($"Run({runId}) is already complete");
                }
                else if (update.Success == null)
                {
                    // there's nothing to update.
                    var dto = mapper.Map<Run>(run);
                    return Ok(dto);
                }

                var finishRunRes = await runService.FinishRunAsync(User, activity, run, update.Success.Value);
                if (finishRunRes.Succeeded)
                {
                    var dto = mapper.Map<Run>(finishRunRes.Entity);
                    return Ok(dto);
                }
                else
                {
                    return Handle(finishRunRes);
                }
            }
            else
            {
                return Handle(activityReadRes);
            }
        }

        /// <summary>
        /// References an Amphora during a run.
        /// </summary>
        /// <param name="id">The activity Id.</param>
        /// <param name="runId">The run Id.</param>
        /// <param name="amphoraId">The Id of the Amphora to reference.</param>
        /// <param name="amphoraReference">Information about the reference.</param>
        /// <returns>The reference information.</returns>
        [HttpPut("{id}/Runs/{runId}/amphorae/{amphoraId}")]
        [Produces(typeof(AmphoraReference))]
        [ProducesBadRequest]
        [CommonAuthorize]
        public async Task<IActionResult> ReferenceAmphora(string id, string runId, string amphoraId, [FromBody] AmphoraReference amphoraReference)
        {
            if (amphoraReference.AmphoraId != amphoraId)
            {
                return BadRequest(new Response($"Amphora Id {amphoraReference.AmphoraId} must match id {amphoraId}"));
            }

            var activityReadRes = await activityService.ReadAsync(User, id);
            if (activityReadRes.Succeeded)
            {
                var activity = activityReadRes.Entity;
                var run = activity.Runs.FirstOrDefault(_ => _.Id == runId);
                if (run == null)
                {
                    return BadRequest(new Response($"Run({runId}) not found in activity {id}"));
                }
                else if (run.Success == true || run.EndTime != null)
                {
                    return BadRequest(new Response($"Run({runId}) is already complete"));
                }

                var amphoraReadRes = await amphoraeService.ReadAsync(User, amphoraId);
                if (amphoraReadRes.Failed)
                {
                    return Handle(amphoraReadRes);
                }

                var referenceModel = mapper.Map<ActivityAmphoraReference>(amphoraReference);
                var referenceRes = await runService.ReferenceAmphoraAsync(User, activity, run, referenceModel);
                if (referenceRes.Succeeded)
                {
                    return Ok(amphoraReference);
                }
                else
                {
                    return Handle(referenceRes);
                }
            }
            else
            {
                return Handle(activityReadRes);
            }
        }
    }
}
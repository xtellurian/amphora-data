using System.Threading.Tasks;
using Amphora.Api.AspNet;
using Amphora.Api.Contracts;
using Amphora.Api.Models.Dtos;
using Amphora.Api.Models.Dtos.Amphorae;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;

namespace Amphora.Api.Controllers.Amphorae
{
    [ApiMajorVersion(0)]
    [ApiController]
    [SkipStatusCodePages]
    [Route("api/amphorae/{id}/quality")]
    [OpenApiTag("Amphorae")]
    public class AmphoraQualityController : EntityController
    {
        private readonly IAmphoraeService amphoraService;
        private readonly IMapper mapper;

        public AmphoraQualityController(IAmphoraeService amphoraService, IMapper mapper)
        {
            this.amphoraService = amphoraService;
            this.mapper = mapper;
        }

        /// <summary>
        /// Sets the data quality metrics for this Amphora.
        /// </summary>
        /// <param name="id">Amphora Id.</param>
        /// <param name="quality">The data quality metrics.</param>
        /// <returns>The quality metrics.</returns>
        [Produces(typeof(Quality))]
        [ProducesBadRequest]
        [HttpPost]
        [CommonAuthorize]
        public async Task<IActionResult> Set(string id, [FromBody] Quality quality)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new Response(ModelState.ValidationState.ToString()));
            }

            var readRes = await amphoraService.ReadAsync(User, id);
            if (readRes.Succeeded)
            {
                var a = readRes.Entity;
                a.Quality = mapper.Map<Common.Models.Amphorae.DataQuality>(quality);
                a.Quality ??= new Common.Models.Amphorae.DataQuality();
                var updateRes = await amphoraService.UpdateAsync(User, a);
                if (updateRes.Succeeded)
                {
                    return Ok(quality);
                }
                else
                {
                    return Handle(updateRes);
                }
            }
            else
            {
                return Handle(readRes);
            }
        }

        /// <summary>
        /// Gets the data quality metrics for this Amphora.
        /// </summary>
        /// <param name="id">Amphora Id.</param>
        /// <returns>The quality metrics.</returns>
        [Produces(typeof(Quality))]
        [ProducesBadRequest]
        [HttpGet]
        [CommonAuthorize]
        public async Task<IActionResult> Get(string id)
        {
            var readRes = await amphoraService.ReadAsync(User, id);
            if (readRes.Succeeded)
            {
                var q = mapper.Map<Quality>(readRes.Entity.Quality);
                return Ok(q ?? new Quality());
            }
            else
            {
                return Handle(readRes);
            }
        }
    }
}
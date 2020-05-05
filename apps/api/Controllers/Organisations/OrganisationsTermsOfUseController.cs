using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amphora.Api.AspNet;
using Amphora.Api.Contracts;
using Amphora.Api.Models.Dtos.Organisations;
using Amphora.Common.Models.Amphorae;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;

namespace Amphora.Api.Controllers.Organisations
{
    [ApiMajorVersion(0)]
    [ApiController]
    [SkipStatusCodePages]
    [OpenApiTag("Organisations")]
    [Route("api/Organisations/{id}/TermsOfUse")]
    public class OrganisationsTermsOfUseController : Controller
    {
        private readonly IOrganisationService organisationService;
        private readonly IMapper mapper;

        public OrganisationsTermsOfUseController(IOrganisationService organisationService, IMapper mapper)
        {
            this.organisationService = organisationService;
            this.mapper = mapper;
        }

        /// <summary>
        /// Adds new Terms and Conditions to your Organisations T/C Library.
        /// </summary>
        /// <param name="id">The Id of the Organisation.</param>
        /// <param name="tou">The new Terms of Use.</param>
        /// <returns> The organisation metadaa. </returns>
        /// <returns> The Terms and Conditions. </returns>
        [Produces(typeof(TermsOfUse))]
        [ProducesResponseType(400)]
        [CommonAuthorize]
        [HttpPost]
        public async Task<IActionResult> Create(string id, [FromBody] CreateTermsOfUse tou)
        {
            var org = await organisationService.Store.ReadAsync(id);

            var model = mapper.Map<TermsOfUseModel>(tou);

            var result = await organisationService.UpdateAsync(User, org);
            if (result.Succeeded)
            {
                tou = mapper.Map<TermsOfUse>(result.Entity);
                return Ok(tou);
            }
            else if (result.WasForbidden)
            {
                return StatusCode(403, result.Message);
            }
            else
            {
                return BadRequest(result.Message);
            }
        }

        /// <summary>
        /// Get's a list of an Organisation's Terms and Conditions.
        /// </summary>
        /// <param name="id">The Id of the Organisation.</param>
        /// <returns> The Terms and Conditions. </returns>
        [Produces(typeof(IEnumerable<TermsOfUse>))]
        [CommonAuthorize]
        [HttpGet]
        public async Task<IActionResult> Read(string id)
        {
            var org = await organisationService.Store.ReadAsync(id);
            return Ok(mapper.Map<List<TermsOfUse>>(org.TermsOfUses));
        }
    }
}
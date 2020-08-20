using System.Collections.Generic;
using System.Threading.Tasks;
using Amphora.Api.AspNet;
using Amphora.Api.Contracts;
using Amphora.Api.Models.Dtos.Terms;
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
        /// Gets a list of an Organisation's Terms of Use.
        /// </summary>
        /// <param name="id">The Id of the Organisation.</param>
        /// <returns> The Terms. </returns>
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
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Api.Models.Dtos;
using Amphora.Common.Models.Organisations;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Amphora.Api.Controllers.Organisations
{
    [ApiController]
    public class TermsAndConditionsController: Controller
    {
        private readonly IOrganisationService organisationService;
        private readonly IMapper mapper;

        public TermsAndConditionsController(IOrganisationService organisationService, IMapper mapper)
        {
            this.organisationService = organisationService;
            this.mapper = mapper;
        }

        /// <summary>
        /// Adds new Terms and Conditions to your Organisations T/C Library
        /// </summary>
        /// <param name="dto">The new Terms and Conditions</param>  
        /// <param name="id">The Id of the Organisation</param>  
        [Produces(typeof(TermsAndConditionsDto))]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("api/Organisations/{id}/TermsAndConditions")]
        public async Task<IActionResult> CreateTermsAndConditions(string id, [FromBody] TermsAndConditionsDto dto)
        {
            var org = await organisationService.Store.ReadAsync(id);
            if(org.TermsAndConditions.Any(t => t.Name == dto.Name))
            {
                return BadRequest($"{dto.Name} already exists");
            }
            var model = mapper.Map<TermsAndConditionsModel>(dto);

            if(org.TermsAndConditions == null) org.TermsAndConditions = new List<TermsAndConditionsModel>();
            org.TermsAndConditions.Add(model);

            var result = await organisationService.UpdateAsync(User, org);
            if(result.Succeeded)
            {
                dto = mapper.Map<TermsAndConditionsDto>(model);
                return Ok(dto);
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
        /// Get's a list of an Organisation's Terms and Conditions
        /// </summary> 
        /// <param name="id">The Id of the Organisation</param>  
        [Produces(typeof(IEnumerable<TermsAndConditionsDto>))]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("api/Organisations/{id}/TermsAndConditions")]
        public async Task<IActionResult> GetTermsAndConditions(string id)
        {
            var org = await organisationService.Store.ReadAsync(id);
            return Ok(mapper.Map<List<TermsAndConditionsDto>>(org.TermsAndConditions));
           
        }
    }
}
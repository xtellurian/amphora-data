using System.Collections.Generic;
using System.Threading.Tasks;
using Amphora.Api.AspNet;
using Amphora.Api.Contracts;
using Amphora.Api.Models.Dtos.Organisations;
using Amphora.Common.Models.Amphorae;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;

namespace Amphora.Api.Controllers
{
    [ApiMajorVersion(0)]
    [ApiController]
    [SkipStatusCodePages]
    [Route("api/TermsOfUse")]
    [OpenApiTag("TermsOfUse")]
    public class TermsOfUseController : EntityController
    {
        private readonly ITermsOfUseService termsOfUseService;
        private readonly IMapper mapper;

        public TermsOfUseController(ITermsOfUseService termsOfUseService, IMapper mapper)
        {
            this.termsOfUseService = termsOfUseService;
            this.mapper = mapper;
        }

        /// <summary>
        /// Returns all Terms of Use.
        /// </summary>
        /// <returns> A collection of Terms of Use.</returns>
        [HttpGet("")]
        [Produces(typeof(IEnumerable<Models.Dtos.Organisations.TermsOfUse>))]
        [CommonAuthorize]
        public async Task<IActionResult> List()
        {
            var res = await termsOfUseService.ListAsync(User);
            if (res.Succeeded)
            {
                var dto = mapper.Map<List<TermsOfUse>>(res.Entity);
                return Ok(dto);
            }
            else if (res.WasForbidden) { return StatusCode(403); }
            else { return BadRequest(res.Message); }
        }

        /// <summary>
        /// Returns all Terms of Use.
        /// </summary>
        /// <returns> A collection of Terms of Use.</returns>
        [HttpGet("{id}")]
        [Produces(typeof(Models.Dtos.Organisations.TermsOfUse))]
        [CommonAuthorize]
        public async Task<IActionResult> Read(string id)
        {
            var res = await termsOfUseService.ReadAsync(User, id);
            if (res.Succeeded)
            {
                var dto = mapper.Map<TermsOfUse>(res.Entity);
                return Ok(dto);
            }
            else if (res.WasForbidden) { return StatusCode(403); }
            else { return BadRequest(res.Message); }
        }

        /// <summary>
        /// Creates a Terms of Use object.
        /// </summary>
        /// <param name="tou">The terms of use to create.</param>
        /// <returns> The terms of use object. </returns>
        [HttpPost("")]
        [Produces(typeof(TermsOfUse))]
        [CommonAuthorize]
        public async Task<IActionResult> Create([FromBody] CreateTermsOfUse tou)
        {
            var model = mapper.Map<TermsOfUseModel>(tou);
            var createRes = await termsOfUseService.CreateAsync(User, model);
            if (createRes.Succeeded)
            {
                var dto = mapper.Map<TermsOfUse>(createRes.Entity);
                return Ok(dto);
            }
            else
            {
                return Handle(createRes);
            }
        }

        /// <summary>
        /// Deletes a Terms of Use object.
        /// </summary>
        /// <param name="id">The terms of use id to delete.</param>
        /// <returns> 200 if successful. </returns>
        [HttpDelete("{id}")]
        [CommonAuthorize]
        public async Task<IActionResult> Delete(string id)
        {
            var readRes = await termsOfUseService.ReadAsync(User, id);
            if (readRes.Succeeded)
            {
                var deleteRes = await termsOfUseService.DeleteAsync(User, readRes.Entity);
                if (deleteRes.Succeeded)
                {
                    return Ok();
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
        /// Accepts a Terms of Use.
        /// </summary>
        /// <param name="id">The Terms of Use id.</param>
        /// <returns> 200 if accepted. </returns>
        [HttpPost("{id}/Accepts")]
        [CommonAuthorize]
        public async Task<IActionResult> Accept(string id)
        {
            var termsOfUseRes = await termsOfUseService.ReadAsync(User, id);
            if (termsOfUseRes.Succeeded)
            {
                var acceptRes = await termsOfUseService.AcceptAsync(User, termsOfUseRes.Entity);
                if (acceptRes.Succeeded)
                {
                    return Ok();
                }
                else
                {
                    return Handle(acceptRes);
                }
            }
            else
            {
                return Handle(termsOfUseRes);
            }
        }
    }
}
using System.Collections.Generic;
using System.Threading.Tasks;
using Amphora.Api.AspNet;
using Amphora.Api.Contracts;
using Amphora.Api.Models.Dtos.Organisations;
using Amphora.Api.Models.Dtos.Terms;
using Amphora.Common.Models.Amphorae;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
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
        private readonly ILogger<TermsOfUseController> logger;

        public TermsOfUseController(ITermsOfUseService termsOfUseService,
                                    IMapper mapper,
                                    ILogger<TermsOfUseController> logger)
        {
            this.termsOfUseService = termsOfUseService;
            this.mapper = mapper;
            this.logger = logger;
        }

        /// <summary>
        /// Returns all Terms of Use.
        /// </summary>
        /// <returns> A collection of Terms of Use.</returns>
        [HttpGet]
        [Produces(typeof(IEnumerable<Models.Dtos.Terms.TermsOfUse>))]
        [ProducesBadRequest]
        [CommonAuthorize]
        public async Task<IActionResult> List([FromQuery] ListTermsOptions options = null)
        {
            options ??= new ListTermsOptions();
            var res = await termsOfUseService.ListAsync(User, options.Skip ?? 0, options.Take ?? 64);
            if (res.Succeeded)
            {
                var dto = mapper.Map<List<TermsOfUse>>(res.Entity);
                return Ok(dto);
            }
            else
            {
                return Handle(res);
            }
        }

        /// <summary>
        /// Returns all Terms of Use.
        /// </summary>
        /// <returns> A collection of Terms of Use.</returns>
        [HttpGet("{id}")]
        [Produces(typeof(Models.Dtos.Terms.TermsOfUse))]
        [ProducesBadRequest]
        [CommonAuthorize]
        public async Task<IActionResult> Read(string id)
        {
            var res = await termsOfUseService.ReadAsync(User, id);
            if (res.Succeeded)
            {
                var dto = mapper.Map<TermsOfUse>(res.Entity);
                return Ok(dto);
            }
            else
            {
                return Handle(res);
            }
        }

        /// <summary>
        /// Creates a Terms of Use object.
        /// </summary>
        /// <param name="tou">The terms of use to create.</param>
        /// <returns> The terms of use object. </returns>
        [HttpPost]
        [Produces(typeof(TermsOfUse))]
        [ProducesBadRequest]
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
        /// Creates a Global Terms of Use object.
        /// </summary>
        /// <param name="tou">The terms of use to create.</param>
        /// <returns> The terms of use object. </returns>
        [HttpPost("/api/GlobalTermsOfUse")]
        [Produces(typeof(TermsOfUse))]
        [ProducesBadRequest]
        [CommonAuthorize]
        [OpenApiIgnore]
        public async Task<IActionResult> CreateGlobal([FromBody] CreateTermsOfUse tou)
        {
            var model = mapper.Map<TermsOfUseModel>(tou);
            try
            {
                var createRes = await termsOfUseService.CreateGlobalAsync(User, model);
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
            catch (Common.Exceptions.PermissionDeniedException permEx)
            {
                logger.LogWarning($"User tried to create Global Terms of Use", permEx);
                return BadRequest("You do not have the right permissions.");
            }
        }

        /// <summary>
        /// Deletes a Terms of Use object.
        /// </summary>
        /// <param name="id">The terms of use id to delete.</param>
        /// <returns> 200 if successful. </returns>
        [HttpDelete("{id}")]
        [ProducesBadRequest]
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
        [ProducesBadRequest]
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
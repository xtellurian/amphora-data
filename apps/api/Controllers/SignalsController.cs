using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Common.Models.Amphorae;
using Amphora.Common.Models.Domains;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace Amphora.Api.Controllers
{
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class SignalController : Controller
    {
        private readonly IAmphoraeService amphoraeService;
        private readonly ISignalService signalService;
        private readonly ITsiService tsiService;
        private readonly IMapper mapper;
        private readonly ILogger<SignalController> logger;

        public SignalController(
            IAmphoraeService amphoraeService,
            ISignalService signalService,
            ITsiService tsiService,
            IMapper mapper,
            ILogger<SignalController> logger)
        {
            this.amphoraeService = amphoraeService;
            this.signalService = signalService;
            this.tsiService = tsiService;
            this.mapper = mapper;
            this.logger = logger;
        }

        [HttpPost("api/amphorae/{id}/signals")]
        public async Task<IActionResult> Upload(string id, [FromBody] JObject jObj)
        {

            var entity = await amphoraeService.AmphoraStore.ReadAsync(id);
            if (entity == null)
            {
                return NotFound("Invalid Id");
            }
            var model = await amphoraeService.AmphoraStore.ReadAsync<AmphoraExtendedModel>(id, entity.OrganisationId); // bit of a hack
            logger.LogInformation($"Signal Upload for {id}");
            jObj["amphora"] = id;
            var domain = Domain.GetDomain(model.DomainId);
            if (domain.IsValid(jObj))
            {
                await signalService.WriteSignalAsync(model, domain.ToDatum(jObj));
                return Ok();
            }
            else
            {
                return BadRequest("Invalid Schema");
            }
        }
    }
}

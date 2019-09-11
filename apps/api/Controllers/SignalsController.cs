using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Amphora.Api.Contracts;
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
        private readonly IDataStore<Amphora.Common.Models.AmphoraModel, Datum> dataStore;
        private readonly ITsiService tsiService;
        private readonly IMapper mapper;
        private readonly ILogger<SignalController> logger;

        public SignalController(
            IAmphoraeService amphoraeService,
            IDataStore<Amphora.Common.Models.AmphoraModel, Datum> dataStore,
            ITsiService tsiService,
            IMapper mapper,
            ILogger<SignalController> logger)
        {
            this.amphoraeService = amphoraeService;
            this.dataStore = dataStore;
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
            logger.LogInformation($"Signal Upload for {id}");
            jObj["amphora"] = id;
            var domain = Domain.GetDomain(entity.DomainId);
            if (domain.IsValid(jObj))
            {
                var jObjResult = await dataStore.SetDataAsync(entity, domain.ToDatum(jObj), null);
                return Ok(jObjResult);
            }
            else
            {
                return BadRequest("Invalid Schema");
            }
        }

        [ApiExplorerSettings(IgnoreApi = true)] // JArray causes issues with swashbucke :(
        [HttpPost("api/amphorae/{id}/uploadMany")]
        public async Task<IActionResult> UploadMany(string id, [FromBody] JArray jArray)
        {
            var entity = await amphoraeService.AmphoraStore.ReadAsync(id);
            if (entity == null)
            {
                return NotFound("Invalid Id");
            }
            var domain = Domain.GetDomain(entity.DomainId);
            var results = new List<Datum>();
            foreach (JObject jObj in jArray)
            {
                jObj["amphora"] = id;
                if (domain.IsValid(jObj))
                {
                    results.Add(await dataStore.SetDataAsync(entity, domain.ToDatum(jObj), null));
                }
                else
                {
                    logger.LogWarning("Invalid Object in uploadMany");
                }
            }
            return Ok();
        }
    }
}

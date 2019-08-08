using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Api.Extensions;
using Amphora.Api.Models;
using Amphora.Api.ViewModels;
using Amphora.Common.Models;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace Amphora.Api.Controllers
{
    public class TemporaeController : Controller
    {
        private readonly IOrgEntityStore<Common.Models.Tempora> temporaEntityStore;
        private readonly IDataStore<Tempora, JObject> dataStore;
        private readonly ITsiService tsiService;
        private readonly IMapper mapper;

        public TemporaeController(
            IOrgEntityStore<Amphora.Common.Models.Tempora> temporaEntityStore,
            IEntityStore<Schema> schemaStore,
            IDataStore<Amphora.Common.Models.Tempora, JObject> dataStore,
            ITsiService tsiService,
            IMapper mapper)
        {
            this.temporaEntityStore = temporaEntityStore;
            this.dataStore = dataStore;
            this.tsiService = tsiService;
            this.mapper = mapper;
        }

        [HttpGet("api/temporae/")]
        public async Task<IActionResult> ListTemporaAsync()
        {
            return Ok(await this.temporaEntityStore.ListAsync());
        }


        [HttpGet("api/temporae/{id}")]
        public async Task<IActionResult> GetInformationAsync(string id)
        {
            return Ok(await this.temporaEntityStore.ReadAsync(id));
        }

        [HttpPut("api/temporae")]
        public async Task<IActionResult> CreateTemporaAsync([FromBody] Amphora.Common.Models.Tempora model)
        {
            if (model == null || !model.IsValid())
            {
                return BadRequest("Invalid Model");
            }
            return Ok(await this.temporaEntityStore.CreateAsync(model));
        }

        [HttpPost("api/temporae/{id}/upload")]
        public async Task<IActionResult> Upload(string id, [FromBody] JObject jObj)
        {
            var entity = await temporaEntityStore.ReadAsync(id);
            if (entity == null)
            {
                return NotFound("Invalid Tempora Id");
            }

            // var schema = schemaStore.Get(entity.SchemaId);
            // if(schema == null)
            // {
            //     return BadRequest("Schema is null");
            // }
            // if (jObj.IsValid(schema.JSchema)) 
            // {
            //     return BadRequest("Invalid Payload");
            // }
            var jObjResult = dataStore.SetData(entity, jObj);
            return Ok(jObjResult);
        }

        [HttpGet("api/temporae/{id}/download")]
        public async Task<IActionResult> Download(string id)
        {
            var entity = await temporaEntityStore.ReadAsync(id);
            if (entity == null)
            {
                return NotFound("Invalid Tempora Id");
            }

            throw new NotImplementedException();
        }
    }
}

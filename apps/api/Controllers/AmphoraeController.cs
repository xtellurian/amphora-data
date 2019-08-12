using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Api.Extensions;
using Amphora.Api.Models;
using Amphora.Api.ViewModels;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;


namespace Amphora.Api.Controllers
{
    public class AmphoraeController : Controller
    {
        private readonly IOrgScopedEntityStore<Common.Models.Amphora> amphoraEntityStore;
        private readonly IDataStore<Common.Models.Amphora, byte[]> dataStore;
        private readonly IMapper mapper;

        public AmphoraeController(
            IOrgScopedEntityStore<Amphora.Common.Models.Amphora> amphoraEntityStore,
            IDataStore<Amphora.Common.Models.Amphora, byte[]> dataStore,
            IMapper mapper)
        {
            this.amphoraEntityStore = amphoraEntityStore;
            this.dataStore = dataStore;
            this.mapper = mapper;
        }

        [HttpGet("api/amphorae")]
        public async Task<IActionResult> ListAmphoraAsync()
        {
            return Ok(await this.amphoraEntityStore.ListAsync());
        }

        [HttpGet("api/amphorae/{id}")]
        public async Task<IActionResult> ReadAsync(string id)
        {
            return Ok(await this.amphoraEntityStore.ReadAsync(id));
        }

        [HttpPut("api/amphorae")]
        public async Task<IActionResult> Create_Api([FromBody] Amphora.Common.Models.Amphora model)
        {
            if (model == null || !model.IsValid())
            {
                return BadRequest("Invalid Model");
            }
            return Ok(await this.amphoraEntityStore.CreateAsync(model));
        }

        [HttpPost("api/amphorae/{id}/upload")]
        public async Task<IActionResult> UploadToAmphora(string id)
        {
            var entity = await amphoraEntityStore.ReadAsync(id);
            if (entity == null)
            {
                return BadRequest("Invalid Amphora Id");
            }

            dataStore.SetData(entity, await Request.Body.ReadFullyAsync());
            return Ok();
        }

        [HttpGet("api/amphorae/{id}/download")]
        public async Task<IActionResult> Download(string id)
        {
            if (string.IsNullOrEmpty(id)) return BadRequest();
            var entity = await amphoraEntityStore.ReadAsync(id);
            if (entity == null) return NotFound("Amphora not found");
            var data = dataStore.GetData(entity);
            if (data == null) return NotFound("Data not found.");
            return File(data, entity.ContentType ?? "application/octet-stream", entity.FileName);
        }
    }
}

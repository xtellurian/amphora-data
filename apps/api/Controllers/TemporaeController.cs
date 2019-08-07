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
        public async Task<IActionResult> FillTempora(string id, [FromBody] JObject jObj)
        {
            var entity = await temporaEntityStore.ReadAsync(id);
            if (entity == null)
            {
                return BadRequest("Invalid Tempora Id");
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
        public async Task<IActionResult> DrinkTemporaAsync(string id)
        {
            var entity = await temporaEntityStore.ReadAsync(id);
            if (entity == null)
            {
                return BadRequest("Invalid Tempora Id");
            }

            throw new NotImplementedException();
        }

        #region Views
        [HttpGet]
        public async Task<IActionResult> Index(string orgId)
        {
            List<Amphora.Common.Models.Tempora> temporae;
            if (orgId != null)
            {
                throw new System.NotImplementedException();
            }
            else
            {
                temporae = (await temporaEntityStore.ListAsync()).ToList();
            }

            var viewModel = new TemporaeViewModel
            {
                Temporae = temporae
            };
            return View(viewModel);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Description,Price")]
            TemporaViewModel temporaVm)
        {
            if (ModelState.IsValid)
            {
                var entity = mapper.Map<Amphora.Common.Models.Tempora>(temporaVm);
                var setResult = await temporaEntityStore.CreateAsync(entity);
                return RedirectToAction(nameof(Index));
            }
            return View(temporaVm);
        }

        [HttpGet]
        public async Task<IActionResult> Detail(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return RedirectToAction("Index");
            }
            var entity = await temporaEntityStore.ReadAsync(id);
            if (entity == null)
            {
                return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            }

            var token = "TODO"; // this can now be a token for THIS site
            var viewModel = new TemporaDetailViewModel
            {
                Token = token,
                Tempora = entity,
                DataAccessFqdn = tsiService.GetDataAccessFqdn()
            };

            return View(viewModel);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        #endregion
    }
}

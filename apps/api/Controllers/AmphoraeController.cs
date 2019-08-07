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
        private readonly IOrgEntityStore<Common.Models.Amphora> amphoraEntityStore;
        private readonly IDataStore<Common.Models.Amphora, byte[]> dataStore;
        private readonly IMapper mapper;

        public AmphoraeController(
            IOrgEntityStore<Amphora.Common.Models.Amphora> amphoraEntityStore,
            IDataStore<Amphora.Common.Models.Amphora, byte[]> dataStore,
            IMapper mapper)
        {
            this.amphoraEntityStore = amphoraEntityStore;
            this.dataStore = dataStore;
            this.mapper = mapper;
        }

        #region REST API
        // this is the API section, where you interact with objects

        [HttpGet("api/amphorae")]
        public async Task<IActionResult> ListAmphoraAsync()
        {
            return Ok(await this.amphoraEntityStore.ListAsync());
        }

        [HttpGet("api/amphorae/{id}")]
        public async Task<IActionResult> GetAmphoraInformationAsync(string id)
        {
            return Ok(await this.amphoraEntityStore.GetAsync(id));
        }

        [HttpPut("api/amphorae")]
        public async Task<IActionResult> Create_Api([FromBody] Amphora.Common.Models.Amphora model)
        {
            if (model == null || !model.IsValid())
            {
                return BadRequest("Invalid Model");
            }
            return Ok(await this.amphoraEntityStore.SetAsync(model));
        }

        [HttpPost("api/amphorae/{id}/upload")]
        public async Task<IActionResult> FillAmphora(string id)
        {
            var entity = await amphoraEntityStore.GetAsync(id);
            if (entity == null)
            {
                return BadRequest("Invalid Amphora Id");
            }

            dataStore.SetData(entity, await Request.Body.ReadFullyAsync());
            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> Download(string id)
        {
            if (string.IsNullOrEmpty(id)) return RedirectToAction(nameof(Index));
            var entity = await amphoraEntityStore.GetAsync(id);
            if (entity == null) return RedirectToAction(nameof(Index));
            var data = dataStore.GetData(entity);
            if (data == null) return RedirectToAction(nameof(Error));
            return File(data, entity.ContentType ?? "application/octet-stream", entity.FileName);
        }

        #endregion

        #region  Views
        // this is the views section

        [HttpGet]
        public async Task<IActionResult> Index(string orgId)
        {
            IList<Amphora.Common.Models.Amphora> amphorae;
            if (orgId != null)
            {
                amphorae = await amphoraEntityStore.ListAsync(orgId);
            }
            else
            {
                amphorae = await amphoraEntityStore.ListAsync();
            }

            var viewModel = new AmphoraeViewModel
            {
                Amphorae = amphorae
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
        public async Task<IActionResult> Create([Bind("Id,Title,Description,ContentType,Price")]
            AmphoraViewModel amphoraVm)
        {
            if (ModelState.IsValid)
            {
                var entity = mapper.Map<Amphora.Common.Models.Amphora>(amphoraVm);
                var setResult = await amphoraEntityStore.SetAsync(entity);
                return RedirectToAction(nameof(Index));
            }
            return View(amphoraVm);
        }

        [HttpGet]
        public async Task<IActionResult> Detail(string id)
        {
            if (string.IsNullOrEmpty(id)) return RedirectToAction(nameof(Index));
            var entity = await amphoraEntityStore.GetAsync(id);
            if (entity == null) return RedirectToAction(nameof(Index));
            return View(mapper.Map<AmphoraViewModel>(entity));
        }


        [HttpPost]
        public async Task<IActionResult> Upload(string id, List<IFormFile> files)
        {
            if (files == null || files.Count > 1)
            {
                throw new System.ArgumentException("Only 1 file is supported");
            }

            if (string.IsNullOrEmpty(id)) return RedirectToAction(nameof(Index));
            var entity = await amphoraEntityStore.GetAsync(id);
            if (entity == null) return RedirectToAction(nameof(Index));

            var formFile = files.FirstOrDefault();

            if (formFile != null && formFile.Length > 0)
            {
                using (var stream = new MemoryStream())
                {
                    await formFile.CopyToAsync(stream);
                    stream.Seek(0, SeekOrigin.Begin);
                    this.dataStore.SetData(entity, await stream.ReadFullyAsync());
                }
                entity.FileName = formFile.FileName;
                await amphoraEntityStore.SetAsync(entity);
            }

            return View("Detail", mapper.Map<AmphoraViewModel>(entity));
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        #endregion
    }
}

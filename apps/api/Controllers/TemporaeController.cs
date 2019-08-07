using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Api.Models;
using Amphora.Api.ViewModels;
using Amphora.Common.Models;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;


namespace Amphora.Api.Controllers
{
    public class TemporaeController : Controller
    {
        private readonly IOrgEntityStore<Common.Models.Tempora> temporaEntityStore;
        private readonly ITsiService tsiService;
        private readonly IMapper mapper;

        public TemporaeController(
            IOrgEntityStore<Amphora.Common.Models.Tempora> temporaEntityStore,
            ITsiService tsiService,
            IMapper mapper)
        {
            this.temporaEntityStore = temporaEntityStore;
            this.tsiService = tsiService;
            this.mapper = mapper;
        }

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
                var setResult = await temporaEntityStore.SetAsync(entity);
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
            var entity = await temporaEntityStore.GetAsync(id);
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
    }
}

using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Api.Models;
using Amphora.Api.ViewModels;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;


namespace Amphora.Api.Controllers
{
    public class AmphoraeController : Controller
    {
        private readonly IOrgEntityStore<Common.Models.Amphora> amphoraEntityStore;
        private readonly IMapper mapper;

        public AmphoraeController(
            IOrgEntityStore<Amphora.Common.Models.Amphora> amphoraEntityStore,
            IMapper mapper)
        {
            this.amphoraEntityStore = amphoraEntityStore;
            this.mapper = mapper;
        }

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

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Api.Models;
using Amphora.Api.ViewModels;
using Amphora.Common.Models;
using Microsoft.AspNetCore.Mvc;


namespace Amphora.Api.Controllers
{
    public class TemporaeController : Controller
    {
        private readonly IDataEntityStore<Common.Models.Tempora> temporaEntityStore;
        private readonly ITsiService tsiService;

        public TemporaeController(IDataEntityStore<Amphora.Common.Models.Tempora> temporaEntityStore,
            ITsiService tsiService )
        {
            this.temporaEntityStore = temporaEntityStore;
            this.tsiService = tsiService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Detail(string id)
        {
            if(string.IsNullOrEmpty(id))
            {
                return RedirectToAction("Index");
            }
            var entity = temporaEntityStore.Get(id);
            if(entity == null)
            {
                return BadRequest($"{id} not found");
            }
            var envId = "f4db6163-54ce-4171-b020-873a7832fdee";
            var token = await tsiService.GetAccessTokenAsync();
            var viewModel = new TemporaDetailViewModel
            {
                Token = token,
                Tempora = entity,
                TSEnvId = envId
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

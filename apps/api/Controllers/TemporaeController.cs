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
        private readonly IOrgEntityStore<Common.Models.Tempora> temporaEntityStore;
        private readonly ITsiService tsiService;

        public TemporaeController(
            IOrgEntityStore<Amphora.Common.Models.Tempora> temporaEntityStore,
            ITsiService tsiService )
        {
            this.temporaEntityStore = temporaEntityStore;
            this.tsiService = tsiService;
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
                temporae = ( await temporaEntityStore.ListAsync()).ToList();
            }
            
            var viewModel = new TemporaeViewModel
            {
                Temporae = temporae
            };
            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Detail(string id)
        {
            if(string.IsNullOrEmpty(id))
            {
                return RedirectToAction("Index");
            }
            var entity = await temporaEntityStore.GetAsync(id);
            if(entity == null)
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

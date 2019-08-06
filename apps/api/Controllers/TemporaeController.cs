using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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

        public TemporaeController(IDataEntityStore<Amphora.Common.Models.Tempora> temporaEntityStore)
        {
            this.temporaEntityStore = temporaEntityStore;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Detail(string id)
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
            // if (orgId != null)
            // {
            //     temporae = temporaEntityStore.List(orgId).ToList();
            // }
            var viewModel = new TemporaDetailViewModel
            {
                Token = "token",
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

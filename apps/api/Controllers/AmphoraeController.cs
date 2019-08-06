using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Amphora.Api.Contracts;
using Amphora.Api.Models;
using Amphora.Api.ViewModels;
using Microsoft.AspNetCore.Mvc;


namespace Amphora.Api.Controllers
{
    public class AmphoraeController : Controller
    {
        private readonly IDataEntityStore<Common.Models.Amphora> amphoraEntityStore;

        public AmphoraeController(IDataEntityStore<Amphora.Common.Models.Amphora> amphoraEntityStore)
        {
            this.amphoraEntityStore = amphoraEntityStore;
        }
        [HttpGet]
        public IActionResult Index(string orgId)
        {
            List<Amphora.Common.Models.Amphora> myAmphora;
            if (orgId != null)
            {
                myAmphora = amphoraEntityStore.List(orgId).ToList();
            }
            else
            {
                myAmphora = new List<Common.Models.Amphora>()
                {
                    new Common.Models.Amphora
                    {
                        Title = "Hello world",
                        Description = "This should be on the page :)"
                    }
                };
            }
            var viewModel = new MyAmphoraeViewModel
            {
                MyAmphora = myAmphora
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

using System.Collections.Generic;
using System.Diagnostics;
using Amphora.Api.Models;
using Amphora.Api.ViewModels;
using Amphora.Common.Models;
using Microsoft.AspNetCore.Mvc;


namespace Amphora.Api.Controllers
{
    public class MyAmphoraeController : Controller
    {
        public IActionResult Index()
        {
            var viewModel = new MyAmphoraeViewModel
            {
                MyAmphora = new List<Common.Models.Amphora>() 
                {
                    new Common.Models.Amphora 
                    {
                        Title = "Hello world",
                        Description = "This should be on the page :)"
                    }
                }
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

using System.Diagnostics;
using Amphora.Api.Models;
using Microsoft.AspNetCore.Mvc;


namespace Amphora.Api.Controllers
{
    public class MyAmphoraeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

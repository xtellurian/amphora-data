using System.Diagnostics;
using Amphora.Api.Contracts;
using Amphora.Api.Models;
using Amphora.Common.Models;
using Microsoft.AspNetCore.Mvc;


namespace Amphora.Api.Controllers
{
    public class ProfileController : Controller
    {
        private readonly IEntityStore<Organisation> orgStore;
        private readonly IOrgEntityStore<User> userStore;

        public ProfileController(IEntityStore<Organisation> orgStore, IOrgEntityStore<User> userStore)
        {
            this.orgStore = orgStore;
            this.userStore = userStore;
        }
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

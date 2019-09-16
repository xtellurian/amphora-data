using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Common.Models.Organisations;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace Amphora.Api.Controllers
{
    [ApiController]
    [Authorize]
    public class ImagesController : Controller
    {
        private readonly IOrganisationService organisationService;

        public ImagesController(IOrganisationService organisationService)
        {
            this.organisationService = organisationService;
        }

        [HttpGet("api/organisations/{id}/profile.jpg")]
        public async Task<IActionResult> GetOrganisationProfileImage(string id)
        {
            var org = await organisationService.Store.ReadAsync(id);
            if (org == null) return NotFound();
            var image = await organisationService.ReadrofilePictureJpg(org);
            return new FileContentResult(image, "image/jpeg");
        }


    }
}

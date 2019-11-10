using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Common.Models.Organisations;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;

namespace Amphora.Api.Controllers
{
    [ApiController]
    [Authorize]
    [SkipStatusCodePages]
    public class ImagesController : Controller
    {
        private readonly IOrganisationService organisationService;

        public ImagesController(IOrganisationService organisationService)
        {
            this.organisationService = organisationService;
        }

        /// <summary>
        /// Gets an organisations profile picture
        /// </summary>
        /// <param name="id">Amphora Id</param>  
        [HttpGet("api/organisations/{id}/profile.jpg")]
        [OpenApiIgnore]
        public async Task<IActionResult> GetOrganisationProfileImage(string id)
        {
            var org = await organisationService.Store.ReadAsync(id);
            if (org == null) return NotFound();
            var image = await organisationService.ReadrofilePictureJpg(org);
            return new FileContentResult(image, "image/jpeg");
        }


    }
}

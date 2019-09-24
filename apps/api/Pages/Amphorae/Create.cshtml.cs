using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Api.Models.Dtos.Amphorae;
using Amphora.Common.Models.Amphorae;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace Amphora.Api.Pages.Amphorae
{
    [ValidateAntiForgeryToken]
    [Authorize]
    public class CreateModel : PageModel
    {
        private readonly IAmphoraeService amphoraeService;
        private readonly IAuthenticateService authenticateService;
        private readonly ILogger<CreateModel> logger;
        private readonly IMapper mapper;

        public CreateModel(
            IAmphoraeService amphoraeService,
            IAuthenticateService authenticateService,
            ILogger<CreateModel> logger,
            IMapper mapper)
        {
            this.amphoraeService = amphoraeService;
            this.authenticateService = authenticateService;
            this.logger = logger;
            this.mapper = mapper;
        }

        [BindProperty]
        public AmphoraDto Input { get; set; }
        public string Token { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var response = await authenticateService.GetToken(User);
            if (response.success)
            {
                Token = response.token;
            }
            else
            {
                logger.LogError("Couldn't get token");
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                GeoLocation location = null;
                if (Input.Lat.HasValue && Input.Lon.HasValue)
                {
                    location =  new GeoLocation(Input.Lon.Value, Input.Lat.Value);
                }
                var entity = new AmphoraExtendedModel
                {
                    Name = Input.Name,
                    Description = Input.Description,
                    GeoLocation = location,
                    Price = Input.Price,
                    IsPublic = true
                };

                var setResult = await amphoraeService.CreateAsync(User, entity);
                if (setResult.Succeeded)
                {
                    return RedirectToPage("/Amphorae/Index");
                }
                else
                {
                    foreach(var e in setResult.Errors)
                    {
                        ModelState.AddModelError(string.Empty, e);
                    }
                    return Page();
                }
            }
            else
            {
                return Page();
            }
        }
    }
}
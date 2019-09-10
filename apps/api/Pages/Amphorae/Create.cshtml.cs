using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Common.Models;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using NGeoHash;

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

        public class InputModel
        {
            [Required]
            public string Title { get; set; }
            [Required]
            [DataType(DataType.MultilineText)]
            public string Description { get; set; }
            [Required]
            [DataType(DataType.Currency)]
            public double Price { get; set; }

            [Display(Name = "Latitude")]
            public double? Lat { get; set; }
            [Display(Name = "Longitude")]
            public double? Lon { get; set; }
        }
        [BindProperty]
        public InputModel Input { get; set; }
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
                string geoHash = null;
                if (Input.Lat.HasValue && Input.Lon.HasValue)
                {
                    geoHash = GeoHash.Encode(Input.Lat.Value, Input.Lon.Value);
                }
                var entity = new Amphora.Common.Models.Amphora
                {
                    Title = Input.Title,
                    Description = Input.Description,
                    GeoHash = geoHash,
                    Price = Input.Price,
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
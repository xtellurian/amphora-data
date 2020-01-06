using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Api.Models.Dtos.Amphorae;
using Amphora.Common.Models.Amphorae;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;

namespace Amphora.Api.Areas.Amphorae.Pages
{
    [ValidateAntiForgeryToken]
    [Authorize]
    public class CreateModel : PageModel
    {
        private readonly IAmphoraeService amphoraeService;
        private readonly IUserService userService;
        private readonly ILogger<CreateModel> logger;
        private readonly IMapper mapper;

        public CreateModel(
            IAmphoraeService amphoraeService,
            IUserService userService,
            ILogger<CreateModel> logger,
            IMapper mapper)
        {
            this.amphoraeService = amphoraeService;
            this.userService = userService;
            this.logger = logger;
            this.mapper = mapper;
        }

        [BindProperty]
        public CreateAmphoraDto AmphoraDto { get; set; }
        public List<SelectListItem> TermsAndConditions { get; set; } = new List<SelectListItem>();
        public string Token { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await userService.ReadUserModelAsync(User);
            var items = user.Organisation.TermsAndConditions.Select(_ => new SelectListItem(_.Name, _.Id));
            this.TermsAndConditions = new List<SelectListItem>(items);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                GeoLocation location = null;
                if (AmphoraDto.Lat.HasValue && AmphoraDto.Lon.HasValue)
                {
                    location = new GeoLocation(AmphoraDto.Lon.Value, AmphoraDto.Lat.Value);
                }
                var entity = new AmphoraModel(AmphoraDto.Name, AmphoraDto.Description, AmphoraDto.Price, null, null, AmphoraDto.TermsAndConditionsId)
                {
                    GeoLocation = location,
                    IsPublic = true,
                };
                entity.Labels = AmphoraDto.GetLabels();

                var setResult = await amphoraeService.CreateAsync(User, entity);
                if (setResult.Succeeded)
                {
                    return RedirectToPage("./Index");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, setResult.Message);
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
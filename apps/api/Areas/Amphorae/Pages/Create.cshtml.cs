using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amphora.Api.AspNet;
using Amphora.Api.Contracts;
using Amphora.Api.Models.Dtos.Amphorae;
using Amphora.Common.Contracts;
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
    [CommonAuthorize]
    public class CreateModel : PageModel
    {
        private readonly IAmphoraeService amphoraeService;
        private readonly IUserDataService userDataService;
        private readonly ILogger<CreateModel> logger;
        private readonly IMapper mapper;

        public CreateModel(
            IAmphoraeService amphoraeService,
            IUserDataService userDataService,
            ILogger<CreateModel> logger,
            IMapper mapper)
        {
            this.amphoraeService = amphoraeService;
            this.userDataService = userDataService;
            this.logger = logger;
            this.mapper = mapper;
        }

        [BindProperty]
        public CreateAmphora AmphoraDto { get; set; }
        public List<SelectListItem> TermsOfUses { get; set; } = new List<SelectListItem>();
        public string Token { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            await LoadTermsAndConditions();
            return Page();
        }

        private async Task LoadTermsAndConditions()
        {
            var userReadRes = await userDataService.ReadAsync(User);
            if (userReadRes.Succeeded)
            {
                var items = userReadRes.Entity.Organisation?.TermsOfUses?.Select(_ => new SelectListItem(_.Name, _.Id));
                if (items != null)
                {
                    this.TermsOfUses = new List<SelectListItem>();
                }
            }
            else
            {
                throw new System.ApplicationException(userReadRes.Message);
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            await LoadTermsAndConditions();
            if (ModelState.IsValid)
            {
                GeoLocation location = null;
                if (AmphoraDto.Lat.HasValue && AmphoraDto.Lon.HasValue)
                {
                    location = new GeoLocation(AmphoraDto.Lon.Value, AmphoraDto.Lat.Value);
                }

                var entity = new AmphoraModel(AmphoraDto.Name, AmphoraDto.Description, AmphoraDto.Price, null, null, AmphoraDto.TermsOfUseId)
                {
                    GeoLocation = location,
                    IsPublic = true,
                };
                entity.Labels = AmphoraDto.GetLabels();

                var setResult = await amphoraeService.CreateAsync(User, entity);
                if (setResult.Succeeded)
                {
                    return RedirectToPage("./Detail/Index", new { id = setResult.Entity.Id });
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
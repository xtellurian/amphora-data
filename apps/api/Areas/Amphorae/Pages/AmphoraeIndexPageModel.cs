using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Api.Pages;
using Amphora.Common.Contracts;
using Amphora.Common.Models;
using Amphora.Common.Models.Amphorae;
using Amphora.Common.Models.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Amphora.Api.Areas.Amphorae.Pages
{
    public abstract class AmphoraeIndexPageModel : PageModelBase
    {
        protected const int DefaultTop = 8;
        protected readonly IAmphoraeService amphoraeService;
        private readonly IUserDataService userDataService;

        public AmphoraeIndexPageModel(IAmphoraeService amphoraeService, IUserDataService userDataService)
        {
            this.amphoraeService = amphoraeService;
            this.userDataService = userDataService;
        }

        public IEnumerable<AmphoraModel> Amphorae { get; set; } = new List<AmphoraModel>();
        public int? Count { get; set; } = null;
        public int? Skip { get; set; } = 0;
        public int? Top { get; set; } = DefaultTop;

        public int TotalSkip => (Skip ?? 0) * (Top ?? DefaultTop);
        protected Error Error { get; set; } = null;

        protected async Task<IActionResult> MyAmphorae()
        {
            var userReadRes = await userDataService.ReadAsync(User);
            if (!userReadRes.Succeeded)
            {
                return RedirectToPage("/Account/Login", new { area = "Profiles" });
            }

            // get my amphora
            var amphorae = amphoraeService.AmphoraStore.Query(a => a.CreatedById == userReadRes.Entity.Id);
            Count = await amphorae.CountAsync();
            this.Amphorae = await amphorae.Skip(TotalSkip).Take(Top ?? 0).ToListAsync();
            return Page();
        }

        protected async Task<IActionResult> MyPurchasedAmphorae()
        {
            var userReadRes = await userDataService.ReadAsync(User);
            if (!userReadRes.Succeeded) { return RedirectToPage("/Account/Login", new { area = "Profiles" }); }

            var amphorae = await amphoraeService.AmphoraPurchasedBy(User, userReadRes.Entity);
            Count = await amphorae.CountAsync();
            this.Amphorae = await amphorae.Skip(TotalSkip).Take(Top ?? 0).ToListAsync();
            return Page();
        }

        protected async Task<IActionResult> OrgAmphorae()
        {
            var userReadRes = await userDataService.ReadAsync(User);
            if (!userReadRes.Succeeded)
            {
                return RedirectToPage("/Account/Login", new { area = "Profiles" });
            }

            // get my amphora
            var amphorae = amphoraeService.AmphoraStore.Query(a => a.OrganisationId == userReadRes.Entity.OrganisationId);
            Count = await amphorae.CountAsync();
            this.Amphorae = await amphorae.Skip(TotalSkip).Take(Top ?? 0).ToListAsync();
            return Page();
        }
    }
}

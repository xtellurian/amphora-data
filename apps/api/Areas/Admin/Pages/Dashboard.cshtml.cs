using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Amphora.Api.AspNet;
using Amphora.Api.Contracts;
using Amphora.Common.Models.Amphorae;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace Amphora.Api.Areas.Admin.Pages
{
    [GlobalAdminAuthorize]
    public class DashboardPageModel : PageModel
    {
        private readonly IAmphoraeService amphoraeService;
        private readonly IOrganisationService organisationService;
        private readonly IUserService userService;
        private readonly IMemoryCache memoryCache;

        public StatisticsModel Stats { get; set; } = new StatisticsModel();

        public DashboardPageModel(IAmphoraeService amphoraeService,
                                  IOrganisationService organisationService,
                                  IUserService userService,
                                  IMemoryCache memoryCache)
        {
            this.amphoraeService = amphoraeService;
            this.organisationService = organisationService;
            this.userService = userService;
            this.memoryCache = memoryCache;
        }
        private DateTime startOfMonth = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1, 0, 0, 0, 0);

        public async Task<IActionResult> OnGetAsync()
        {
            if( memoryCache.TryGetValue(nameof(Stats), out StatisticsModel stats ))
            {
                this.Stats = stats;
            }
            else
            {
                await LoadAmphoraStats();
                await LoadUserStats();
                var entry = memoryCache.CreateEntry(nameof(Stats));
                entry.Value = Stats;
                entry.SetAbsoluteExpiration(DateTime.Now.AddHours(1));
                memoryCache.Set(nameof(Stats), entry);
            }
            return Page();
        }

        private async Task LoadUserStats()
        {
            this.Stats.UsersLoggedInThisMonth =
                await organisationService.Store.CountAsync(o => o.Memberships.Any(m => m.User.LastLoggedIn > startOfMonth));


        }

        private async Task LoadAmphoraStats()
        {
            Expression<Func<AmphoraModel, bool>> activeExpression = a => a.LastModified > startOfMonth;

            this.Stats.MeanPriceOfActiveAmphora = 
                await this.amphoraeService.AmphoraStore.Query(activeExpression).AverageAsync(a => a.Price);

            this.Stats.AmphoraCount = await this.amphoraeService.AmphoraStore.CountAsync();
            this.Stats.AmphoraModifiedThisMonthCount = await this.amphoraeService.AmphoraStore.CountAsync(activeExpression);
        }
    }

    public class StatisticsModel
    {
        public int AmphoraCount { get; set; }
        public int AmphoraModifiedThisMonthCount { get; set; }
        public double? MeanPriceOfActiveAmphora { get; set; }
        public int UsersLoggedInThisMonth { get; set; }
    }
}
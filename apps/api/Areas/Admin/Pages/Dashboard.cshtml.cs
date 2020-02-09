using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Amphora.Api.AspNet;
using Amphora.Api.Contracts;
using Amphora.Common.Contracts;
using Amphora.Common.Models.Amphorae;
using Amphora.Common.Models.Users;
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

        public StatisticsCollection Stats { get; set; } = new StatisticsCollection();

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

        private Expression<Func<T, bool>> Active<T>() where T : IEntity => a => a.LastModified > monthAgo;
        private DateTime monthAgo = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1, 0, 0, 0, 0);

        public async Task<IActionResult> OnGetAsync()
        {
            if (memoryCache.TryGetValue(nameof(Stats), out StatisticsCollection stats))
            {
                this.Stats = stats;
            }
            else
            {
                await LoadAmphoraStats();
                await LoadUserStats();
                await LoadOrganisationStats();
                await LoadDebitStats();
                this.Stats.GeneratedTime = DateTime.Now;

                var cacheEntryOptions = new MemoryCacheEntryOptions()
                     .SetAbsoluteExpiration(DateTime.Now.AddHours(1));
                memoryCache.Set(nameof(Stats), this.Stats, cacheEntryOptions);
            }

            return Page();
        }

        private async Task LoadUserStats()
        {
            this.Stats.Users.TotalCount = await userService.UserStore.CountAsync();
            this.Stats.Users.ActiveCount = await userService.UserStore.CountAsync(Active<ApplicationUser>());
        }

        private async Task LoadOrganisationStats()
        {
            this.Stats.Organisations.TotalCount = await organisationService.Store.CountAsync();
            this.Stats.Organisations.ActiveCount = (await userService.UserStore
                .Query(Active<ApplicationUser>())
                .Select(_ => _.OrganisationId)
                .ToListAsync()) // switch to client side, can't do distinct in Cosmos?
                .Distinct()
                .Count();

            this.Stats.Organisations.AggregateBalance = (await organisationService.Store
                .Query(_ => true)
                .ToListAsync()) // switch to client side, can't do distinct in Cosmos?
                .Average(_ => _.Account.Balance);

            this.Stats.Organisations.AggregateBalance = (await organisationService.Store
                .Query(_ => true)
                .ToListAsync()) // switch to client side, can't do distinct in Cosmos?
                .Sum(_ => _.Account.Balance);
        }

        private async Task LoadDebitStats()
        {
            this.Stats.Debits.TotalCount = (await organisationService.Store
                .Query(_ => true) // for all orgs
                .ToListAsync())
                .Where(_ => _.Account != null)
                .SelectMany(_ => _.Account.Debits)
                .Count();

            this.Stats.Debits.ActiveCount = (await organisationService.Store
                .Query(_ => true) // for all orgs
                .ToListAsync())
                .Where(_ => _.Account != null)
                .SelectMany(_ => _.Account.Debits)
                .Where(_ => _.CreatedDate > monthAgo)
                .Count();

            this.Stats.Debits.MeanActivePrice = (await organisationService.Store
                .Query(_ => true) // for all orgs
                .ToListAsync())
                .Where(_ => _.Account != null)
                .SelectMany(_ => _.Account.Debits)
                .Where(_ => _.CreatedDate > monthAgo)
                .Average(_ => _.Amount);
        }

        private async Task LoadAmphoraStats()
        {
            this.Stats.Amphorae.MeanActivePrice =
                await this.amphoraeService.AmphoraStore.Query(Active<AmphoraModel>()).AverageAsync(a => a.Price);

            this.Stats.Amphorae.TotalCount = await this.amphoraeService.AmphoraStore.CountAsync();
            this.Stats.Amphorae.ActiveCount = await this.amphoraeService.AmphoraStore.CountAsync(Active<AmphoraModel>());
        }
    }

    public class StatisticsCollection
    {
        public DateTimeOffset? GeneratedTime { get; set; }
        public StatisticsGroup Amphorae { get; set; } = new StatisticsGroup();
        public StatisticsGroup Users { get; set; } = new StatisticsGroup();
        public OrgStatsGroup Organisations { get; set; } = new OrgStatsGroup();
        public StatisticsGroup Debits { get; set; } = new StatisticsGroup();
    }

    public class OrgStatsGroup : StatisticsGroup
    {
        public double? AggregateBalance { get; set; }
        public double? MeanBalance { get; set; }
    }

    public class StatisticsGroup
    {
        public int? TotalCount { get; set; }
        public int? ActiveCount { get; set; }
        public double? MeanActivePrice { get; set; }
    }
}

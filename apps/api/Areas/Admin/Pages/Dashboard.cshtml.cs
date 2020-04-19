using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Amphora.Api.AspNet;
using Amphora.Api.Contracts;
using Amphora.Common.Contracts;
using Amphora.Common.Models.Amphorae;
using Amphora.Common.Models.Purchases;
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
        private readonly IEntityStore<PurchaseModel> purchaseStore;
        private readonly IEntityStore<CommissionModel> commissionStore;
        private readonly IUserDataService userDataService;
        private readonly ICache cache;

        public StatisticsCollection Stats { get; set; } = new StatisticsCollection();

        public DashboardPageModel(IAmphoraeService amphoraeService,
                                  IOrganisationService organisationService,
                                  IEntityStore<PurchaseModel> purchaseStore,
                                  IEntityStore<CommissionModel> commissionStore,
                                  IUserDataService userDataService,
                                  ICache cache)
        {
            this.amphoraeService = amphoraeService;
            this.organisationService = organisationService;
            this.purchaseStore = purchaseStore;
            this.commissionStore = commissionStore;
            this.userDataService = userDataService;
            this.cache = cache;
        }

        private Expression<Func<T, bool>> Active<T>() where T : IEntity => a => a.LastModified > monthAgo;
        private DateTime monthAgo = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1, 0, 0, 0, 0);

        public async Task<IActionResult> OnGetAsync()
        {
            if (cache.TryGetValue(nameof(Stats), out StatisticsCollection stats))
            {
                this.Stats = stats;
            }
            else
            {
                await LoadAmphoraStats();
                await LoadUserStats();
                await LoadOrganisationStats();
                await LoadDebitStats();
                await LoadPurchaseStats();
                this.Stats.GeneratedTime = DateTime.Now;

                cache.Compact();
                cache.Set(nameof(Stats), this.Stats, DateTime.Now.AddHours(1));
            }

            return Page();
        }

        private async Task LoadUserStats()
        {
            this.Stats.Users.TotalCount = await userDataService.Query(User, _ => true).CountAsync();
        }

        private async Task LoadOrganisationStats()
        {
            this.Stats.Organisations.TotalCount = await organisationService.Store.CountAsync();

            this.Stats.Organisations.MeanBalance = (await organisationService.Store
                .Query(_ => true)
                .ToListAsync()) // switch to client side, can't do distinct in Cosmos?
                .Average(_ => _.Account.Balance);

            this.Stats.Organisations.AggregateBalance = (await organisationService.Store
                .Query(_ => true)
                .ToListAsync()) // switch to client side, can't do distinct in Cosmos?
                .Sum(_ => _.Account.Balance);
        }

        private async Task LoadPurchaseStats()
        {
            this.Stats.Purchases.TotalCount = await purchaseStore.CountAsync();
            this.Stats.Purchases.ActiveCount = await purchaseStore.CountAsync(Active<PurchaseModel>());
            this.Stats.Purchases.MeanActivePrice =
                (await purchaseStore.QueryAsync(Active<PurchaseModel>()))
                .Average(_ => _.Price);
        }

        private async Task LoadCommissionStats()
        {
            this.Stats.Commissions.TotalCount = await commissionStore.CountAsync();
            this.Stats.Commissions.ActiveCount = await commissionStore.CountAsync(Active<CommissionModel>());
            this.Stats.Commissions.MeanActivePrice =
                (await commissionStore.QueryAsync(Active<CommissionModel>()))
                .Average(_ => _.Amount);
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
        public CountableGroup Users { get; set; } = new CountableGroup();
        public OrgStatsGroup Organisations { get; set; } = new OrgStatsGroup();
        public StatisticsGroup Debits { get; set; } = new StatisticsGroup();
        public StatisticsGroup Purchases { get; set; } = new StatisticsGroup();
        public StatisticsGroup Commissions { get; set; } = new StatisticsGroup();
    }

    public class CountableGroup
    {
        public int? TotalCount { get; set; }
    }

    public class OrgStatsGroup : CountableGroup
    {
        public double? AggregateBalance { get; set; }
        public double? MeanBalance { get; set; }
    }

    public class StatisticsGroup : CountableGroup
    {
        public int? ActiveCount { get; set; }
        public double? MeanActivePrice { get; set; }
    }
}

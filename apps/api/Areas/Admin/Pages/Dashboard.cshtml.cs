using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Amphora.Api.AspNet;
using Amphora.Api.Contracts;
using Amphora.Common.Contracts;
using Amphora.Common.Models.Amphorae;
using Amphora.Common.Models.Purchases;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

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
        private readonly ITelemetry telemetry;
        private readonly ICache cache;

        public StatisticsCollection Stats { get; set; } = new StatisticsCollection();

        public DashboardPageModel(IAmphoraeService amphoraeService,
                                  IOrganisationService organisationService,
                                  IEntityStore<PurchaseModel> purchaseStore,
                                  IEntityStore<CommissionModel> commissionStore,
                                  IUserDataService userDataService,
                                  ITelemetry telemetry,
                                  ICache cache)
        {
            this.amphoraeService = amphoraeService;
            this.organisationService = organisationService;
            this.purchaseStore = purchaseStore;
            this.commissionStore = commissionStore;
            this.userDataService = userDataService;
            this.telemetry = telemetry;
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
                TrackMetrics(this.Stats);
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

        private Task LoadSignalsStats()
        {
            long total = 0;
            var uniqueCount = new Dictionary<string, long>();
            foreach (var a in amphoraeService.AmphoraStore.Query(_ => true))
            {
                // go through every amphora
                total += a.V2Signals.Count;
                foreach (var s in a.V2Signals)
                {
                    if (!uniqueCount.ContainsKey(s.Property))
                    {
                        uniqueCount[s.Property] = 0; // initialise entry
                    }

                    uniqueCount[s.Property] += 1;
                }
            }

            this.Stats.Signals.TotalCount = total;
            this.Stats.Signals.TotalUniqueCount = uniqueCount.Keys.Count;
            return Task.CompletedTask;
        }

        private void TrackMetrics(StatisticsCollection collection)
        {
            TrackMetric("Amphorae", collection.Amphorae);
            TrackMetric("Commissions", collection.Commissions);
            TrackMetric("Debits", collection.Debits);
            TrackMetric("Organisations", collection.Organisations);
            TrackMetric("Purchases", collection.Purchases);
            TrackMetric("Signals", collection.Signals);
            TrackMetric("Users", collection.Users);
        }

        private void TrackMetric(string name, PricedGroup group)
        {
            telemetry.TrackMetricValue($"{name}.ActiveCount", group.ActiveCount);
            telemetry.TrackMetricValue($"{name}.MeanActivePrice", group.MeanActivePrice);
            TrackMetric(name, (CountableGroup)group);
        }

        private void TrackMetric(string name, UniqueCountableGroup group)
        {
            telemetry.TrackMetricValue($"{name}.TotalUniqueCount", group.TotalUniqueCount);
            TrackMetric(name, (CountableGroup)group);
        }

        private void TrackMetric(string name, CountableGroup group)
        {
            telemetry.TrackMetricValue($"{name}.TotalCount", group.TotalCount);
        }
    }

    public class StatisticsCollection
    {
        public DateTimeOffset? GeneratedTime { get; set; }
        public PricedGroup Amphorae { get; set; } = new PricedGroup();
        public CountableGroup Users { get; set; } = new CountableGroup();
        public OrgStatsGroup Organisations { get; set; } = new OrgStatsGroup();
        public PricedGroup Debits { get; set; } = new PricedGroup();
        public PricedGroup Purchases { get; set; } = new PricedGroup();
        public PricedGroup Commissions { get; set; } = new PricedGroup();
        public UniqueCountableGroup Signals { get; set; } = new UniqueCountableGroup();
    }

    public class CountableGroup
    {
        public long? TotalCount { get; set; }
    }

    public class UniqueCountableGroup : CountableGroup
    {
        public long? TotalUniqueCount { get; set; }
    }

    public class OrgStatsGroup : CountableGroup
    {
        public double? AggregateBalance { get; set; }
        public double? MeanBalance { get; set; }
    }

    public class PricedGroup : CountableGroup
    {
        public int? ActiveCount { get; set; }
        public double? MeanActivePrice { get; set; }
    }
}

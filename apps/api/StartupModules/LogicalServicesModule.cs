using System.Linq;
using Amphora.Api.Contracts;
using Amphora.Api.Services.Amphorae;
using Amphora.Api.Services.Applications;
using Amphora.Api.Services.Feeds;
using Amphora.Api.Services.Organisations;
using Amphora.Api.Services.Purchases;
using Amphora.Common.Contracts;
using Amphora.Common.Services.Access;
using Amphora.Common.Services.Activities;
using Amphora.Common.Services.Plans;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Amphora.Api.StartupModules
{
    public class LogicalServicesModule
    {
        public LogicalServicesModule(IConfiguration configuration, IWebHostEnvironment env)
        {
            this.HostingEnvironment = env;
            this.Configuration = configuration;
        }

        public IWebHostEnvironment HostingEnvironment { get; }
        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IDateTimeProvider, Common.Services.Timing.DateTimeProvider>();
            services.AddTransient<IAmphoraeService, AmphoraeService>();
            services.AddTransient<IAmphoraFileService, AmphoraFileService>();
            services.AddTransient<IOrganisationService, OrganisationService>();
            services.AddTransient<IPurchaseService, PurchaseService>();
            services.AddTransient<ICommissionTrackingService, CommissionTrackingService>();
            services.AddTransient<IAccountsService, AccountsService>();
            services.AddTransient<IInvoiceFileService, InvoiceFileService>();
            services.AddTransient<IQualityEstimatorService, QualityEstimatorService>();
            services.AddTransient<IAccessControlService, AccessControlService>();
            services.AddTransient<IPlanLimitService, PlanLimitService>();
            services.AddTransient<ITermsOfUseService, TermsOfUseService>();
            services.AddTransient<IActivityService, ActivityService>();
            services.AddTransient<IActivityRunService, ActivityRunService>();
            services.AddTransient<IApplicationService, ApplicationService>();
            services.AddScoped<IFeedAggregator, FeedAggregatorService>();
            services.AddScoped<IAmphoraFeedService, AmphoraFeedService>();
        }

        public void Configure(IApplicationBuilder app)
        { }
    }
}
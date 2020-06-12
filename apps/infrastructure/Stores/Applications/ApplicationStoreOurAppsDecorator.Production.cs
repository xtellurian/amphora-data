using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Amphora.Common.Contracts;
using Amphora.Common.Extensions;
using Amphora.Common.Models.Applications;
using Amphora.Common.Models.Options;
using Amphora.Common.Models.Platform;
using Amphora.Common.Stores.EFCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Amphora.Identity.Stores.EFCore
{
    public class ApplicationStoreOurAppsDecoratorProduction : StoreDecorator<ApplicationModel>
    {
        private const string AppUrl = "app.amphoradata.com";
        private readonly ExternalServices externalServices;
        private readonly EnvironmentInfo envInfo;
        private readonly OAuthClientSecret mvcClientSecret;
        private readonly ILogger<ApplicationStoreOurAppsDecoratorProduction> logger;

        public ApplicationStoreOurAppsDecoratorProduction(IEntityStore<ApplicationModel> store,
                                                          ILogger<ApplicationStoreOurAppsDecoratorProduction> logger,
                                                          IOptions<ExternalServices> externalServices,
                                                          IOptions<EnvironmentInfo> envInfo,
                                                          IOptions<OAuthClientSecret> mvcClientSecret) : base(store)
        {
            this.externalServices = externalServices.Value;
            this.envInfo = envInfo.Value;
            this.mvcClientSecret = mvcClientSecret.Value;
            this.logger = logger;
            logger.LogInformation("Using Production Our Apps decorator.");
        }

        private List<ApplicationModel> OurApps()
        {
            return new List<ApplicationModel>
            {
                new ApplicationModel
                {
                    Id = Common.Security.OAuthClients.SPA,
                    Name = "SPA Client",
                    AllowOffline = true,
                    RequireConsent = false,
                    RedirectUris = StandardRedirectUrls("/#/callback", "/silentRenew.html"),
                    LogoutUrl = StandardLogoutUrl(),
                    PostLogoutRedirects = StandardPostLogoutRedirects()
                }
            };
        }

        public override async Task<ApplicationModel?> ReadAsync(string id)
        {
            var am = await base.ReadAsync(id);
            return am ?? OurApps().FirstOrDefault(_ => _.Id == id);
        }

        public override async Task<IEnumerable<ApplicationModel>> QueryAsync(Expression<Func<ApplicationModel, bool>> where)
        {
            return await this.Query(where).ToListAsync();
        }

        public override IQueryable<ApplicationModel> Query(Expression<Func<ApplicationModel, bool>> where)
        {
            var ours = OurApps().Where(_ => where.Compile()(_));
            var q = base.Query(where);
            return q.Union(ours);
        }

        protected ICollection<string> StandardRedirectUrls(params string[] callbackPaths)
        {
            if (callbackPaths == null)
            {
                throw new System.ArgumentNullException($"{nameof(callbackPaths)} must not be null.");
            }

            var urls = new List<string>();
            if (string.IsNullOrEmpty(envInfo.Location))
            {
                foreach (var p in callbackPaths)
                {
                    urls.Add($"{envInfo.Stack}.{AppUrl}".ToUri().ToStandardString() + p);
                }
            }
            else
            {
                urls.Add($"{envInfo.Stack}.{envInfo.Location}.{AppUrl}".ToUri().ToStandardString() + "/signin-oidc");
            }

            urls.Add(AppUrl.ToUri().ToStandardString() + "/signin-oidc");

            if (!string.IsNullOrEmpty(envInfo.Stack))
            {
                // add something like develop.app.amphoradata.com
                urls.Add($"{envInfo.Stack}.{AppUrl}".ToUri().ToStandardString() + "/signin-oidc");
            }

            logger.LogInformation($"Redirects: {JsonConvert.SerializeObject(urls, Formatting.Indented)}");
            return urls;
        }

        protected ICollection<string> StandardPostLogoutRedirects()
        {
            var urls = new List<string>();

            urls.Add($"{envInfo.Stack}.{envInfo.Location}.{AppUrl}".ToUri().ToStandardString() + "/signout-callback-oidc");
            urls.Add(AppUrl.ToUri().ToStandardString() + "/signout-callback-oidc");

            if (!string.IsNullOrEmpty(envInfo.Stack))
            {
                urls.Add($"{envInfo.Stack}.{AppUrl}".ToUri().ToStandardString() + "/signout-callback-oidc");
            }

            logger.LogInformation($"Post Logout Redirects: {JsonConvert.SerializeObject(urls, Formatting.Indented)}");
            return urls;
        }

        protected string StandardLogoutUrl()
        {
            string logoutRedirect;

            if (!string.IsNullOrEmpty(externalServices.WebAppBaseUrl))
            {
                logoutRedirect = externalServices.WebAppUri().ToStandardString();
            }
            else if (envInfo.Stack?.ToLower() == "prod")
            {
                logoutRedirect = $"{AppUrl.ToUri().ToStandardString()}/signout-oidc";
            }
            else if (!string.IsNullOrEmpty(envInfo.Stack))
            {
                logoutRedirect = $"{envInfo.Stack}.{AppUrl}".ToUri().ToStandardString() + "/signout-oidc";
            }
            else
            {
                var app = $"{envInfo.Stack}.{envInfo.Location}.{AppUrl}".ToUri().ToStandardString();
                logoutRedirect = $"{app}/signout-oidc";
            }

            logger.LogInformation($"Logout Redirect: {logoutRedirect}");
            return logoutRedirect;
        }
    }
}
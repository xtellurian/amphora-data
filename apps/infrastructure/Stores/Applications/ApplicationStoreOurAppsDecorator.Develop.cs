using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Amphora.Common.Contracts;
using Amphora.Common.Extensions;
using Amphora.Common.Models.Applications;
using Amphora.Common.Stores.EFCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Amphora.Identity.Stores.EFCore
{
    public class ApplicationStoreOurAppsDecoratorDevelop : StoreDecorator<ApplicationModel>
    {
        private readonly ILogger<ApplicationStoreOurAppsDecoratorDevelop> logger;

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

        public ApplicationStoreOurAppsDecoratorDevelop(IEntityStore<ApplicationModel> store,
                                                       ILogger<ApplicationStoreOurAppsDecoratorDevelop> logger) : base(store)
        {
            this.logger = logger;
            logger.LogWarning("Using development decorator for Applications Models");
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

        private ICollection<string> StandardRedirectUrls(params string[] callbackPaths)
        {
            var urls = new List<string>();
            if (callbackPaths == null)
            {
                throw new System.ArgumentNullException($"{nameof(callbackPaths)} must not be null.");
            }

            foreach (var p in callbackPaths)
            {
                urls.Add("localhost:5001".ToUri().ToStandardString() + p);
            }

            return urls;
        }

        private ICollection<string> StandardPostLogoutRedirects()
        {
            var urls = new List<string>();
            urls.Add("localhost:5001".ToUri().ToStandardString() + "/signout-callback-oidc");
            return urls;
        }

        private string StandardLogoutUrl()
        {
            var logoutRedirect = "localhost:5001".ToUri().ToStandardString() + "/signout-oidc";
            return logoutRedirect;
        }
    }
}
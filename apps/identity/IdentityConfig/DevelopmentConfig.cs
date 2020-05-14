using System.Collections.Generic;
using System.Linq;
using Amphora.Common.Extensions;
using Amphora.Common.Security;
using IdentityServer4.Models;
using Newtonsoft.Json;

namespace Amphora.Identity.IdentityConfig
{
    internal class DevelopmentConfig : ConfigBase, IIdentityServerConfig
    {
        public DevelopmentConfig(string mvcClientSecret) : base(mvcClientSecret)
        { }

        public override IEnumerable<IdentityResource> IdentityResources()
        {
            return base.IdentityResources();
        }

        public override IEnumerable<ApiResource> Apis()
        {
            return base.Apis();
        }

        protected override ICollection<string> StandardRedirectUrls(params string[] callbackPaths)
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

            System.Console.WriteLine($"Redirects: {JsonConvert.SerializeObject(urls, Formatting.Indented)}");
            return urls;
        }

        protected override ICollection<string> StandardPostLogoutRedirects()
        {
            var urls = new List<string>();
            urls.Add("localhost:5001".ToUri().ToStandardString() + "/signout-callback-oidc");

            System.Console.WriteLine($"Post Logout Redirects: {JsonConvert.SerializeObject(urls, Formatting.Indented)}");
            return urls;
        }

        protected override string StandardLogoutUrl()
        {
            var logoutRedirect = "localhost:5001".ToUri().ToStandardString() + "/signout-oidc";
            System.Console.WriteLine($"Logout Redirect: {logoutRedirect}");
            return logoutRedirect;
        }
    }
}
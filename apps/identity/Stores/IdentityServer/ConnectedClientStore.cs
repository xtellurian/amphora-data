using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amphora.Common.Contracts;
using Amphora.Common.Models.Applications;
using Amphora.Common.Security;
using IdentityServer4.Models;
using IdentityServer4.Stores;

namespace Amphora.Identity.Stores.IdentityServer
{
    public class ConnectedClientStore : IClientStore
    {
        private readonly IEntityStore<ApplicationModel> store;

        public ConnectedClientStore(IEntityStore<ApplicationModel> store)
        {
            this.store = store;
        }

        public async Task<Client?> FindClientByIdAsync(string clientId)
        {
            var model = await store.ReadAsync(clientId);
            return ToClient(model);
        }

        // below: need to map ApplicationModel to Client
        private Client? ToClient(ApplicationModel? model)
        {
            if (model == null)
            {
                return null;
            }

            return new Client
            {
                ClientId = model.Id,
                ClientName = model.Name,
                AllowAccessTokensViaBrowser = true,
                AllowedGrantTypes = GrantTypes.Code,
                RequirePkce = true,
                AllowOfflineAccess = model.AllowOffline ?? false,
                RequireConsent = model.RequireConsent ?? true,
                RedirectUris = model.RedirectUris(),
                FrontChannelLogoutUri = model.LogoutUrl,
                PostLogoutRedirectUris = model.PostLogoutRedirects(),
                AllowedCorsOrigins = model.Origins,
                RequireClientSecret = false,
                AllowedScopes = CommonScopes(model)
            };
        }

        private List<string> CommonScopes(ApplicationModel model)
        {
            var scopes = new List<string>
            {
                IdentityModel.OidcConstants.StandardScopes.OpenId,
                IdentityModel.OidcConstants.StandardScopes.Profile,
                IdentityModel.OidcConstants.StandardScopes.Email,
                Common.Security.Resources.WebApi,
                Scopes.AmphoraScope
            };

            if (model.AllowOffline == true)
            {
                scopes.Add(IdentityModel.OidcConstants.StandardScopes.OfflineAccess);
            }

            return scopes;
        }
    }
}
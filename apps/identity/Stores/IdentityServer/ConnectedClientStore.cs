using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amphora.Common.Contracts;
using Amphora.Common.Models.Applications;
using Amphora.Common.Models.Options;
using Amphora.Common.Security;
using IdentityServer4.Models;
using IdentityServer4.Stores;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Amphora.Identity.Stores.IdentityServer
{
    public class ConnectedClientStore : IClientStore
    {
        private readonly IEntityStore<ApplicationModel> store;
        private readonly ILogger<ConnectedClientStore> logger;
        private readonly string? secret;

        public ConnectedClientStore(IEntityStore<ApplicationModel> store,
                                    IOptions<MvcOptions> mvc,
                                    ILogger<ConnectedClientStore> logger)
        {
            this.store = store;
            this.logger = logger;
            this.secret = mvc.Value.MvcClientSecret;
        }

        public async Task<Client?> FindClientByIdAsync(string clientId)
        {
            var model = await store.ReadAsync(clientId);
            var client = ToClient(model);
            if (client is null)
            {
                logger.LogWarning($"No client found with id {clientId}");
            }
            else
            {
                logger.LogInformation($"Found client {clientId}");
                logger.LogInformation($"Client{clientId} has allowed CORS origins {string.Join(',', client.AllowedCorsOrigins)}");
            }

            return client;
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
                AllowedGrantTypes = model.AllowedGrantTypes ?? GrantTypes.Code,
                RequirePkce = true,
                AllowOfflineAccess = model.AllowOffline ?? false,
                RequireConsent = model.RequireConsent ?? true,
                RedirectUris = model.RedirectUris(),
                FrontChannelLogoutUri = model.LogoutUrl,
                PostLogoutRedirectUris = model.PostLogoutRedirects(),
                AllowedCorsOrigins = model.Origins,
                RequireClientSecret = model?.AllowedGrantTypes?.Contains("implicit") == true ? true : false,
                ClientSecrets = { new Secret(this.secret) },
                AllowedScopes = GetScopes(model!),
                // defaults
                // AccessTokenLifetime = 3600,
                // AbsoluteRefreshTokenLifetime = 30 * 24 * 60 * 60
            };
        }

        private ICollection<string> GetScopes(ApplicationModel model)
        {
            if (model.AllowedScopes != null && model.AllowedScopes.Any() && model.AreScopesValid())
            {
                // there are scopes in the application model'
                var result = new List<string>(model.AllowedScopes);
                if (!result.Contains(IdentityModel.OidcConstants.StandardScopes.OpenId))
                {
                    result.Add(IdentityModel.OidcConstants.StandardScopes.OpenId);
                }

                return result;
            }
            else
            {
                // just return the standard list of scopes
                var scopes = new List<string>
                {
                    IdentityModel.OidcConstants.StandardScopes.OpenId,
                    IdentityModel.OidcConstants.StandardScopes.Profile,
                    IdentityModel.OidcConstants.StandardScopes.Email,
                    Common.Security.Resources.WebApi,
                    Scopes.AmphoraScope,
                    Scopes.PurchaseScope
                };

                if (model?.AllowOffline == true)
                {
                    scopes.Add(IdentityModel.OidcConstants.StandardScopes.OfflineAccess);
                }

                return scopes;
            }
        }
    }
}
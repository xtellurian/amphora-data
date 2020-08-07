using System.Collections.Generic;

namespace Amphora.Api.Models.Dtos.Applications
{
    public abstract class ApplicationBase
    {
        /// <summary>
        /// Gets or sets the name of your application.
        /// Will be shown on the consent page.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets a url for front channel HTTP logouts.
        /// </summary>
        public string LogoutUrl { get; set; }

        /// <summary>
        /// Gets or sets the allowed scopes for the app.
        /// openid is not required, and will be automatically included.
        /// Options include: ['amphora', 'amphora.purchase', 'profile', 'email', 'web_api'].
        /// </summary>
        public List<string> AllowedScopes { get; set; }
    }
}
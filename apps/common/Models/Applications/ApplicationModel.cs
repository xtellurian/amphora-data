using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Amphora.Common.Models.Applications
{
    public class ApplicationModel : EntityBase
    {
        public string OrganisationId { get; set; } = null!;
        public string Name { get; set; } = null!;
        public bool? AllowOffline { get; set; }
        public bool? RequireConsent { get; set; }
        public ICollection<string> RedirectUris { get; set; } = new Collection<string>();
        public ICollection<string> PostLogoutRedirects { get; set; } = new Collection<string>();
        public string LogoutUrl { get; set; } = null!;
    }
}
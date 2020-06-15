using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Amphora.Common.Models.Applications
{
    public class ApplicationModel : EntityBase
    {
        public string OrganisationId { get; set; } = null!;
        public string Name { get; set; } = null!;
        public bool? AllowOffline { get; set; }
        public bool? RequireConsent { get; set; }
        public virtual ICollection<ApplicationLocationModel> Locations { get; set; } = new Collection<ApplicationLocationModel>();
        public string LogoutUrl { get; set; } = null!;
        public ICollection<string>? Origins => Locations?.Select(_ => _.Origin).ToList() ?? new List<string>();
        public ICollection<string> RedirectUris()
        {
            var results = new List<string>();
            if (Locations == null || Locations.Count == 0)
            {
                return results;
            }
            else
            {
                foreach (var loc in Locations)
                {
                    results.AddRange(loc.RedirectUris());
                }
            }

            return results;
        }

        public ICollection<string> PostLogoutRedirects()
        {
            var results = new List<string>();
            if (Locations == null || Locations.Count == 0)
            {
                return results;
            }
            else
            {
                foreach (var loc in Locations)
                {
                    results.AddRange(loc.PostLogoutRedirects);
                }
            }

            return results;
        }
    }
}
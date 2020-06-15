using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Amphora.Common.Models.Applications
{
    public class ApplicationLocationModel : EntityBase
    {
        public string ApplicationId { get; set; } = null!;
        public string Origin { get; set; } = null!;
        public ICollection<string> AllowedRedirectPaths { get; set; } = new Collection<string>();
        public ICollection<string> PostLogoutRedirects { get; set; } = new Collection<string>();
        public ICollection<string> RedirectUris()
        {
            if (this.AllowedRedirectPaths == null)
            {
                return new List<string>();
            }
            else
            {
                return this.AllowedRedirectPaths.Select(_ => $"{Origin}{_}").ToList();
            }
        }
    }
}
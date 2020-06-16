using System.Collections.Generic;

namespace Amphora.Identity.Models.ViewModels.Consent
{
    public class ConsentViewModel : ConsentInputModel
    {
        public string ClientName { get; set; } = null!;
        public string? ClientUrl { get; set; }
        public string? ClientLogoUrl { get; set; }
        public bool AllowRememberConsent { get; set; }

        public IEnumerable<ScopeViewModel> IdentityScopes { get; set; } = new List<ScopeViewModel>();
        public IEnumerable<ScopeViewModel> ResourceScopes { get; set; } = new List<ScopeViewModel>();
    }
}
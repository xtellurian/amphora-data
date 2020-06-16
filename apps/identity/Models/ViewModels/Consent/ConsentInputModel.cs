using System.Collections.Generic;

namespace Amphora.Identity.Models.ViewModels.Consent
{
    public class ConsentInputModel
    {
        public string? Button { get; set; }
        public IEnumerable<string> ScopesConsented { get; set; } = new List<string>();
        public bool RememberConsent { get; set; }
        public string? ReturnUrl { get; set; }
    }
}
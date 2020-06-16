namespace Amphora.Identity.Models.ViewModels.Consent
{
    public class ProcessConsentResult
    {
        public bool IsRedirect => RedirectUri != null;
        public string RedirectUri { get; set; } = null!;
        public string ClientId { get; set; } = null!;

        public bool ShowView => ViewModel != null;
        public ConsentViewModel? ViewModel { get; set; }

        public bool HasValidationError => ValidationError != null;
        public string? ValidationError { get; set; }
    }
}
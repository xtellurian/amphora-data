namespace Amphora.Identity.Models.ViewModels.Consent
{
    public class ScopeViewModel
    {
        public int Index { get; set; }
        public string? Name { get; set; }
        public string? DisplayName { get; set; }
        public string? Description { get; set; }
        public bool Emphasize { get; set; }
        public bool Required { get; set; }
        public bool Checked { get; set; }
    }
}
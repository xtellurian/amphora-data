namespace Amphora.Common.Models.Organisations
{
    public class TermsAndConditionsModel
    {
        public string Name { get; set; }
        public string Contents { get; set; }
        public string OrganisationId { get; set; }
        public virtual OrganisationModel Organisation { get; set; }
    }
}
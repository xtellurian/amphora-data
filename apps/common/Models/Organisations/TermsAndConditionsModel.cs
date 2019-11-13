using System;

namespace Amphora.Common.Models.Organisations
{
    public class TermsAndConditionsModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Contents { get; set; }
        // Navigation
        public string OrganisationId { get; set; }
        public virtual OrganisationModel Organisation { get; set; }
    }
}
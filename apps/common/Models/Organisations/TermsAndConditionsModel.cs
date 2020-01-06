using System;

namespace Amphora.Common.Models.Organisations
{
    public class TermsAndConditionsModel
    {
        public TermsAndConditionsModel(string id, string name, string contents)
        {
            Id = id;
            Name = name;
            Contents = contents;
        }

        public string Id { get; set; }
        public string Name { get; set; }
        public string Contents { get; set; }
        // Navigation
        public string OrganisationId { get; set; } = null!;
        public virtual OrganisationModel Organisation { get; set; } = null!;
    }
}
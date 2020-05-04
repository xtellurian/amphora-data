using System;
using Amphora.Common.Models.Organisations;

namespace Amphora.Common.Models.Amphorae
{
    public class TermsOfUseModel
    {
        [Obsolete]
        public TermsOfUseModel(string id, string name, string contents)
        {
            Id = id;
            Name = name;
            Contents = contents;
        }

        public TermsOfUseModel(string name, string contents)
        {
            Id = null!;
            Name = name;
            Contents = contents;
        }

        public string Id { get; set; }
        public string Name { get; set; }
        public string Contents { get; set; }

        // Navigation
        public string? OrganisationId { get; set; }
        public virtual OrganisationModel? Organisation { get; set; }
    }
}
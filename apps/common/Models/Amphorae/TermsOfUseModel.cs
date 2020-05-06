using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Amphora.Common.Models.Organisations;

namespace Amphora.Common.Models.Amphorae
{
    public class TermsOfUseModel : EntityBase
    {
        public TermsOfUseModel(string name, string contents)
        {
            Id = null!;
            Name = name;
            Contents = contents;
        }

        public string Name { get; set; }
        public string Contents { get; set; }

        // Navigation
        public string? OrganisationId { get; set; }
        public virtual OrganisationModel? Organisation { get; set; }
        public virtual ICollection<AmphoraModel> AppliedToAmphoras { get; set; } = new Collection<AmphoraModel>();
    }
}
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Amphora.Common.Contracts;
using Amphora.Common.Models.Organisations;

namespace Amphora.Common.Models.Activities
{
    public class ActivityModel : EntityBase
    {
        public string? Name { get; set; }
        // navigation
        public string? OrganisationId { get; set; }
        public virtual OrganisationModel? Organisation { get; set; }
        public virtual ICollection<ActivityRunModel>? Runs { get; set; } = new Collection<ActivityRunModel>();
    }
}
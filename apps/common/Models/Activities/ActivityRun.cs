using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Amphora.Common.Models.Activities
{
    public class ActivityRun
    {
        public int? Id { get; set; }
        public string? StartedByUserId { get; set; }
        public bool? Success { get; set; }
        public DateTimeOffset? StartTime { get; set; }
        public DateTimeOffset? EndTime { get; set; }
        public virtual ICollection<ActivityAmphoraReference>? AmphoraReferences { get; set; } = new Collection<ActivityAmphoraReference>();
    }
}
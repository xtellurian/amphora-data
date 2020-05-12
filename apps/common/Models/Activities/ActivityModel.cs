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
        public virtual ICollection<ActivityRun>? Runs { get; set; } = new Collection<ActivityRun>();
        // navigation
        public string? OrganisationId { get; set; }
        public virtual OrganisationModel? Organisation { get; set; }
        // methods
        public ActivityRun NewRun(DateTimeOffset startTime, IUser user)
        {
            var run = new ActivityRun()
            {
                StartedByUserId = user.Id,
                StartTime = startTime
            };
            Runs ??= new Collection<ActivityRun>();
            Runs.Add(run);
            return run;
        }
    }
}
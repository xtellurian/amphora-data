using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Amphora.Common.Contracts;
using Amphora.Common.Models.Users;

namespace Amphora.Common.Models.Activities
{
    public class ActivityRunModel : EntityBase
    {
        public ActivityRunModel() { }
        public ActivityRunModel(ActivityModel activity, IUser creator, DateTimeOffset startTime)
        {
            StartedByUserId = creator.Id;
            StartTime = startTime;
            ActivityId = activity.Id;
            Activity = activity;
        }

        public string? StartedByUserId { get; set; }
        public virtual ApplicationUserDataModel? StartedByUser { get; set; }
        public virtual VersionInfo? VersionInfo { get; set; }
        public bool? Success { get; set; }
        public DateTimeOffset? StartTime { get; set; }
        public DateTimeOffset? EndTime { get; set; }
        // navigation properties
        public string? ActivityId { get; set; }
        public virtual ActivityModel? Activity { get; set; }
        public virtual ICollection<ActivityAmphoraReference>? AmphoraReferences { get; set; } = new Collection<ActivityAmphoraReference>();
        // public virtual IList<string> Logs { get; set; } = new List<string>();
    }
}
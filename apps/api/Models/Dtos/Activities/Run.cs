using System;
using System.Collections.Generic;
using Amphora.Common.Models.Activities;

namespace Amphora.Api.Models.Dtos.Activities
{
    public class Run
    {
        public string Id { get; set; }
        public VersionInfo VersionInfo { get; set; }
        public string StartedBy { get; set; }
        public bool? Success { get; set; }
        public DateTimeOffset? StartTime { get; set; }
        public DateTimeOffset? EndTime { get; set; }
        public IEnumerable<AmphoraReference> AmphoraReferences { get; set; } = new List<AmphoraReference>();
    }
}
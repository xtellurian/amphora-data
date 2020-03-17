using System.Collections.Generic;
using Amphora.Common.Contracts;
using Amphora.Common.Models.Amphorae;
using Amphora.Common.Models.Users;

namespace Amphora.Common.Models.DataRequests
{
    public class DataRequestModel : EntityBase, ISearchable
    {
        public DataRequestModel(string name, string description, GeoLocation? geoLocation)
        {
            Name = name;
            Description = description;
            GeoLocation = geoLocation;
        }

        public string Name { get; set; }
        public string Description { get; set; }
        public GeoLocation? GeoLocation { get; set; }
        public List<string>? UserIdVotes { get; set; } = new List<string>();
        public virtual ApplicationUserDataModel? CreatedBy { get; set; } = null!;
        public string? CreatedById { get; set; } = null!;

        // methods
        public bool TryVote(string userId)
        {
            if (this.UserIdVotes == null)
            {
                this.UserIdVotes = new List<string>();
            }

            if (this.UserIdVotes.Contains(userId))
            {
                // already voted
                return false;
            }
            else
            {
                this.UserIdVotes.Add(userId);
                return true;
            }
        }
    }
}
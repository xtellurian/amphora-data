using Amphora.Common.Models.Organisations;

namespace Amphora.Common.Models.Platform
{
    public class InvitationModel : Entity
    {
        public InvitationModel()
        {

        }
        public string TargetEmail { get; set; }
        public string TargetDomain { get; set; }
        public bool? IsClaimed { get; set; }
        public bool? IsGlobalAdmin { get; set; }

        //navigation
        public string TargetOrganisationId { get; set; } // nullable
        public virtual OrganisationModel TargetOrganisation { get; set; }
    }
}
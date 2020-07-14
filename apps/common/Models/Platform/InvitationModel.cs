using Amphora.Common.Models.Organisations;

namespace Amphora.Common.Models.Platform
{
    public enum InvitationTrigger
    {
        Accept,
        Reject
    }

    public enum InvitationState
    {
        Open = 0,
        Accepted = 1,
        Rejected = 2
    }

    public class InvitationModel : EntityBase
    {
        public InvitationModel() { }
        public string? TargetEmail { get; set; }
        public string? TargetDomain { get; set; }
        public InvitationState? State { get; set; } = InvitationState.Open;

        // navigation
        public string? TargetOrganisationId { get; set; } // nullable
        public virtual OrganisationModel? TargetOrganisation { get; set; }
    }
}
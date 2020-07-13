using Amphora.Common.Models.Platform;
using Amphora.Workflow.Contracts;
using Stateless;

namespace Amphora.Workflow.StateMachines.Invitations
{
    public class InvitationWorkflow : IWorkflow<InvitationModel, InvitationTrigger>
    {
        public StateMachine<InvitationState, InvitationTrigger> Machine { get; }
        public InvitationModel Model { get; }

        public InvitationWorkflow(InvitationModel invitation)
        {
            this.Model = invitation;
            this.Machine = new StateMachine<InvitationState, InvitationTrigger>(
                () => Model.State ?? InvitationState.Open, // state accessor
                (s) => Model.State = s); // state mutator

            Machine.Configure(InvitationState.Open)
                .Permit(InvitationTrigger.Accept, InvitationState.Accepted)
                .Permit(InvitationTrigger.Reject, InvitationState.Rejected);
        }

        public void Transition(InvitationTrigger trigger)
        {
            Machine.Fire(trigger);
        }
    }
}
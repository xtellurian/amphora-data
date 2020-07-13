using Amphora.Common.Models.Platform;
using Amphora.Workflow.Contracts;
using Amphora.Workflow.StateMachines.Invitations;

namespace Amphora.Workflow
{
    public class WorkflowFactory : IWorkflows
    {
        public IWorkflow<InvitationModel, InvitationTrigger> InvitationWorkflow(InvitationModel model) => new InvitationWorkflow(model);
    }
}
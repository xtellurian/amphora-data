using Amphora.Common.Models.Platform;

namespace Amphora.Workflow.Contracts
{
    public interface IWorkflows
    {
        IWorkflow<InvitationModel, InvitationTrigger> InvitationWorkflow(InvitationModel model);
    }
}
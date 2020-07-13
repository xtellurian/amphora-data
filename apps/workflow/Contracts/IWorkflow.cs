namespace Amphora.Workflow.Contracts
{
    public interface IWorkflow<TModel, TTrigger>
    {
        void Transition(TTrigger trigger);
    }
}
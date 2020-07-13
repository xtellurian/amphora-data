using Amphora.Workflow.Contracts;
using Microsoft.Extensions.DependencyInjection;

namespace Amphora.Workflow
{
    public static class Registration
    {
        public static void RegisterWorkflows(this IServiceCollection services)
        {
            services.AddSingleton<IWorkflows, WorkflowFactory>();
        }
    }
}
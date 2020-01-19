using System.Collections.Generic;
using System.Threading.Tasks;

namespace Amphora.Common.Contracts
{
    public interface IEventHubSender
    {
        Task SendToEventHubAsync(IEnumerable<Dictionary<string, object?>> signals);
        Task SendToEventHubAsync(Dictionary<string, object> signal);
    }
}
using System.Threading.Tasks;
using Amphora.Common.Contracts;

namespace Amphora.Common.Contracts
{
    public interface IEventPublisher
    {
        Task PublishEventAsync(params IEvent[] events);
    }
}
using System.Threading.Tasks;

namespace Amphora.Common.Contracts
{
    public interface IEventPublisher
    {
        Task PublishEventAsync(params IEvent[] events);
    }
}
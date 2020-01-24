using System.Threading.Tasks;
using Amphora.Common.Contracts;

namespace Amphora.Api.Contracts
{
    public interface IEventPublisher
    {
        Task PublishEventAsync(params IEvent[] events);
    }
}
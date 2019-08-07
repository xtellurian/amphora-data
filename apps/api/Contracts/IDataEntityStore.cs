using System.Collections.Generic;
using System.Threading.Tasks;
using Amphora.Common.Contracts;

namespace Amphora.Api.Contracts
{
    public interface IOrgEntityStore<T> : IEntityStore<T> where T : IOrgEntity
    {
        Task<T> ReadAsync(string id, string orgId);
        Task<IList<T>> ListAsync(string orgId);
    }
}
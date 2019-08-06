using System.Collections.Generic;
using System.Threading.Tasks;
using Amphora.Common.Contracts;

namespace Amphora.Api.Contracts
{
    public interface IOrgEntityStore<T> where T : IOrgEntity
    {
        Task<T> GetAsync(string id, string orgId = "default");
        Task<T> SetAsync(T entity);
        Task<IEnumerable<T>> ListAsync(string orgId = "default");
    }
}
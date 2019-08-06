using System.Collections.Generic;
using System.Threading.Tasks;
using Amphora.Common.Contracts;

namespace Amphora.Api.Contracts
{
    public interface IEntityStore<T> where T : IEntity
    {
        Task<IEnumerable<T>> ListAsync();
        Task<T> GetAsync(string id);
        Task<T> SetAsync(T entity);
        
    }
}
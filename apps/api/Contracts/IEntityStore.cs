using System.Collections.Generic;
using System.Threading.Tasks;
using Amphora.Common.Contracts;

namespace Amphora.Api.Contracts
{
    public interface IEntityStore<T> where T : IEntity
    {
        Task<IList<T>> ListAsync();
        Task<T> CreateAsync(T entity);
        Task<T> ReadAsync(string id);
        Task<T> UpdateAsync(T entity);
        Task DeleteAsync(T entity);
        
    }
}
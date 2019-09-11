using System.Collections.Generic;
using System.Threading.Tasks;
using Amphora.Common.Contracts;

namespace Amphora.Api.Contracts
{
    public interface IEntityStore<T> where T : IEntity
    {
        Task<IList<T>> ListAsync();
        Task<IList<T>> ListAsync(string orgId);
        Task<T> CreateAsync(T entity);
        Task<T> ReadAsync(string id);
        Task<T> UpdateAsync(T entity);
        Task DeleteAsync(T entity);
        Task<IList<T>> StartsWithQueryAsync(string propertyName, string givenValue);
        Task<T> ReadAsync(string id, string orgId);
        Task<TExtended> ReadAsync<TExtended>(string id, string orgId) where TExtended: class, T;
        Task<IEnumerable<T>> QueryAsync(System.Func<T, bool> where);
    }
}
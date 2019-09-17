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
        Task<TExtended> CreateAsync<TExtended>(TExtended entity) where TExtended: class, T;
        Task<T> ReadAsync(string id);
        Task<TExtended> ReadAsync<TExtended>(string id) where TExtended: class, T;
        Task<T> ReadAsync(string id, string orgId);
        Task<TExtended> ReadAsync<TExtended>(string id, string orgId) where TExtended: class, T;
        Task<T> UpdateAsync(T entity);
        Task DeleteAsync(T entity);
        Task<IList<TExtended>> StartsWithQueryAsync<TExtended>(string propertyName, string givenValue) where TExtended: class, T;
        Task<IEnumerable<TQuery>> QueryAsync<TQuery>(System.Func<TQuery, bool> where) where TQuery : class, T;
    }
}
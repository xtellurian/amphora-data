using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Amphora.Common.Contracts;
using Amphora.Common.Models.Amphorae;

namespace Amphora.Api.Contracts
{
    public interface IEntityStore<T> where T : IEntity
    {
        Task<IList<T>> TopAsync();
        Task<IList<T>> TopAsync(string orgId);
        Task<T> CreateAsync(T entity);
        Task<T> ReadAsync(string id);
        Task<T> UpdateAsync(T entity);
        Task DeleteAsync(T entity);
        Task<IEnumerable<T>> QueryAsync(Expression<Func<T, bool>> where);
        IQueryable<T> Query(Expression<Func<T, bool>> where);
        Task<int> CountAsync(Expression<Func<T, bool>> where = null);
    }
}
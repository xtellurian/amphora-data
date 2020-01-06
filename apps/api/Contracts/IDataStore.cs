using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Amphora.Common.Contracts;

namespace Amphora.Api.Contracts
{
    [Obsolete]
    public interface IDataStore<T, TData> where T : IEntity
    {
        Task<TData> GetDataAsync(T entity, string path);
        Task<IEnumerable<string>> ListNamesAsync(T entity);
        Task<TData> SetDataAsync(T entity, TData data, string path);
    }
}
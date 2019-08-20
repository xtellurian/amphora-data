using System.Collections.Generic;
using System.Threading.Tasks;
using Amphora.Common.Contracts;

namespace Amphora.Api.Contracts
{
    public interface IDataStore<T,TData> where T: IOrgScoped
    {
        Task<TData> GetDataAsync(T entity, string name);
        Task<IEnumerable<string>> ListNames(T entity);
        Task<TData> SetDataAsync(T entity, TData data, string name);
    }
}
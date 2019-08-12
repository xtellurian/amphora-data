using Amphora.Common.Contracts;

namespace Amphora.Api.Contracts
{
    public interface IDataStore<T,TData> where T: IOrgScoped
    {
        TData GetData(T entity);
        TData SetData(T entity, TData data);
    }
}
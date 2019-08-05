using Amphora.Common.Contracts;

namespace Amphora.Api.Contracts
{
    public interface IDataStore<T,TData> where T: IDataEntity
    {
        TData GetData(T entity);
        TData SetData(T entity, TData data);
    }
}
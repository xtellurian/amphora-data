using System.Collections.Generic;
using common.Contracts;

namespace Amphora.Api.Contracts
{
    public interface IAmphoraEntityStore<T> where T : IAmphoraEntity
    {
        IReadOnlyCollection<T> List();
        T Get(string id);
        T Set(T entity);
        
    }
}
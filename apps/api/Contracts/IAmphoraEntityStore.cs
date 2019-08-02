using System.Collections.Generic;
using common.Contracts;

namespace api.Contracts
{
    public interface IAmphoraEntityStore<T> where T : IAmphoraEntity
    {
        IEnumerable<string> ListIds();
        T Get(string id);
        T Set(T entity);
        
    }
}
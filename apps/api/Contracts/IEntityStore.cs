using System.Collections.Generic;
using Amphora.Common.Contracts;

namespace Amphora.Api.Contracts
{
    public interface IEntityStore<T> where T : IEntity
    {
        IReadOnlyCollection<T> List();
        T Get(string id);
        T Set(T entity);
        
    }
}
using System.Collections.Generic;
using Amphora.Common.Contracts;

namespace Amphora.Api.Contracts
{
    public interface IDataEntityStore<T> : IEntityStore<T> where T : IDataEntity
    {
        T Get(string id, string orgId);
        IReadOnlyCollection<T> List(string orgId);
    }
}
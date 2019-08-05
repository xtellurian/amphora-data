using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Amphora.Api.Contracts;
using Amphora.Common.Contracts;

namespace Amphora.Api.Stores
{
    public class InMemoryDataEntityStore<T> : InMemoryEntityStore<T>, IDataEntityStore<T> where T: class, IDataEntity
    {
        public T Get(string id, string orgId)
        {
            return this.store.FirstOrDefault(o => string.Equals(o.Id, id) && string.Equals(o.OrgId, orgId));
        }

        public IReadOnlyCollection<T> List(string orgId)
        {
            return new ReadOnlyCollection<T>(this.store.Where(o => string.Equals(orgId, o.OrgId)).ToList());
        }
    }
}
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Common.Contracts;

namespace Amphora.Api.Stores
{
    public class InMemoryOrgEntityStore<T> : InMemoryEntityStore<T>, IOrgScopedEntityStore<T> where T : class, IOrgScoped
    {
        public Task<T> ReadAsync(string id, string orgId)
        {
            return Task<T>.Factory.StartNew(() =>
            {
                return this.store.FirstOrDefault(e => string.Equals(e.Id, id) && string.Equals(e.OrgId, orgId));
            });
        }

        public Task<IList<T>> ListAsync(string orgId)
        {
            return Task<IList<T>>.Factory.StartNew( () => 
            {
                return new ReadOnlyCollection<T>(this.store.Where(o => string.Equals(orgId, o.OrgId)).ToList());
            });
        }
    }
}
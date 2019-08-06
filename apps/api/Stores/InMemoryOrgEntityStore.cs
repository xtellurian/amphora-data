using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Common.Contracts;

namespace Amphora.Api.Stores
{
    public class InMemoryDataEntityStore<T> : InMemoryEntityStore<T>, IOrgEntityStore<T> where T : class, IOrgEntity
    {
        public Task<T> GetAsync(string id, string orgId)
        {
            return Task<T>.Factory.StartNew(() =>
            {
                return this.store.FirstOrDefault(e => string.Equals(e.Id, id) && string.Equals(e.OrgId, orgId));
            });
        }

        public Task<IEnumerable<T>> ListAsync(string orgId)
        {
            return Task<IEnumerable<T>>.Factory.StartNew( () => 
            {
                return new ReadOnlyCollection<T>(this.store.Where(o => string.Equals(orgId, o.OrgId)).ToList());
            });
        }
    }
}
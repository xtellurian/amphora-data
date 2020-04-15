using System.Collections.Generic;
using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Common.Contracts;
using Amphora.Common.Models.Amphorae;

namespace Amphora.Api.Stores.InMemory
{
    public class InMemoryAmphoraBlobStore : InMemoryBlobStore<AmphoraModel>, IAmphoraBlobStore
    {
        public Task<IList<IAmphoraFileReference>> GetFilesAsync(AmphoraModel entity)
        {
            var res = new List<IAmphoraFileReference>();
            if (store.ContainsKey(entity.Id))
            {
                var items = store[entity.Id];
                foreach (var kvp in items)
                {
                    res.Add(new InMemoryFileReference(kvp.Key));
                }
            }

            return Task.FromResult<IList<IAmphoraFileReference>>(res);
        }

        public class InMemoryFileReference : IAmphoraFileReference
        {
            public InMemoryFileReference(string name)
            {
                Name = name;
            }

            public string Name { get; }
        }
    }
}
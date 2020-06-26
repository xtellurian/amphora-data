using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Amphora.Common.Contracts;
using Amphora.Common.Models.Amphorae;

namespace Amphora.Api.Stores.InMemory
{
    public class InMemoryAmphoraBlobStore : InMemoryBlobStore<AmphoraModel>, IAmphoraBlobStore
    {
        public InMemoryAmphoraBlobStore(IDateTimeProvider dateTimeProvider) : base(dateTimeProvider)
        {
        }

        public async Task<IList<IAmphoraFileReference>> GetFilesAsync(AmphoraModel entity, string prefix = null, int? segmentSize = null)
        {
            var res = new List<IAmphoraFileReference>();
            if (store.ContainsKey(entity.Id))
            {
                var items = store[entity.Id];
                foreach (var kvp in items)
                {
                    res.Add(new InMemoryFileReference(kvp.Key, lastModified[entity.Id][kvp.Key], await ReadAttributes(entity, kvp.Key)));
                }
            }

            return res;
        }

        public Task<IDictionary<string, string>> ReadAttributes(AmphoraModel entity, string path)
        {
            if (metadata.ContainsKey(entity.Id))
            {
                // then get the files metadata
                if (metadata[entity.Id].ContainsKey(path))
                {
                    return Task.FromResult(metadata[entity.Id][path]);
                }
            }

            // no metadata found. return default
            return Task.FromResult<IDictionary<string, string>>(new Dictionary<string, string>());
        }

        public Task WriteAttributes(AmphoraModel entity, string path, IDictionary<string, string> attributes)
        {
            if (!metadata.ContainsKey(entity.Id))
            {
                metadata[entity.Id] = new Dictionary<string, IDictionary<string, string>>();
            }

            metadata[entity.Id][path] = attributes;
            return Task.CompletedTask;
        }

        public class InMemoryFileReference : IAmphoraFileReference
        {
            private readonly IDictionary<string, string> attributes;
            public InMemoryFileReference(string name, DateTimeOffset? lastModified, IDictionary<string, string> attributes)
            {
                Name = name;
                this.attributes = attributes;
                LastModified = lastModified;
            }

            public string Name { get; }
            public DateTimeOffset? LastModified { get; }

            public IDictionary<string, string> Metadata => attributes;

            public Task LoadAttributesAsync()
            {
                return Task.CompletedTask;
            }
        }
    }
}
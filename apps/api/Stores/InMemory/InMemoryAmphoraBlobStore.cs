using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amphora.Common.Contracts;
using Amphora.Common.Models.Amphorae;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Amphora.Api.Stores.InMemory
{
    public class InMemoryAmphoraBlobStore : InMemoryBlobStore<AmphoraModel>, IAmphoraBlobStore
    {
        private static int instanceCount = 0;
        public InMemoryAmphoraBlobStore(IDateTimeProvider dateTimeProvider, ILogger<InMemoryAmphoraBlobStore> logger)
        : base(dateTimeProvider, logger)
        {
            instanceCount++;
            if (instanceCount > 1)
            {
                // throw new InMemoryStoreException("Multiple stores detected!");
            }
        }

        public async Task<int> CountFilesAsync(AmphoraModel entity, string prefix = null)
        {
            return (await this.GetFilesAsync(entity, prefix, take: 10000)).Count;
        }

        public async Task<IList<IAmphoraFileReference>> GetFilesAsync(AmphoraModel entity, string prefix = null, int skip = 0, int take = 64)
        {
            var res = new List<IAmphoraFileReference>();
            IEnumerable<KeyValuePair<string, byte[]>> filtered = new List<KeyValuePair<string, byte[]>>();
            lock (DataContainersLock)
            {
                if (DataContainers.TryGetValue(entity.Id, out var items))
                {
                    filtered = items
                        .Where(i => string.IsNullOrEmpty(prefix) || i.Key.StartsWith(prefix))
                        .Skip(skip)
                        .Take(take);
                }
                else if (entity.Name == "xxx")
                {
                    throw new InMemoryStoreException($"{entity.Id} was not in: " + JsonConvert.SerializeObject(DataContainers.Keys));
                }
            }

            foreach (var kvp in filtered)
            {
                var lastModified = dateTimeProvider.MinValue;
                if (LastModified.TryGetValue(entity.Id, out var lmValues))
                {
                    if (lmValues.TryGetValue(kvp.Key, out var lm))
                    {
                        lastModified = lm.HasValue ? lm.Value : lastModified;
                    }
                }

                res.Add(new InMemoryFileReference(kvp.Key, lastModified, await ReadAttributes(entity, kvp.Key)));
            }

            return res;
        }

        public Task<IDictionary<string, string>> ReadAttributes(AmphoraModel entity, string path)
        {
            lock (MetadataLock)
            {
                if (Metadata.ContainsKey(entity.Id))
                {
                    // then get the files metadata
                    if (Metadata[entity.Id].ContainsKey(path))
                    {
                        return Task.FromResult<IDictionary<string, string>>(Metadata[entity.Id][path]);
                    }
                }
            }

            // no metadata found. return default
            return Task.FromResult<IDictionary<string, string>>(new Dictionary<string, string>());
        }

        public Task WriteAttributes(AmphoraModel entity, string path, IDictionary<string, string> attributes)
        {
            lock (MetadataLock)
            {
                if (!Metadata.ContainsKey(entity.Id))
                {
                    if (!Metadata.TryAdd(entity.Id, new ConcurrentDictionary<string, ConcurrentDictionary<string, string>>()))
                    {
                        throw new InMemoryStoreException($"Failed to add {entity.Id} to Metadata");
                    }
                }
            }

            if (Metadata.TryGetValue(entity.Id, out var container))
            {
                if (!container.TryAdd(path, new ConcurrentDictionary<string, string>(attributes)))
                {
                    throw new InMemoryStoreException($"Failed to add Metadata attributes for {entity.Id}");
                }
            }

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
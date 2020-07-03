using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Amphora.Api.Extensions;
using Amphora.Common.Contracts;
using Microsoft.Extensions.Logging;

namespace Amphora.Api.Stores.InMemory
{
    public class InMemoryBlobStore<T> : IBlobStore<T> where T : class, IEntity
    {
        protected static readonly object DataContainersLock = new object();
        protected static readonly ConcurrentDictionary<string, ConcurrentDictionary<string, byte[]>> DataContainers = new ConcurrentDictionary<string, ConcurrentDictionary<string, byte[]>>();
        protected static readonly object MetadataLock = new object();
        protected static readonly ConcurrentDictionary<string, ConcurrentDictionary<string, ConcurrentDictionary<string, string>>> Metadata
            = new ConcurrentDictionary<string, ConcurrentDictionary<string, ConcurrentDictionary<string, string>>>();
        protected static readonly object LastModifiedLock = new object();
        protected static readonly ConcurrentDictionary<string, ConcurrentDictionary<string, DateTimeOffset?>> LastModified
            = new ConcurrentDictionary<string, ConcurrentDictionary<string, DateTimeOffset?>>();

        protected readonly IDateTimeProvider dateTimeProvider;
        protected readonly ILogger<InMemoryBlobStore<T>> logger;

        public InMemoryBlobStore(IDateTimeProvider dateTimeProvider, ILogger<InMemoryBlobStore<T>> logger)
        {
            this.dateTimeProvider = dateTimeProvider;
            this.logger = logger;
        }

        public Task<byte[]> ReadBytesAsync(T entity, string path)
        {
            if (entity?.Id == null) { throw new ArgumentException(); }
            lock (DataContainersLock)
            {
                if (DataContainers.TryGetValue(entity.Id, out var container))
                {
                    if (container.TryGetValue(path, out var data))
                    {
                        return Task.FromResult(data.ToArray()); // copy the data
                    }
                    else
                    {
                        return Task.FromResult(null as byte[]);
                    }
                }
                else
                {
                    return Task.FromResult(null as byte[]);
                }
            }
        }

        public Task WriteBytesAsync(T entity, string path, byte[] bytes)
        {
            if (string.IsNullOrEmpty(entity.Id))
            {
                throw new ArgumentNullException("Entity.Id must not be null");
                // entity.Id = Guid.NewGuid().ToString();
            }

            lock (DataContainersLock)
            {
                if (!DataContainers.ContainsKey(entity.Id))
                {
                    DataContainers.TryAdd(entity.Id, new ConcurrentDictionary<string, byte[]>());
                }
            }

            lock (LastModifiedLock)
            {
                if (!LastModified.ContainsKey(entity.Id))
                {
                    LastModified.TryAdd(entity.Id, new ConcurrentDictionary<string, DateTimeOffset?>());
                }
            }

            lock (DataContainersLock)
            {
                if (DataContainers.TryGetValue(entity.Id, out var container))
                {
                    if (!container.TryAdd(path, bytes.ToArray())) // copy the data to a new reference
                    {
                        throw new InMemoryStoreException($"Failed to add data to dataContainer({entity.Id}), path: {path}");
                    }
                }
                else
                {
                    throw new InMemoryStoreException($"Expected dataContainers to contain {entity.Id}");
                }
            }

            lock (LastModifiedLock)
            {
                if (LastModified.TryGetValue(entity.Id, out var lastModifiedContainer))
                {
                    if (!lastModifiedContainer.TryAdd(path, dateTimeProvider.UtcNow))
                    {
                        throw new InMemoryStoreException($"Failed to add last Modified to ({entity.Id}), path: {path}");
                    }
                }
                else
                {
                    throw new InMemoryStoreException($"Expected lastModified to contain {entity.Id}");
                }
            }

            return Task.CompletedTask;
        }

        public Task<string> GetPublicUrl(T entity, string path)
        {
            return Task.FromResult("~/images/amphora_2.png");
        }

        public Task<IList<string>> ListBlobsAsync(T entity)
        {
            lock (DataContainersLock)
            {
                if (DataContainers.ContainsKey(entity.Id))
                {
                    var entityData = DataContainers[entity.Id];
                    return Task.FromResult(entityData.Keys.ToList() as IList<string>);
                }
                else
                {
                    return Task.FromResult(new List<string>() as IList<string>);
                }
            }
        }

        public Task<string> GetWritableUrl(T entity, string fileName)
        {
            // return the relative url
            return Task.FromResult($"/api/amphorae/{entity.Id}/files/{fileName}");
        }

        public Task<DateTimeOffset?> LastModifiedAsync(T entity)
        {
            return Task.FromResult<DateTimeOffset?>(DateTimeOffset.Now);
        }

        public Task<bool> ExistsAsync(T entity, string path)
        {
            lock (DataContainersLock)
            {
                if (DataContainers.TryGetValue(entity.Id, out var container))
                {
                    return Task.FromResult(container.ContainsKey(path));
                }
            }

            logger.LogWarning($"Entity({entity.Id}) has no file named {path}");
            return Task.FromResult(false);
        }

        public Task<Stream> GetWritableStreamAsync(T entity, string path)
        {
            return Task.FromResult<Stream>(new MemoryStream());
        }

        public Task<bool> DeleteAsync(T entity, string path)
        {
            lock (DataContainersLock)
            {
                if (DataContainers.TryGetValue(entity.Id, out var container))
                {
                    if (container.ContainsKey(path))
                    {
                        if (container.TryRemove(path, out var value))
                        {
                            logger.LogInformation($"Removed {value?.Length} bytes from container({entity.Id})");
                        }
                        else
                        {
                            logger.LogWarning($"Error when removing data from container({entity.Id})");
                            return Task.FromResult(false);
                        }

                        return Task.FromResult(true);
                    }
                }
            }

            return Task.FromResult(false);
        }

        public Task<long> GetContainerSizeAsync(T entity)
        {
            return Task.FromResult<long>(1024);
        }

        public async Task WriteAsync(T entity, string path, Stream content)
        {
            await WriteBytesAsync(entity, path, await content.ReadFullyAsync());
        }
    }
}
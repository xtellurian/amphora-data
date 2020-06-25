using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Api.Extensions;
using Amphora.Common.Contracts;

namespace Amphora.Api.Stores.InMemory
{
    public class InMemoryBlobStore<T> : IBlobStore<T> where T : class, IEntity
    {
        protected Dictionary<string, Dictionary<string, byte[]>> store = new Dictionary<string, Dictionary<string, byte[]>>();
        protected Dictionary<string, Dictionary<string, IDictionary<string, string>>> metadata
            = new Dictionary<string, Dictionary<string, IDictionary<string, string>>>();
        public Task<byte[]> ReadBytesAsync(T entity, string path)
        {
            return Task<byte[]>.Factory.StartNew(() =>
            {
                if (entity?.Id == null) { throw new ArgumentException(); }
                if (store.ContainsKey(entity.Id))
                {
                    var dataStore = store[entity.Id];
                    if (dataStore.ContainsKey(path))
                    {
                        return dataStore[path];
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    return null;
                }
            });
        }

        public Task WriteBytesAsync(T entity, string path, byte[] bytes)
        {
            if (string.IsNullOrEmpty(entity.Id))
            {
                entity.Id = Guid.NewGuid().ToString();
            }

            if (!store.ContainsKey(entity.Id))
            {
                store[entity.Id] = new Dictionary<string, byte[]>();
            }

            var dataStore = store[entity.Id];
            dataStore[path] = bytes;
            return Task.CompletedTask;
        }

        public Task<string> GetPublicUrl(T entity, string path)
        {
            return Task.FromResult("~/images/amphora_2.png");
        }

        public Task<IList<string>> ListBlobsAsync(T entity)
        {
            return Task<IList<string>>.Factory.StartNew(() =>
            {
                if (store.ContainsKey(entity.Id))
                {
                    var entityData = store[entity.Id];
                    return entityData.Keys.ToList();
                }
                else
                {
                    return new List<string>();
                }
            });
        }

        public Task<string> GetWritableUrl(T entity, string fileName)
        {
            return Task.FromResult(string.Empty);
        }

        public Task<DateTimeOffset?> LastModifiedAsync(T entity)
        {
            return Task.FromResult<DateTimeOffset?>(DateTimeOffset.Now);
        }

        public Task<bool> ExistsAsync(T entity, string path)
        {
            if (store.ContainsKey(entity.Id))
            {
                return Task.FromResult(store[entity.Id].ContainsKey(path));
            }
            else
            {
                return Task.FromResult(false);
            }
        }

        public Task<Stream> GetWritableStreamAsync(T entity, string path)
        {
            return Task.FromResult<Stream>(new MemoryStream());
        }

        public Task<bool> DeleteAsync(T entity, string path)
        {
            if (this.store.TryGetValue(entity.Id, out var x))
            {
                if (x.ContainsKey(path))
                {
                    x.Remove(path);
                    return Task.FromResult(true);
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
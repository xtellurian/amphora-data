using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Common.Contracts;

namespace Amphora.Api.Stores
{
    public class InMemoryBlobStore<T> : IBlobStore<T> where T : class, IEntity
    {
        private Dictionary<string, Dictionary<string, byte[]>> store = new Dictionary<string, Dictionary<string, byte[]>>();
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
    }
}
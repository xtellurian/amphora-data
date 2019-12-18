using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Amphora.Common.Contracts;
using Amphora.Common.Models.Amphorae;

namespace Amphora.Api.Contracts
{
    public interface IBlobStore<T> where T : IEntity
    {
        Task<string> GetPublicUrl(T entity, string path);
        Task<string> GetWritableUrl(T entity, string fileName);
        Task<DateTimeOffset?> LastModifiedAsync(T entity);
        Task<IList<string>> ListBlobsAsync(T entity);
        Task<byte[]> ReadBytesAsync(T entity, string path);
        Task WriteBytesAsync(T entity, string path, byte[] bytes);
    }
}
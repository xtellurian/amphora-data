using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Amphora.Common.Contracts;

namespace Amphora.Common.Contracts
{
    public interface IBlobStore<T> where T : IEntity
    {
        Task<bool> ExistsAsync(T entity, string path);
        Task<string> GetPublicUrl(T entity, string path);
        Task<long> GetContainerSizeAsync(T entity);
        Task WriteAsync(T entity, string path, Stream content);
        Task<string> GetWritableUrl(T entity, string fileName);
        Task<DateTimeOffset?> LastModifiedAsync(T entity);
        Task<byte[]> ReadBytesAsync(T entity, string path);
        Task WriteBytesAsync(T entity, string path, byte[] bytes);
        Task<bool> DeleteAsync(T entity, string path);
    }
}
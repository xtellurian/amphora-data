using System.Collections.Generic;
using System.Threading.Tasks;
using Amphora.Common.Contracts;

namespace Amphora.Api.Contracts
{
    public interface IBlobStore<T> where T : IEntity
    {
        Task<string> GetPublicUrl(T entity, string path);
        Task<IList<string>> ListBlobsAsync(T entity);
        Task<byte[]> ReadBytesAsync(T entity, string path);
        Task WriteBytesAsync(T entity, string path, byte[] bytes);
    }
}
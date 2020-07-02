using System.Collections.Generic;
using System.Threading.Tasks;
using Amphora.Common.Models.Amphorae;

namespace Amphora.Common.Contracts
{
    public interface IAmphoraBlobStore : IBlobStore<AmphoraModel>
    {
        Task<IList<IAmphoraFileReference>> GetFilesAsync(AmphoraModel entity, string? prefix = null, int skip = 0, int take = 64);
        Task<int> CountFilesAsync(AmphoraModel entity, string? prefix = null);
        Task WriteAttributes(AmphoraModel entity, string path, IDictionary<string, string> attributes);
        Task<IDictionary<string, string>> ReadAttributes(AmphoraModel entity, string path);
    }
}
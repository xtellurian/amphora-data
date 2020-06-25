using System.Collections.Generic;
using System.Threading.Tasks;
using Amphora.Common.Contracts;
using Amphora.Common.Models.Amphorae;

namespace Amphora.Common.Contracts
{
    public interface IAmphoraBlobStore : IBlobStore<AmphoraModel>
    {
        Task<IList<IAmphoraFileReference>> GetFilesAsync(AmphoraModel entity, string? prefix = null, int? segmentSize = null);
        Task WriteAttributes(AmphoraModel entity, string path, IDictionary<string, string> attributes);
        Task<IDictionary<string, string>> ReadAttributes(AmphoraModel entity, string path);
    }
}
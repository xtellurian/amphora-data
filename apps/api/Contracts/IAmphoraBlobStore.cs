using System.Collections.Generic;
using System.Threading.Tasks;
using Amphora.Common.Contracts;
using Amphora.Common.Models.Amphorae;

namespace Amphora.Api.Contracts
{
    public interface IAmphoraBlobStore : IBlobStore<AmphoraModel>
    {
        Task<IList<IAmphoraFileReference>> GetFilesAsync(AmphoraModel entity);
    }
}
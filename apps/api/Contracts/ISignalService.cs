using System.Threading.Tasks;
using Amphora.Common.Models.Amphorae;

namespace Amphora.Api.Contracts
{
    public interface ISignalService
    {
        Task WriteSignalAsync(AmphoraModel entity, Common.Models.Domains.Datum d);
    }
}
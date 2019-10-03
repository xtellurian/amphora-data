using System.Threading.Tasks;
using Amphora.Common.Models.Amphorae;
using Newtonsoft.Json.Linq;

namespace Amphora.Api.Contracts
{
    public interface ISignalService
    {
        Task WriteSignalAsync(AmphoraModel entity, JObject jObj);
    }
}
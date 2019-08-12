using System.Threading.Tasks;

namespace Amphora.Api.Contracts
{
    public interface ITemporaPayloadValidationService
    {
        Task<bool> IsValidAsync(Common.Models.Tempora tempora, Newtonsoft.Json.Linq.JObject payload);
    }
}
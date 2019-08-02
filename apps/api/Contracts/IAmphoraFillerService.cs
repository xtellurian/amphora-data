using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace api.Contracts
{
    public interface IAmphoraFillerService
    {
        Task FillWithJson(string amphoraId, IEnumerable<JObject> jsonPayloads);
    }
}
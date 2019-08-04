using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Amphora.Api.Contracts
{
    public interface IAmphoraFillerService
    {
        Task FillWithBinary(string amphoraId, System.IO.Stream data);
        Task FillWithJson(string amphoraId, IEnumerable<JObject> jsonPayloads);
    }
}
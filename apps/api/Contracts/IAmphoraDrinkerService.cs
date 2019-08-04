using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Amphora.Api.Contracts
{
    public interface IAmphoraDrinkerService
    {
        Task<Stream> DrinkBinary(string amphoraId);
    }
}
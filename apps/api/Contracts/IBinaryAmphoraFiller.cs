using System.IO;
using System.Threading.Tasks;
using Amphora.Common.Models;

namespace Amphora.Api.Contracts
{
    public interface IBinaryAmphoraFiller
    {
        Task Fill(AmphoraModel amphora, Stream data);
        bool IsAmphoraSupported(Common.Models.AmphoraModel amphora);
    }
    public interface IBinaryAmphoraDrinker
    {
        Task<Stream> Drink(AmphoraModel amphora);
        bool IsAmphoraSupported(Common.Models.AmphoraModel amphora);
    }
}